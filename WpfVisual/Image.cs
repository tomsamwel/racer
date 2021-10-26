using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace WpfVisual
{
    static class Image
    {
        private static Dictionary<string, Bitmap> Cache;

        public static Bitmap GetEmptyBitmap(int x, int y)
        {
            if (Cache.ContainsKey("empty"))
            {
                return (Bitmap)Cache["empty"].Clone();
            }

            Bitmap bitmap = new Bitmap(x, y);

            // Loop through the images pixels to reset color.
            for (x = 0; x < bitmap.Width; x++)
            {
                for (y = 0; y < bitmap.Height; y++)
                {
                    Color newColor = Color.FromArgb(0, 130, 0);
                    bitmap.SetPixel(x, y, newColor);
                }
            }

            Cache.Add("empty", bitmap);

            return (Bitmap)Cache["empty"].Clone();

        }

        public static Bitmap GetSectionImage(string sectionName)
        {
            if (Cache.ContainsKey(sectionName))
            {
                return Cache[sectionName];
            }

            Bitmap bitmap = new Bitmap(sectionName);
            Cache.Add(sectionName, bitmap);

            return Cache[sectionName];
        }

        public static void EmptyCache() => Cache.Clear();

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
