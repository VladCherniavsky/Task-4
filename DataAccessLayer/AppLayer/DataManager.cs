using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccessLayer;

namespace AppLayer
{
    public class DataManager: IDisposable
    {
        public void Start()
        {
            const string pathes = "C:\\Users\\Влад\\Desktop\\Files";
            List<string> listOfpathes = new List<string>();
            List<FileName> namesOfFlies = new List<FileName>();
            List<List<DocumentContent>> setOfDocs = new List<List<DocumentContent>>();


            FileSystemWatcher watcher = new FileSystemWatcher(pathes, "*.csv");
            watcher.EnableRaisingEvents = true;

            watcher.Created += GetFiles(listOfpathes, pathes);
            watcher.Created += SaveDataForAdding(listOfpathes, namesOfFlies, setOfDocs);
            watcher.Created += AddDataToDatabase(namesOfFlies, setOfDocs);
        }

        public static FileSystemEventHandler GetFiles(List<string> listOfpathes, string pathes)
        {
            string[] listOfpath = System.IO.Directory.GetFiles(pathes, "*.csv");
            for (int i = 0; i < listOfpath.Length; i++)
            {
                listOfpathes.Add(listOfpath[i]);
            }
            return null;
        }

        public static FileSystemEventHandler SaveDataForAdding(List<string> listOfpathes,
           List<FileName> namesOfFlies, List<List<DocumentContent>> contentList)
        {
            //var st = Stopwatch.StartNew();
            foreach (var nameOfFile in listOfpathes)
            {
                List<DocumentContent> holdAllStringsOfFile = new List<DocumentContent>();
                string fileName = Path.GetFileNameWithoutExtension(nameOfFile);
                Regex re = new Regex(@"([a-zA-Z]+)");
                Regex re1 = new Regex(@"(\d+)");

                Match result = re.Match(fileName);
                Match result1 = re1.Match(fileName);
                string alphaPart = result.Groups[1].Value;
                int numberPart = Convert.ToInt32((result1.Groups[1].Value));
                var separatedNumberPart = numberPart.ToString(("##\\.##\\.####"));
                FileName nameAndDate = new FileName(alphaPart, separatedNumberPart);
                namesOfFlies.Add(nameAndDate);


                using (var streamReader = new StreamReader(nameOfFile))
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
                            holdAllStringsOfFile.Add(documentContent);
                        }
                    }
                }

                contentList.Add(holdAllStringsOfFile);

            }
            //st.Stop();
            //double secSpan = st.Elapsed.TotalSeconds;
            return null;
        }
        public static FileSystemEventHandler AddDataToDatabase(List<FileName> namesOfFlies,
            List<List<DocumentContent>> setOfDocs)
        {
            DbModelContainer db = new DbModelContainer();
            var st = Stopwatch.StartNew();
            for (int i = 0; i < namesOfFlies.Count; i++)
            {
                Manager manager = new Manager();
                var fileName = namesOfFlies.ElementAt(i);
                manager.ManagerName = fileName.SecondNameInFileName;

                var oneDoc = setOfDocs.ElementAt(i);

                foreach (var oneLine in oneDoc)
                {

                    Info infos = new Info();
                    infos.ClientName = oneLine.ClientName;
                    infos.Date = oneLine.DateContentInFile;
                    infos.Item = oneLine.Item;
                    infos.Sum = Convert.ToInt32(oneLine.Sum);

                    db.InfoSet.Add(infos);
                }
                db.ManagerSet.Add(manager);
                db.SaveChanges();
            }
            st.Stop();
            double secSpan = st.Elapsed.TotalSeconds;
            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
