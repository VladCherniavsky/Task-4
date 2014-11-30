using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Globalization;
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
        private FileSystemWatcher _watcher;
        private Object _lockerForManager;
        const string SourcePath = "C:\\Users\\Влад\\Desktop\\Files";
        public void Start()
        {
           
            _taskFactory = new TaskFactory();

            _lockerForManager = new object();
             string[] sourceFileNames = Directory.GetFiles(SourcePath, "*.csv");
            foreach (var fileName in sourceFileNames)
            {
                ProcessFileAsync(fileName);
            }

            _watcher = new FileSystemWatcher(SourcePath, "*.csv");
            _watcher.Created += CreateFileHandler;
            _watcher.EnableRaisingEvents = true;
        }

        public void ProcessFileAsync(string fileName)
        {
            
            CreateFileHandler(this, new FileSystemEventArgs(WatcherChangeTypes.All, SourcePath, Path.GetFileName(fileName)));
        }

        public void ProcessFile(string fileName)
        {
            string managerName = ProcessFileName(fileName);
            Manager manager = GetOrCreateManager(managerName);
            using (var streamReader = new StreamReader(fileName))
            {
                AddInfo(streamReader, manager);
            }
        }

        public void CreateFileHandler(object sender, FileSystemEventArgs e)
        {
            _taskFactory.StartNew((object fileName) => ProcessFile(fileName as String), e.FullPath);
        }

        
        protected Manager GetOrCreateManager(string managerName)
        {
            Manager m = null;
            using (DbModelContainer dcModel = new DbModelContainer())
            {
                lock (_lockerForManager)
                {
                     m = dcModel.ManagerSet.FirstOrDefault(x => x.ManagerName == managerName);
                    if (m == null)
                    {
                        m = new Manager(){ManagerName = managerName};
                        dcModel.ManagerSet.Add(m);
                        dcModel.SaveChanges();
                    }
                }
            }
            return m;
        }

        public string ProcessFileName(string fileName)
        {
            
            string separatedfileName = Path.GetFileNameWithoutExtension(fileName);
            Regex re = new Regex(@"([a-zA-Z]+)");
            Regex re1 = new Regex(@"(\d+)");

            Match result = re.Match(separatedfileName);
            string alphaPart = result.Groups[1].Value;
            return alphaPart;
        }

        protected void AddInfo(StreamReader streamReader, Manager manager)
        {
            using (var dbModelContainer = new DbModelContainer())
            {
               
                dbModelContainer.ManagerSet.Attach(manager);
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line != null)
                    {
                        string[] separatedContent = line.Split(',').Select(x=>x.Trim()).ToArray();
                        dbModelContainer.InfoSet.Add(new Info()
                        {
                            
                            Date = DateTime.ParseExact(separatedContent[0], "MM.dd.yyyy", CultureInfo.InvariantCulture),
                            ClientName = separatedContent[1],
                            Item = separatedContent[2],
                            Sum = Convert.ToInt32(separatedContent[3], CultureInfo.InvariantCulture),
                            Manager = manager
                        });
                    }
                }
                dbModelContainer.SaveChanges();
            }
        }

        public void Stop()
        {
            _watcher.Dispose();
            _watcher = null;
            _taskFactory = null;
           // _lockerForManager = null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
