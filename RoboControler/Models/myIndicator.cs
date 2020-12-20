using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RoboControler.Models
{
    class myIndicator
    {
        public enum Arrangement
        {
            Vertical = 0,
            Horizontal = 1
        }

        public List<Texture2D> icon { get; set; } = new List<Texture2D>();

        public Texture2D mask { get; set; }

        public List<bool> isEnable { get; set; } = new List<bool>() { false };

        public void Draw(SpriteBatch SpriteBatch, Rectangle Position, Arrangement Arrangement = Arrangement.Horizontal)
        {
            int index = 0;
            foreach(Texture2D texture in icon)
            {
                Rectangle ico;
                Rectangle mas;
                switch (Arrangement)
                {
                    case Arrangement.Vertical:
                        ico = new Rectangle(Position.X, 
                            Position.Y + Position.Height * index , 
                            Position.Width, 
                            Position.Height);
                        mas = new Rectangle(Position.X + Position.Width / 2, 
                            Position.Y + Position.Height * index + Position.Height / 2, 
                            Position.Height / 2, 
                            Position.Height / 2);
                        break;
                    case Arrangement.Horizontal:
                        ico = new Rectangle(Position.X + Position.Width * index, 
                            Position.Y, 
                            Position.Width, 
                            Position.Height);
                        mas = new Rectangle(Position.X + Position.Width * index + Position.Width / 2, 
                            Position.Y + Position.Height / 2, 
                            Position.Height / 2, 
                            Position.Height / 2);
                        break;
                    default:
                        ico = new Rectangle(Position.X + Position.Width * index, 
                            Position.Y, 
                            Position.Width, 
                            Position.Height);
                        mas = new Rectangle(Position.X + Position.Width * index + Position.Width / 2,
                            Position.Y + Position.Height / 2, 
                            Position.Height / 2, 
                            Position.Height / 2);
                        break;
                }

                SpriteBatch.Draw(texture, 
                    ico,
                    Color.White);

                if (!isEnable[index])
                    SpriteBatch.Draw(mask, mas, Color.White);

                index++;
            }
        }
    }
}
