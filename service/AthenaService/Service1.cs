using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using AthenaCore;
using System.Diagnostics;

namespace AthenaService
{
    public partial class Service1 : ServiceBase
    {

        private Core SmsCore;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // log
            //SmsCore.mEventLog = new EventLog();
            //SmsCore.mEventLog.Source = "AthenaSrvLog";
            //SmsCore.mEventLog.Log = "Athena Service Log";

            SmsCore = new Core();
        }

        protected override void OnStop()
        {

            SmsCore.ShutDown();
            SmsCore = null;
        }
    }
}
