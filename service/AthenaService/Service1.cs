﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AthenaService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // log
            Program.mEventLog = new EventLog();
            Program.mEventLog.Source = "AthenaSrvLog";
            Program.mEventLog.Log = "Athena Service Log";

            // run startup
	    Program.StartUp();
        }

        protected override void OnStop()
        {

            Program.ShutDown();
        }
    }
}
