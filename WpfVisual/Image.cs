using System.Collections.Generic;
using System.Drawing;

namespace WpfVisual
{
    class Image
    {
        private Dictionary<string, Bitmap> Cache;

        public Bitmap GetSectionImage(string sectionName)
        {
            if (!Cache.ContainsKey(sectionName))
            {
                Bitmap bitmap = new Bitmap(sectionName);
                Cache.Add(sectionName, bitmap);
            }

            return Cache[sectionName];
        }

        public void EmptyCache() => Cache.Clear();
    }
}
