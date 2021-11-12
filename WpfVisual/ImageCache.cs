using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace WpfVisual
{
    static class ImageCache
    {
        private static Dictionary<string, Bitmap> _cache;

        public static void Initialize()
        {
            _cache = new Dictionary<string, Bitmap>();
        }

        public static Bitmap GetEmptyBitmap(int x, int y)
        {
            if (_cache.ContainsKey("empty"))
            {
                return (Bitmap)_cache["empty"].Clone();
            }

            Bitmap bitmap = new Bitmap(x, y);

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Green);

            _cache.Add("empty", bitmap);

            return (Bitmap)_cache["empty"].Clone();

        }

        public static Bitmap GetBitmapFromImage(string filename)
        {
            if (_cache.ContainsKey(filename))
            {
                return _cache[filename];
            }

            string filepath = $@".\graphics\{filename}";
            //string i = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Bitmap bitmap = new Bitmap(filepath);
            _cache.Add(filename, bitmap);

            return _cache[filename];
        }

        public static void EmptyCache() => _cache.Clear();

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            BitmapData bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                int size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
