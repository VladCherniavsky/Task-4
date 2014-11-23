using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringConsoleApp
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

        //List<FileName> _dateAndNameOfFileNames = new List<FileName>();
    }
}