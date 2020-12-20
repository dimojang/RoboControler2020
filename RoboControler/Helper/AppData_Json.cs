using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

using MonoGame.Framework;

namespace RoboControler.Helper
{
    class AppData_Json
    {
        /// <summary>
        /// 将Json反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="Json">Json</param>
        /// <returns>对象</returns>
        public static T CoverJsonToObject<T>(string Json)
        {
            return JsonConvert.DeserializeObject<T>(Json);
        }

        /// <summary>
        /// 将对象序列化为Json
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="Object">对象</param>
        /// <returns>Json</returns>
        public static string CoverObjectToJson<T>(T Object)
        {
            return JsonConvert.SerializeObject(Object, Formatting.None);
        }

        /// <summary>
        /// 将Bitmap转化为Texture2D
        /// </summary>
        /// <param name="device">渲染设备</param>
        /// <param name="bitmap">Bitmap</param>
        /// <returns></returns>
        public static Texture2D GetTexture2DFromBitmap(GraphicsDevice device, Bitmap bitmap)
        {
            Texture2D tex = new Texture2D(device, bitmap.Width, bitmap.Height, true, SurfaceFormat.Color);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int bufferSize = data.Height * data.Stride;
            byte[] bytes = new byte[bufferSize];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            tex.SetData(bytes);
            bitmap.UnlockBits(data);
            return tex;
        }
    }
}
