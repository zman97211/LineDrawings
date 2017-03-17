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
    class Program
    {
        private static Bitmap TestBitmap = new Bitmap(@"d:\pic.jpg");

        static void Main(string[] args)
        {
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
        }
    }
}
