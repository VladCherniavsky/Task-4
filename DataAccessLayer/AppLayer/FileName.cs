namespace AppLayer
{
    public class FileName
    {
        public string SecondNameInFileName { get; set; }
        public string DateInFileName { get; set; }

        public FileName(string name, string date)
        {
            SecondNameInFileName = name;
            DateInFileName = date;
        }

        
    }
}