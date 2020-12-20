using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using RoboControler.Helper;
using RoboControler.Models;
using System.Diagnostics;

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace RoboControler
{
    public class Main : Game
    {
        private IPAddress iPAddress = IPAddress.Parse("192.168.8.124");

        //Controller definition
        private TcpClient controllerTCP = new TcpClient();
        private string controllerPort = "8888";
        NetworkStream controllerStream;
        bool isControllerConnected = false;

        //CamaraStream definition
        private TcpClient camaraStreamTCP = new TcpClient();
        private string camaraStreamPort = "8889";
        NetworkStream camaraStreamStream; 
        Texture2D camView;
        private List<string> frames = new List<string>() { "" };
        bool isCamaraStreamConnected = false;

        //Status monitor definition
        private TcpClient statusMonitorTCP = new TcpClient();
        private string statusMonitorPort = "8887";
        NetworkStream statusMonitorStream;
        bool isStatusMonitorConnected = false;
        string status = "";

        //UI interfaces
        myIndicator statusIndicator = new myIndicator();
        myIndicator operateIndicator = new myIndicator();

        SpriteFont font;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Try connect to the robo.
            controllerTCP.NoDelay = true;
            controllerTCP.BeginConnect(iPAddress,
                Convert.ToInt32(controllerPort),
                new AsyncCallback(controllerConnectCallback),
                controllerTCP);

            //Try connect to the camara stream.
            camaraStreamTCP.BeginConnect(iPAddress,
                Convert.ToInt32(camaraStreamPort),
                new AsyncCallback(camaraStreamConnectCallback),
                camaraStreamTCP);

            //Try connect to the status monitor.
            statusMonitorTCP.BeginConnect(iPAddress,
                Convert.ToInt32(statusMonitorPort),
                new AsyncCallback(statusMonitorConnectCallback),
                statusMonitorTCP);

            base.Initialize();
        }
        private void controllerConnectCallback(IAsyncResult ar)
        {
            TcpClient t = (TcpClient)ar.AsyncState;
            try
            {
                if (t.Connected)
                {
                    controllerStream = controllerTCP.GetStream();
                    isControllerConnected = true;
                }
            }
            catch { }
        }
        private void camaraStreamConnectCallback(IAsyncResult ar)
        {
            TcpClient t = (TcpClient)ar.AsyncState;
            try
            {
                if (t.Connected)
                {
                    camaraStreamStream = camaraStreamTCP.GetStream();
                    isCamaraStreamConnected = true;
                }
            }
            catch { }
        }
        private void statusMonitorConnectCallback(IAsyncResult ar)
        {
            TcpClient t = (TcpClient)ar.AsyncState;
            try
            {
                if (t.Connected)
                {
                    statusMonitorStream = statusMonitorTCP.GetStream();
                    isStatusMonitorConnected = true;
                }
            }
            catch { }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //Load no signal image
            camView = Content.Load<Texture2D>("ns");

            //Load UI resource
            #region Load status indicator
            statusIndicator.icon = new List<Texture2D>()
            {
                Content.Load<Texture2D>("cam"),
                Content.Load<Texture2D>("cpu"),
                Content.Load<Texture2D>("bat"),
                Content.Load<Texture2D>("gpd")
            };
            statusIndicator.isEnable = new List<bool>() { false, false, false, false };
            statusIndicator.mask = Content.Load<Texture2D>("no");
            #endregion

            #region Load operate indicator
            operateIndicator.icon = new List<Texture2D>()
            {
                Content.Load<Texture2D>("lt"),
                Content.Load<Texture2D>("rt")
            };
            operateIndicator.isEnable = new List<bool>() { true, true };
            operateIndicator.mask = Content.Load<Texture2D>("no");
            #endregion

            font = Content.Load<SpriteFont>("File");

            //Set resolution
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 ;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GamePadState gamePadOne = GamePad.GetState(PlayerIndex.One);

            Vector2 leftPosition = gamePadOne.ThumbSticks.Left;
            Vector2 rightPosition = gamePadOne.ThumbSticks.Right;
            float leftTrigger = gamePadOne.Triggers.Left;
            float rightTrigger = gamePadOne.Triggers.Right;

            #region Controller vibration
            float vibration;
            if(MathHelper_2D.GetDistance(leftPosition, Vector2.Zero) > MathHelper_2D.GetDistance(rightPosition, Vector2.Zero))
                vibration = (float)MathHelper_2D.GetDistance(leftPosition, Vector2.Zero) / 50;
            else
                vibration = (float)MathHelper_2D.GetDistance(rightPosition, Vector2.Zero) / 50;
            if (leftTrigger == 0 && rightTrigger == 0)
                GamePad.SetVibration(PlayerIndex.One, vibration, vibration);
            #endregion

            //Communicate with robo
            #region Send controller status to robo
            if (isControllerConnected)
            {
                StickStatus stickStatus = new StickStatus()
                {
                    X = leftPosition.X,
                    Y = leftPosition.Y,
                    Xr = rightPosition.X,
                    Yr = rightPosition.Y,
                    rt = rightTrigger,
                    lt = leftTrigger
                };
                byte[] data = Encoding.ASCII.GetBytes(AppData_Json.CoverObjectToJson(stickStatus) + "#");

                try
                {
                    controllerStream.BeginWrite(data, 0, data.Length, new AsyncCallback(SendCallback), controllerStream);
                }
                catch { }
            }
            #endregion

            #region Get images from robo
            if (isCamaraStreamConnected)
            {
                byte[] result = new byte[camaraStreamTCP.Available];
                try
                {
                    camaraStreamStream.BeginRead(result, 0, result.Length, new AsyncCallback(ReadCallback), camaraStreamStream);
                    string strResponse = Encoding.ASCII.GetString(result).Trim();

                    if (strResponse.Contains('#'))
                    {
                        string[] result2 = strResponse.Split('#');
                        frames[frames.Count - 1] += result2[0];
                        frames.Add(result2[result2.Length - 1]);
                    }
                    else
                    {
                        frames[frames.Count - 1] += strResponse;
                    }
                }  
                catch { }
            }
            #endregion

            if (isStatusMonitorConnected)
            {
                byte[] result = new byte[statusMonitorTCP.Available];
                try
                {
                    statusMonitorStream.BeginRead(result, 0, result.Length, new AsyncCallback(ReadCallback), statusMonitorStream);
                    string strResponse = Encoding.ASCII.GetString(result).Trim();

                    if (strResponse.Contains("#"))
                    {
                        string[] result2 = strResponse.Split("#");
                        status = "Battery Voltage:" + result2[0] + "  " + result2[1];
                    }
                }
                catch { }
            }

            statusIndicator.isEnable[0] = isCamaraStreamConnected;
            statusIndicator.isEnable[3] = isControllerConnected;
            statusIndicator.isEnable[1] = isStatusMonitorConnected;
            statusIndicator.isEnable[2] = isStatusMonitorConnected;

            base.Update(gameTime);
        }
        private void SendCallback(IAsyncResult ar)
        {
            ((NetworkStream)ar.AsyncState).EndWrite(ar);
        }
        private void ReadCallback(IAsyncResult ar)
        {
            ((NetworkStream)ar.AsyncState).EndRead(ar);
        }

    protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            //Display image from robo
            try
            {
                if (!string.IsNullOrWhiteSpace(frames[frames.Count - 2]))
                {
                    string base64 = frames[frames.Count - 2];
                    byte[] bytes = Convert.FromBase64String(base64);
                    using (MemoryStream ms2 = new MemoryStream(bytes))
                    {
                        Image mImage = Image.FromStream(ms2);
                        Bitmap bmp2 = new Bitmap(mImage);
                        camView = AppData_Json.GetTexture2DFromBitmap(GraphicsDevice, bmp2);
                        bmp2.Dispose();
                    }
                    if (frames.Count >= 5)
                        frames.RemoveAt(0);
                }
            }
            catch { }
            _spriteBatch.Draw(camView, new Microsoft.Xna.Framework.Rectangle(0, 0, 1920, 1080), Microsoft.Xna.Framework.Color.White);

            _spriteBatch.DrawString(font, status, new Vector2(5, 60), Microsoft.Xna.Framework.Color.Tomato);

            statusIndicator.Draw(_spriteBatch, new Microsoft.Xna.Framework.Rectangle(5, 5, 50, 50), myIndicator.Arrangement.Horizontal);
            operateIndicator.Draw(_spriteBatch, new Microsoft.Xna.Framework.Rectangle(5, 975, 200, 50), myIndicator.Arrangement.Vertical);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
