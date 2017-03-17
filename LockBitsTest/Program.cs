using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LockBitsTest
{




    class FastImage
    {
        private const int BytesPerPixel = 4;
        private readonly byte[] _buffer;

        public FastImage(Bitmap from)
        {
            _buffer = new byte[from.Width * from.Height * BytesPerPixel + 1024 * 1024 * 256];
            GCHandle handle = GCHandle.Alloc(_buffer);
            try
            {
                var address = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, 0);
                var bitsPerPixel = 32;
                var stride = bitsPerPixel * from.Width;
                var pixelFormat = PixelFormat.Format32bppArgb;
                using (var fastBitmap = new Bitmap(from.Width, from.Height, stride, pixelFormat, address))
                {
                    using (var fastBitmapGraphics = Graphics.FromImage(fastBitmap))
                        fastBitmapGraphics.DrawImageUnscaled(from, 0, 0);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                handle.Free();
            }
        }

        public Color GetPixel(int x, int y)
        {
            var i = y * BytesPerPixel + x;
            return Color.FromArgb(_buffer[i], _buffer[i + 1], _buffer[i + 2], _buffer[i + 3]);
        }
    }




    class Program
    {
        private static Bitmap TestBitmap = new Bitmap(@"d:\pic.jpg");

        static void Main(string[] args)
        {

            var original = new Bitmap(@"d:\pic.jpg");
            var fast = new FastImage(original);
            var fresh = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            for (var y = 0; y < original.Height; ++y)
                for (var x = 0; x < original.Width; ++x)
                    fresh.SetPixel(x, y, original.GetPixel(x, y));
            fresh.Save(@"d:\pic_copy.jpg");


#if false
            byte[] buffer = new byte[16777216];
            GCHandle handle = GCHandle.Alloc(buffer);
            try
            {
                var address = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                var width = 256;
                var height = 256;
                var bitsPerPixel = 32;
                var stride = bitsPerPixel * width;
                var pixelFormat = PixelFormat.Format32bppArgb;
                using (var fastBitmap = new Bitmap(width, height, stride, pixelFormat, address))
                {
                    using (var fastBitmapGraphics = Graphics.FromImage(fastBitmap))
                        fastBitmapGraphics.DrawImageUnscaled(TestBitmap, 0, 0);
                    fastBitmap.Save(@"d:\pic_copy.jpg");
                }
            }
            finally
            {
                handle.Free();
            }
#endif
        }
    }
}
