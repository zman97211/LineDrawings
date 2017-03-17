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
        private int _width;
        private int _height;

        public FastImage(int width, int height)
        {
            _width = width;
            _height = height;
            _buffer = new byte[_width * _height * BytesPerPixel + 1024 * 1024 * 256];
        }

        public FastImage(Bitmap from)
        {
            _width = from.Width;
            _height = from.Height;
            _buffer = new byte[_width * _height * BytesPerPixel + 1024 * 1024 * 256];
            GCHandle handle = GCHandle.Alloc(_buffer);
            try
            {
                var address = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, 0);
                var bitsPerPixel = 32;
                var stride = bitsPerPixel * _width;
                var pixelFormat = PixelFormat.Format32bppArgb;
                using (var fastBitmap = new Bitmap(_width, _height, stride, pixelFormat, address))
                {
                    using (var fastBitmapGraphics = Graphics.FromImage(fastBitmap))
                        fastBitmapGraphics.DrawImageUnscaled(from, 0, 0);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        public void Save(string filename)
        {
            GCHandle handle = GCHandle.Alloc(_buffer);
            try
            {
                var address = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, 0);
                var bitsPerPixel = 32;
                var stride = bitsPerPixel * _width;
                var pixelFormat = PixelFormat.Format32bppArgb;
                using (var fastBitmap = new Bitmap(_width, _height, stride, pixelFormat, address))
                {
                    fastBitmap.Save(filename);
                }
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

        public void SetPixel(int x, int y, Color color)
        {
            var i = y * BytesPerPixel + x;
            _buffer[i] = color.A;
            _buffer[i + 1] = color.R;
            _buffer[i + 2] = color.G;
            _buffer[i + 3] = color.B;
        }
    }




    class Program
    {
        private static Bitmap TestBitmap = new Bitmap(@"d:\pic.jpg");

        static void Main(string[] args)
        {

            var original = new Bitmap(@"d:\pic.jpg");
            var fast = new FastImage(original);
            var fresh = new FastImage(original.Width, original.Height);
            for (var y = 0; y < original.Height; ++y)
                for (var x = 0; x < original.Width; ++x)
                    fresh.SetPixel(x, y, fast.GetPixel(x, y));
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
