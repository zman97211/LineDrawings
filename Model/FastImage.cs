using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Model
{
    public class FastImage
    {
        private const int BytesPerPixel = 4;
        private readonly byte[] _buffer;
        private const int ExtraSpace = 0;

        public int Width { get; set; }
        public int Height { get; set; }

        public FastImage(int width, int height)
        {
            Width = width;
            Height = height;
            _buffer = new byte[Width * Height * BytesPerPixel];
        }

        public FastImage(Bitmap from)
        {
            Width = from.Width;
            Height = from.Height;
            _buffer = new byte[Width * Height * BytesPerPixel];
            GCHandle handle = GCHandle.Alloc(_buffer);
            try
            {
                var address = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, 0);
                var stride = BytesPerPixel * Width;
                var pixelFormat = PixelFormat.Format32bppArgb;
                using (var fastBitmap = new Bitmap(Width, Height, stride, pixelFormat, address))
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
                var stride = BytesPerPixel * Width;
                var pixelFormat = PixelFormat.Format32bppArgb;
                using (var fastBitmap = new Bitmap(Width, Height, stride, pixelFormat, address))
                {
                    fastBitmap.Save(filename);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        public void Save(MemoryStream stream, ImageFormat format)
        {
            GCHandle handle = GCHandle.Alloc(_buffer);
            try
            {
                var address = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, 0);
                var stride = BytesPerPixel * Width;
                var pixelFormat = PixelFormat.Format32bppArgb;
                using (var fastBitmap = new Bitmap(Width, Height, stride, pixelFormat, address))
                {
                    fastBitmap.Save(stream, format);
                }
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
