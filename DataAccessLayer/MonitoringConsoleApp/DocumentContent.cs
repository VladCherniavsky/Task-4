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
        public string DateContentInFile { get; set; }
        public string ClientName { get; set; }
        public string Item { get; set; }
        public string Sum { get; set;  }

        public DocumentContent( string date, string client, string item, string sum)
        {
            DateContentInFile = date;
            ClientName = client;
            Item = item;
            Sum = sum;
        }
    }
}
