﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccessLayer;
using System.Configuration.Install;
using MonitoringConsoleApp;

namespace MonitoringService
{
    [RunInstaller(true)]
    public class MyWindowsServiceInstaller : Installer
    {
        public MyWindowsServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "My Service";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "My Service";

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }

        private Service1 service11;

        private void InitializeComponent()
        {
            this.service11 = new MonitoringService.Service1();
            // 
            // service11
            // 
            this.service11.ExitCode = 0;
            this.service11.ServiceName = "Service1";

        }
    }


    

    public partial class Service1 : ServiceBase
    {
        
        public Service1()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }


        protected override void OnStart(string[] args)
        {
            Dictionary<FileName,Object> dataForAdding = new Dictionary<FileName,Object>();
            const string pathes = "C:\\Users\\Влад\\Desktop\\Files";
            List<string> listOfpathes= new List<string>();
            List<FileName> namesOfFlies = new List<FileName>();

            
            FileSystemWatcher watcher = new FileSystemWatcher(pathes, "*.csv");
            watcher.EnableRaisingEvents = true;
            
            watcher.Created += GetFiles(listOfpathes, pathes);
            watcher.Created += SaveDataForAdding(listOfpathes, dataForAdding);
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Created += AddDataToDatabase(dataForAdding);
        }

        protected override void OnStop()
        {
        }

        public static void OnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            
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

        public static FileSystemEventHandler SaveDataForAdding(List<string> listOfpathes, Dictionary<FileName, Object> dataForAdding)
        {
            foreach (var nameOfFile in listOfpathes)
            {
                List<DocumentContent> holdContentOfFile = new List<DocumentContent>();
                string fileName = Path.GetFileNameWithoutExtension(nameOfFile);
                Regex re = new Regex(@"([a-zA-Z]+)");
                Regex re1 = new Regex(@"(\d+)");

                Match result = re.Match(fileName);
                Match result1 = re1.Match(fileName);
                string alphaPart = result.Groups[1].Value;
                int numberPart = Convert.ToInt32((result1.Groups[1].Value));
                var separatedNumberPart = numberPart.ToString(("##\\.##\\.####"));
                FileName nameAndDate = new FileName(alphaPart, separatedNumberPart);
              // namesOfFlies.Add(nameAndDate);

                List<Object> setOfHoldContentOfFile = new List<object>();
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
                            holdContentOfFile.Add(documentContent);
                        }
                    }
                }

                setOfHoldContentOfFile.Add(holdContentOfFile);
                dataForAdding.Add(nameAndDate, setOfHoldContentOfFile);
            }
            return null;
        }

        public FileSystemEventHandler AddDataToDatabase(Dictionary<FileName, Object> dataForAdding)
        {
            DbModelContainer db = new DbModelContainer();
            foreach (var data in dataForAdding)
            {
                db.ManagerSet.Add(new Manager(){Id = 1, ManagerName = data.Key.SecondNameInFileName});
                var item = data.Value;
            }
            return null;
        }

    }
}
