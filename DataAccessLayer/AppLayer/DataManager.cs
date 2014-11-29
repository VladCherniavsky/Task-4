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
        private TaskFactory _taskFactory;

        public void Start()
        {
            const string pathes = "C:\\Users\\Влад\\Desktop\\Files";
            _taskFactory = new TaskFactory();
            FileSystemWatcher watcher = new FileSystemWatcher(pathes, "*.csv");
            watcher.EnableRaisingEvents = true;

            watcher.Created += CreateFileHandler;
        }
        public void CreateFileHandler(object sender, FileSystemEventArgs e)
        {
            _taskFactory.StartNew((object fileName) =>
            {
                FileName namesOfFile;
                List<DocumentContent> listOfstrings;
                ProcessFile(fileName as string, out  namesOfFile, out listOfstrings);
                AddToDb( namesOfFile, listOfstrings);
            }, e.FullPath);
        }

        public void ProcessFile(string fileName, out FileName namesOfFile, out List<DocumentContent> listOfstrings)
        {
            namesOfFile = new FileName();  
            listOfstrings = new List<DocumentContent>();
            string  separatedfileName = Path.GetFileNameWithoutExtension(fileName);
            Regex re = new Regex(@"([a-zA-Z]+)");
            Regex re1 = new Regex(@"(\d+)");

            Match result = re.Match(separatedfileName);
            Match result1 = re1.Match(separatedfileName);
            string alphaPart = result.Groups[1].Value;
            int numberPart = Convert.ToInt32((result1.Groups[1].Value));
            var separatedNumberPart = numberPart.ToString(("##\\.##\\.####"));
            FileName nameAndDate = new FileName(alphaPart, separatedNumberPart);
            namesOfFile = nameAndDate;

            
            using (var streamReader = new StreamReader(fileName))
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
                        listOfstrings.Add(documentContent);
                    }
                }
            }
        }

        public void AddToDb( FileName namesOfFile, List<DocumentContent> listOfstrings)
        {
            DbModelContainer db = new DbModelContainer();
          
            db.ManagerSet.Add(new Manager() {ManagerName = namesOfFile.SecondNameInFileName});

            listOfstrings.ToList().ForEach(x => db.InfoSet.Add(new Info()
            {
                ClientName = x.ClientName,
                Date = x.DateContentInFile,
                Item = x.Item,
                Sum = Convert.ToInt32(x.Sum)
            }));
            db.SaveChanges();
        }
        
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
