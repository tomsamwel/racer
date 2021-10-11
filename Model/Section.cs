namespace Model
{
    public class Section
    {
        public Section(SectionTypes sectionType)
        {
            SectionType = sectionType;
        }

        public SectionTypes SectionType { get; set; }
    }
}