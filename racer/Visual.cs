using System;
using System.Collections.Generic;
using System.Numerics;
using Controller;
using Model;

namespace View
{
    public static class Visual
    {
        private static Dictionary<Vector2, string[]> Graphics { get; set; }
        private static int SectionWidth { get; set; }
        private static int SectionHeight { get; set; }


        public static void Initialize(Track track)
        {
            Graphics = new Dictionary<Vector2, string[]>();
            SectionWidth = TrackHorizontal[0].Length;
            SectionHeight = TrackHorizontal.Length;

            DrawTrack(track);
            
            Data.CurrentRace.DriversChanged += OnDriversChanged;
            Data.NewRace += OnNewRace;
        }

        public static void OnDriversChanged(DriversChangedEventArgs e)
        {
            DrawTrack(e.Track);
        }

        public static void OnNewRace(RaceEventArgs e)
        {
            Initialize(e.Track);
        }

        private static void DrawTrack(Track track)
        {
            LoadTrack(track);
            DrawGraphics();
        }

        // int direction: 0->north, 1->east, 2->south, 3->west
        private static void LoadTrack(Track track, int startX = 0, int startY = 0, int direction = 1)
        {
            Console.Clear();
            Graphics.Clear();

            int x = startX;
            int y = startY;
            int minX = startX;
            int minY = startY;

            foreach (Section section in track.Sections)
            {
                if (x < minX) minX = x;
                if (y < minY) minY = y;

                string[] sectionGraphic = GetSectionGraphic(section.SectionType, direction);

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
        }

        private static void DrawGraphics()
        {
            foreach ((Vector2 positions, string[] sectionGraphic) in Graphics)
                DrawSection(sectionGraphic, (int) positions.X, (int) positions.Y);
        }

        private static void DrawSection(string[] sectionGraphic, int x = 0, int y = 0)
        {
            for (int i = 0; i < sectionGraphic.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(sectionGraphic[i]);
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

        private static string[] GetSectionGraphic(SectionTypes sectionType, int direction = 1)
        {
            string[] section;

            switch (sectionType)
            {
                case SectionTypes.StartGrid:
                case SectionTypes.Straight:
                    if (direction == 1 || direction == 3)
                        section = TrackHorizontal;
                    else
                        section = TrackVertical;

                    break;
                case SectionTypes.LeftCorner:
                    section = direction switch
                    {
                        1 => CornerTopLeft,
                        2 => CornerTopRight,
                        3 => CornerBottomRight,
                        0 => CornerBottomLeft,
                        _ => CornerTopLeft
                    };
                    break;
                case SectionTypes.RightCorner:
                    section = direction switch
                    {
                        1 => CornerBottomLeft,
                        2 => CornerTopLeft,
                        3 => CornerTopRight,
                        0 => CornerBottomRight,
                        _ => CornerBottomLeft
                    };
                    break;
                case SectionTypes.Finish:
                    if (direction == 1 || direction == 3)
                        section = FinishHorizontal;
                    else
                        section = FinishVertical;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sectionType), sectionType, null);
            }

            string[] result = (string[]) section.Clone();
            return result;
        }

        private static string[] DrawParticipantsInSection(string[] sectionGraphic, IParticipant participant1 = null,
            IParticipant participant2 = null)
        {
            string player1Visual = participant1 == null ? " " : GenPlayerGraphic(participant1);
            string player2Visual = participant2 == null ? " " : GenPlayerGraphic(participant2);

            for (int i = 0; i < sectionGraphic.Length; i++)
            {
                sectionGraphic[i] = sectionGraphic[i].Replace("1", player1Visual);
                sectionGraphic[i] = sectionGraphic[i].Replace("2", player2Visual);
            }

            return sectionGraphic;
        }

        private static string GenPlayerGraphic(IParticipant participant)
        {
            string playerGraphic;
            if (participant.Equipment.IsBroken)
            {
                playerGraphic = "*";
            }
            else
            {
                playerGraphic = string.IsNullOrEmpty(participant.Name) ? Racer[0] : participant.Name[..1];
            }
            return playerGraphic;
        }


        #region graphics

        private static readonly string[] Racer = {"۞", "۝", "࿇", "ᐁ", "ᐃ", "ᐅ", "ᐊ"};

        private static readonly string[] FinishHorizontal =
        {
            "════════",
            "   1  # ",
            "  2   # ",
            "════════"
        };

        private static readonly string[] FinishVertical =
        {
            "║      ║",
            "║######║",
            "║ 1    ║",
            "║    2 ║"
        };

        private static readonly string[] TrackHorizontal =
        {
            "════════",
            "    1   ",
            "   2    ",
            "════════"
        };

        private static readonly string[] TrackVertical =
        {
            "║      ║",
            "║      ║",
            "║ 1  2 ║",
            "║      ║"
        };

        private static readonly string[] CornerTopLeft =
        {
            "╝      ║",
            "  1    ║",
            "    2  ║",
            "═══════╝"
        };

        private static readonly string[] CornerTopRight =
        {
            "║      ╚",
            "║    1  ",
            "║  2    ",
            "╚═══════"
        };

        private static readonly string[] CornerBottomLeft =
        {
            "═══════╗",
            "    2  ║",
            "  1    ║",
            "╗      ║"
        };

        private static readonly string[] CornerBottomRight =
        {
            "╔═══════",
            "║  2    ",
            "║    1  ",
            "║      ╔"
        };

        #endregion
    }
}