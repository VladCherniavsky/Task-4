using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppLayer;
using DataAccessLayer;

namespace MonitoringConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DataManager manager = new DataManager();
            manager.Start();
            Console.ReadKey();
        }
    }
}
