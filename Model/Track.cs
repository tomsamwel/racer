using System.Collections.Generic;

namespace Model
{
    public class Track
    {
        public Track(string name, SectionTypes[] sections)
        {
            Name = name;
            Sections = new LinkedList<Section>();

            Sections = SectionTypesToLinkedList(sections);
        }

        public string Name { get; set; }
        public LinkedList<Section> Sections { get; set; }

        private static LinkedList<Section> SectionTypesToLinkedList(SectionTypes[] sections)
        {
            LinkedList<Section> linkedList = new LinkedList<Section>();
            foreach (SectionTypes section in sections) linkedList.AddLast(new Section(section));

            return linkedList;
        }
    }
}