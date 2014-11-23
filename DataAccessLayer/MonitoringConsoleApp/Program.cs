using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccessLayer;

namespace MonitoringConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<FileName> listFilesNames = new List<FileName>();
            List<DocumentContent> holdContentOfFile = new List<DocumentContent>();
            const string pathes = "C:\\Users\\Влад\\Desktop\\Files";


            IEnumerable<string> filesName = System.IO.Directory.GetFiles(pathes, "*.csv");
            FileSystemWatcher watcher = new FileSystemWatcher(pathes, "*.csv");
            
            watcher.Created += Showing(filesName, listFilesNames);
            watcher.Created += ParsingDocument(filesName, holdContentOfFile);
            watcher.Created += ShowContetnOfFile(holdContentOfFile);
            watcher.Created += Showin(listFilesNames);
            
            //Showing(filesName, listFilesNames);
            //Showin(listFilesNames);
        }

        

        public static FileSystemEventHandler ShowContetnOfFile(List<DocumentContent> holdContentOfFile)
        {
            foreach (var content in holdContentOfFile)
            {
                Console.WriteLine("{0} {1} {2} {3}",content.DateContentInFile, content.ClientName, content.Item, content.Sum);
            }
            return null;
        }

        public static FileSystemEventHandler ParsingDocument(IEnumerable<string> filesName, List<DocumentContent> holdContentOfFile )
        {
            foreach (var path in filesName)
            {
                using (var streamReader = new StreamReader(path))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        if (line != null)
                        {
                            string[] separatedContent = line.Split(',');
                            DocumentContent documentContent = new DocumentContent(
                                separatedContent[0].Trim(), 
                                separatedContent[1].Trim(),
                                separatedContent[2].Trim(),
                                separatedContent[3].Trim());
                            holdContentOfFile.Add(documentContent);
                        }
                    }
                }
                
            }
            return null;
        }

        public static FileSystemEventHandler Showin(List<FileName> listFilesName)
        {
            foreach (var ob in listFilesName)
            {
                Console.WriteLine("{0}---- {1}", ob.SecondNameInFileName, ob.DateInFileName);
            }
            return null;
        }

        public static FileSystemEventHandler Showing(IEnumerable<string> array, List<FileName> listFilesName)
        {
            
            DbModelContainer dbModelContainer = new DbModelContainer();
            foreach (var file in array)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
               Regex re = new Regex(@"([a-zA-Z]+)");
               Regex re1 = new Regex(@"(\d+)");

               Match result = re.Match(fileName);
               Match result1 = re1.Match(fileName);
               string alphaPart = result.Groups[1].Value;
               int numberPart = Convert.ToInt32((result1.Groups[1].Value));
               var separatedNumberPart = numberPart.ToString(("##\\.##\\.####"));
                FileName nameAndDate = new FileName(alphaPart, separatedNumberPart);
                listFilesName.Add(nameAndDate);
            }
            return null;

        }
    }
}
