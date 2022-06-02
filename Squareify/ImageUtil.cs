using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Squareify
{
    class ImageUtil
    {
        /// <summary>
        /// Color indices
        /// </summary>
        public const int R = 0, G = 1, B = 2;

        /// <summary>
        /// Converts a RGB byte array to a bitmap
        /// </summary>
        /// <param name="data">Source array</param>
        /// <returns>Bitmap</returns>
        public static Bitmap RgbToBitmap(byte[][][] data)
        {
            int h = data[0].Length;
            int w = data[0][0].Length;

            Bitmap bmp = new Bitmap(w, h);
            BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytes = bmpdata.Stride * bmpdata.Height;
            byte[] buffer = new byte[bytes];

            int index = 0;

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    buffer[index] = data[B][i][j];
                    buffer[index + 1] = data[G][i][j];
                    buffer[index + 2] = data[R][i][j];
                    buffer[index + 3] = 255;
                    index += 4;
                }
            }

            Marshal.Copy(buffer, 0, bmpdata.Scan0, bytes);
            bmp.UnlockBits(bmpdata);

            return bmp;
        }

        /// <summary>
        /// Converts a bitmap to an RGB byte array
        /// </summary>
        /// <param name="bitmap">Source image</param>
        /// <returns>RGB byte array</returns>
        public static byte[][][] BitmapToRgb(Bitmap bitmap)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;
            byte[][][] rgb = new byte[3][][];

            rgb[R] = new byte[h][];
            rgb[G] = new byte[h][];
            rgb[B] = new byte[h][];

            Parallel.For(0, h, i =>
            {
                rgb[R][i] = new byte[w];
                rgb[G][i] = new byte[w];
                rgb[B][i] = new byte[w];
            });

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var c = bitmap.GetPixel(j, i);
                    rgb[R][i][j] = c.R;
                    rgb[G][i][j] = c.G;
                    rgb[B][i][j] = c.B;
                }
            }

            return rgb;
        }
    }
}
