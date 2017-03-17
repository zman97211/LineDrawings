using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FastImageTest
{
    class FastImage
    {
        private const int BytesPerPixel = 4;
        private readonly byte[] _buffer;
        private readonly int _width;
        private readonly int _height;
        private const int ExtraSpace = 0;// 1024 * 1024 * 256;

        public FastImage(int width, int height)
        {
            _width = width;
            _height = height;
            _buffer = new byte[_width * _height * BytesPerPixel + ExtraSpace];
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
        }
    }
}
