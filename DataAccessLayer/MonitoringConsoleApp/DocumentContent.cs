using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringConsoleApp
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
        public DocumentContent(System.DateTime date, string client, string item, double sum)
        {
            DateContentInFile = date;
            ClientName = client;
            Item = item;
            Sum = sum;
        }
    }
}
