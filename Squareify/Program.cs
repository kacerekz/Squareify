using System;
using System.IO;
using System.Drawing;

namespace Squareify
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) goto exit;
            if (!Directory.Exists(args[0])) goto exit;

            var dir = args[0];
            var files = Directory.EnumerateFiles(dir);

            foreach (var file in files)
            {
                var b = new Bitmap(file);
                var brgb = ImageUtil.BitmapToRgb(b);
                int size = Math.Max(b.Width, b.Height);
                Color avgcol = AvgColor(brgb);

                var nrgb = MakeImage(size, avgcol);
                nrgb = FillImage(nrgb, brgb);
                var nb = ImageUtil.RgbToBitmap(nrgb);
                b.Dispose();
                File.Delete(file);
                nb.Save(file);
            }
            return;

        exit:
            Console.WriteLine("Launch with path to directory.");
            return;
        }

        private static byte[][][] FillImage(byte[][][] nrgb, byte[][][] brgb)
        {
            var brgbA = brgb[0].Length;
            var brgbB = brgb[0][0].Length;
            var size = nrgb[0].Length;

            var aoff = size - brgbA;
            var boff = size - brgbB;
            aoff /= 2;
            boff /= 2;

            for (int i = 0; i < brgbA; i++)
            {
                for (int j = 0; j < brgbB; j++)
                {
                    nrgb[0][aoff+i][boff+j] = brgb[0][i][j];
                    nrgb[1][aoff+i][boff+j] = brgb[1][i][j];
                    nrgb[2][aoff+i][boff+j] = brgb[2][i][j];
                }
            }

            return nrgb;
        }

        private static Color AvgColor(byte[][][] brgb)
        {
            int avgr = 0;
            int avgg = 0;
            int avgb = 0;
            int n = 0;

            for (int i = 0; i < brgb[0].Length; i++)
                for (int j = 0; j < brgb[0][0].Length; j++)
                {
                    avgr += brgb[0][i][j];
                    avgg += brgb[1][i][j];
                    avgb += brgb[2][i][j];
                    n++;
                }

            avgr /= n;
            avgg /= n;
            avgb /= n;

            avgr = Math.Min(avgr, 255);
            avgg = Math.Min(avgg, 255);
            avgb = Math.Min(avgb, 255);

            avgr = Math.Max(avgr, 0);
            avgg = Math.Max(avgg, 0);
            avgb = Math.Max(avgb, 0);

            return Color.FromArgb(avgr, avgg, avgb);
        }

        private static byte[][][] MakeImage(int size, Color avgcol)
        {
            byte[][][] n = new byte[3][][];

            n[0] = new byte[size][];
            n[1] = new byte[size][];
            n[2] = new byte[size][];


            for (int i = 0; i < size; i++)
            {
                n[0][i] = new byte[size];
                n[1][i] = new byte[size];
                n[2][i] = new byte[size];
            }

            for (int i = 0; i < size; i++) 
            {
                for (int j = 0; j < size; j++)
                {
                    n[0][i][j] = avgcol.R;
                    n[1][i][j] = avgcol.G;
                    n[2][i][j] = avgcol.B;
                }
            }
            
            return n;
        }
    }
}
