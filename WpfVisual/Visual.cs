using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media.Imaging;
using Controller;
using Model;
using System.Numerics;

namespace WpfVisual
{
    static class Visual
    {
        private static Dictionary<Vector2, Bitmap> Graphics { get; set; }
        private static int SectionWidth { get; set; }
        private static int SectionHeight { get; set; }
        private static int TrackWidth { get; set; }
        private static int TrackHeight { get; set; }

        public static void Initialize(Track track)
        {
            Graphics = new Dictionary<Vector2, Bitmap>();
            SectionWidth = 200;
            SectionHeight = 200;

            //DrawTrack(track);

            //Data.CurrentRace.DriversChanged += OnDriversChanged;
            //Data.NewRace += OnNewRace;

        }

        public static void OnDriversChanged(DriversChangedEventArgs e)
        {
            DrawTrack(e.Track);
        }

        public static void OnNewRace(RaceEventArgs e)
        {
            Initialize(e.Track);
        }

        public static BitmapSource DrawTrack(Track track)
        {
            LoadTrack(track);

            Bitmap bitmap = ImageHandler.GetEmptyBitmap(500,500);

            BitmapSource bitmapSource = ImageHandler.CreateBitmapSourceFromGdiBitmap(bitmap);
            return bitmapSource;
        }

        private static void LoadTrack(Track track, int startX = 0, int startY = 0, int direction = 1)
        {
            Graphics.Clear();

            int x = startX;
            int y = startY;
            int minX = startX;
            int minY = startY;
            int maxX = startX;
            int maxY = startY;

            foreach (Section section in track.Sections)
            {
                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;

                //todo change to bitmap / graphic
                Bitmap sectionGraphic = GetSectionGraphic(section.SectionType, direction);

                SectionData sectionData = Data.CurrentRace.GetSectionData(section);
                sectionGraphic = DrawParticipantsInSection(sectionGraphic, sectionData.Left, sectionData.Right);

                Graphics.Add(new Vector2(x, y), sectionGraphic);

                UpdateDirection(ref direction, section.SectionType);
                SetNextSectionCursorPositions(ref x, ref y, direction);
            }

            if (minX < startX * -1 || minY < startY * -1)
            {
                startX = minX * -1;
                startY = minY * -1;
                LoadTrack(track, startX, startY);
            }

            TrackHeight = maxY - minY;
            TrackWidth = maxX - minX;
        }

        private static Bitmap DrawParticipantsInSection(Bitmap sectionGraphic, IParticipant sectionDataLeft, IParticipant sectionDataRight)
        {
            return sectionGraphic;
        }

        private static void UpdateDirection(ref int direction, SectionTypes sectionType)
        {
            switch (sectionType)
            {
                case SectionTypes.LeftCorner when direction == 0:
                    direction = 3;
                    break;
                case SectionTypes.LeftCorner:
                    direction--;
                    break;
                case SectionTypes.RightCorner when direction == 3:
                    direction = 0;
                    break;
                case SectionTypes.RightCorner:
                    direction++;
                    break;
            }
        }
        private static void SetNextSectionCursorPositions(ref int x, ref int y, int direction)
        {
            switch (direction)
            {
                case 0:
                    y -= SectionHeight;
                    break;
                case 1:
                    x += SectionWidth;
                    break;
                case 2:
                    y += SectionHeight;
                    break;
                case 3:
                    x -= SectionWidth;
                    break;
            }
        }

        private static Bitmap GetSectionGraphic(SectionTypes sectionType, int direction = 1)
        {
            string sectionName;

            switch (sectionType)
            {
                case SectionTypes.StartGrid:
                case SectionTypes.Straight:
                    if (direction == 1 || direction == 3)
                        sectionName = "TrackHorizontal";
                    else
                        sectionName = "TrackVertical";

                    break;
                case SectionTypes.LeftCorner:
                    sectionName = direction switch
                    {
                        1 => "CornerTopLeft",
                        2 => "CornerTopRight",
                        3 => "CornerBottomRight",
                        0 => "CornerBottomLeft",
                        _ => "CornerTopLeft"
                    };
                    break;
                case SectionTypes.RightCorner:
                    sectionName = direction switch
                    {
                        1 => "CornerBottomLeft",
                        2 => "CornerTopLeft",
                        3 => "CornerTopRight",
                        0 => "CornerBottomRight",
                        _ => "CornerBottomLeft"
                    };
                    break;
                case SectionTypes.Finish:
                    if (direction == 1 || direction == 3)
                        sectionName = "FinishHorizontal";
                    else
                        sectionName = "FinishVertical";

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sectionType), sectionType, null);
            }

            Bitmap bitmap = ImageHandler.GetSectionImage(sectionName);
            return bitmap;
        }


    }
}
