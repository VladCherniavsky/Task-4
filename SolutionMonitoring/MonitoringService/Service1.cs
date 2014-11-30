using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppLayer;
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

        private Service1 _service11;

        private void InitializeComponent()
        {
            this._service11 = new MonitoringService.Service1();
            // 
            // service11
            // 
            this._service11.ExitCode = 0;
            this._service11.ServiceName = "Service1";

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




        DataManager manager = new DataManager();
        protected override void OnStart(string[] args)
        {
           
            manager.Start();
        }
        

        protected override void OnStop()
        {
            manager.Stop();
        }

       
    }
}
