using System;
using System.Globalization;

namespace AppLayer
{
    public class DocumentContent
    {
        public DateTime DateContentInFile { get; set; }
        public string ClientName { get; set; }
        public string Item { get; set; }
        public double Sum { get; set;  }

        public DocumentContent( string date, string client, string item, string sum)
        {
            DateContentInFile = DateTime.ParseExact(date, "MM.dd.yyyy", CultureInfo.InvariantCulture);
            ClientName = client;
            Item = item;
            Sum = Convert.ToDouble(sum, CultureInfo.InvariantCulture);
        }


    }
}
