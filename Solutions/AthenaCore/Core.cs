using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AthenaCore
{
    public class Core
    {

        public int modeSysMsg = 0;
        public const int modeMsgOff = 0;
        public const int modeMsgText = 1;
        public const int modeMsgMail = 2;


        public SqlDb mSqlDb;
        public string mSqlHost;
        public Groups mGroups;
        public SocketManager mSocketManager;
        public ModemManager mModemManager;

        public EventLog mEventLog;

        public bool isRunning = true;

        private string[] seperator1 = new string[] { "\r\n" };
        private string[] seperator2 = new string[] { ":" };

        private Object jobLock = new Object();

        public Core()
        {
            StartUp();
        }


        // TODO REMOVE
        /*
        public bool runDailyJob()
        {
            bool X = false;


            try
            {

                string date_ext = DateTime.Now.ToString("MM_dd_yyyy");

                FileStream fis = new FileStream(Resources.FileDir + Resources.fileJobType + date_ext, FileMode.Open);
                Console.WriteLine("Processing notices...");
                if (fis.CanRead)
                {
                    String fileStr = "";
                    int data;
                    while ((data = fis.ReadByte()) != -1)
                    {
                        fileStr += (char)data;
                    }

                    string[] cr = new string[] { "\r\n" }; // may not work if it comes from linux
                    Resources.todaysHoldNotices = fileStr.Split(cr, StringSplitOptions.None);
                    Resources.todaysHoldTexts = new ArrayList();
                    for (int i = 0; i < Resources.todaysHoldNotices.Length; i++)
                    {
                        if (Resources.todaysHoldNotices[i].Length > 0)
                        {
                            string[] pipe = new string[] { "|" };

                            Console.WriteLine(Resources.todaysHoldNotices[i]);
                            string[] txts = Resources.todaysHoldNotices[i].Split(pipe, StringSplitOptions.None);

                            Resources.todaysHoldTexts.Add(txts);

                        }
                    }

                    String[] what = { "sysmsg", "Daily Hold Texts" };
                    mModemManager.addToAndProcessOutgoingMessages(Resources.todaysHoldTexts, what);
                    X = true;
                }
                else
                {
                    doEventLog("Main.runDailyHoldNotices(): Failed to read " + Resources.FileDir + Resources.fileJobType + date_ext, 0);
                    sendMail("Athena failed to open the Daily Hold Notices file", "Program.runDailyJob(): Failed to read " + Resources.FileDir + Resources.fileJobType + date_ext);
                }

            }
            catch (Exception e)
            {
                doEventLog("Program.runDailyHoldNotices(): " + e.Message + "\n\n" + e.StackTrace, 0);
            }
            return X;
        }
        */

        private bool addFileToOutGoingMessages(string[] sa, string date_ext)
        {
            bool X = false;
            lock (jobLock)
            {
                string jName = sa[0];
                string jLoc = sa[1];
                string jFile = sa[2];

                Console.WriteLine("Running " + jName);

                try
                {
                    using (FileStream fis = new FileStream(jLoc + jFile + date_ext, FileMode.Open))
                    {

                        String fileStr = "";
                        int data;
                        while ((data = fis.ReadByte()) != -1)
                        {
                            fileStr += (char)data;
                        }
                        fis.Close();

                        string[] currentNotices = fileStr.Split(new string[] { "\n" }, StringSplitOptions.None);
                        ArrayList currentTexts = new ArrayList();
                        for (int i = 0; i < currentNotices.Length; i++)
                        {
                            if (currentNotices[i].Length > 0)
                            {
                                Console.WriteLine(currentNotices[i]);
                                string[] txts = currentNotices[i].Split(new string[] { " | " }, StringSplitOptions.None);
                                currentTexts.Add(txts);
                            }
                        }

                        String[] what = { "sysmsg", jName };
                        X = mModemManager.addToAndProcessOutgoingMessages(currentTexts, what);

                    }
                }
                catch (Exception e)
                {
                    doEventLog("Program.addFileToOutgoingMessages(): " + e.Message + "\n\n" + e.StackTrace, 0);
                    doNotify("Athena failed to process " + jName, "Error processing " + jLoc + jFile + date_ext);
                }
            }
            return X;
        }

        public bool runSingleJob(string name)
        {
            bool X = false;
            ArrayList list = mSqlDb.GetJobInfo(name);
            string date_ext = "";
            int sch = -1;
            if (list.Count.Equals(4))
            {
                Int32.TryParse((string)list[3], out sch);
                if (sch >= 0)
                {
                    switch (sch)
                    {
                        case 3:
                            date_ext = "-" + DateTime.Now.ToString("MM");
                            break;
                        case 2:
                            date_ext = "-" + DateTime.Now.ToString("MM_dd_yyyy");
                            break;
                        case 1:
                            date_ext = "-" + DateTime.Now.ToString("MM_dd_yyyy");
                            break;
                    }
                    string[] job = { (string)list[0], (string)list[1], (string)list[2] };
                    new Thread(new ThreadStart(
                        delegate()
                        {
                            X = addFileToOutGoingMessages(job, date_ext);
                            Thread.Sleep(10000);
                            mSqlDb.MoveBadEntries();
                        }
                      )).Start();
                }
            }
            return X;
        }

        public bool runScheduledJobs(int s)
        {
            bool X = false;
            ArrayList list = mSqlDb.GetScheduledJobs(s);
            if (list.Count > 0)
            {
                new Thread(new ThreadStart(
                        delegate()
                        {
                            if (list.Count > 0)
                            {
                                foreach (string[] sa in list)
                                {
                                    X = addFileToOutGoingMessages(sa, "-" + DateTime.Now.ToString("MM_dd_yyyy"));
                                    if (!X)
                                    {
                                        break;
                                    }
                                    Thread.Sleep(10000);
                                }
                                if (X)
                                {
                                    mSqlDb.MoveBadEntries();
                                }
                            }
                        }
                      )).Start();
            }
            return X;
        }



        public void readConfigFile()
        {
            Console.WriteLine("readConfigFile()");
            try
            {
                Console.WriteLine("try");
                StreamReader cfg = new StreamReader(Resources.AthenaDir + "conf\\athenasms.conf");
                string cs = cfg.ReadToEnd();
                cfg.Close();
                //cfg.Dispose();
                string[] configInfo = cs.Split(Resources.seperator1, StringSplitOptions.None) ;
                
                
                if(configInfo.Length < 1)
                {
                    Console.WriteLine("creating config file");
                    createConfigFile();
                    cfg = new StreamReader(Resources.AthenaDir + "conf\\athenasms.conf");
                    cs = cfg.ReadToEnd();
                    Console.WriteLine("contents: " + cs);
                    cfg.Close();
                    //cfg.Dispose();
                    configInfo = cs.Split(Resources.seperator1, StringSplitOptions.None);
                }
                
                Console.WriteLine("starting config loop");

                foreach (string row in configInfo)
                {
                    //Console.WriteLine("looping thru configInfo");
                    if (row.Length > 0 && !row.StartsWith("//"))
                    {
                        Console.WriteLine("row: " + row);
                        string[] tmp = row.Split(Resources.seperator2, StringSplitOptions.None);
                        //Console.WriteLine(tmp[0] + ":" + tmp[1]);
                        if (tmp[0].Equals("APPTITLE") && tmp[1].Length > 0)
                        {
                            Resources.appTitle = tmp[1];
                        }
                        else if (tmp[0].Equals("COMPANY") && tmp.Length > 1)
                        {
                            Resources.companyName = tmp[1].Trim();
                        }
                        else if (tmp[0].Equals("EMAILHOST") && tmp.Length > 1)
                        {
                            Resources.emailHost = tmp[1].Trim();
                        }
                        else if (tmp[0].Equals("EMAIL_SENDER") && tmp.Length > 1)
                        {
                            Resources.emailSender = tmp[1].Trim();
                        }
                        else if (tmp[0].Equals("EMAIL_RECIPIENT") && tmp.Length > 1)
                        {
                            Resources.emailRecipient = tmp[1].Trim();
                        }
                        else if (tmp[0].Equals("EMAIL_HEADER") && tmp.Length > 1)
                        {
                            Resources.mailHeader = tmp[1].Trim();
                        }
                        else if (tmp[0].Equals("MODEM") && tmp.Length > 1)
                        {
                            Console.WriteLine("MODEM:" + tmp[1].Trim());
                            mModemManager.addModem(tmp[1].Trim());
                        }
                        else if (tmp[0].Equals("SQL_HOST") && tmp.Length > 1)
                        {
                            Resources.mSqlHost = tmp[1].Trim();
                        }
                        else if (tmp[0].Equals("SQL_USER") && tmp.Length > 1)
                        {
                            Resources.mSqlUser = tmp[1].Trim();
                        }
                        else if (tmp[0].Equals("SQL_PASS") && tmp.Length > 1)
                        {
                            Resources.mSqlPass = tmp[1].Trim();
                        }
                    }
                }
                Console.WriteLine("done with config loop.");
            }
            catch (Exception e)
            {
                doEventLog("Main.readConfigFile(): " + e.Message, 1);
                createConfigFile();
                Console.WriteLine("Main.readConfigFile(): " + e.Message);
            }

            Console.WriteLine("end of readConfigFile");
        }

        public void deleteConfigFile()
        {
            File.Delete(Resources.AthenaDir + "conf\\");
        }

        public void createConfigFile()
        {
            try
            {
                using (StreamWriter cfg = new StreamWriter(Resources.AthenaDir + @"conf\\athenasms.conf", true))
                {
                    cfg.Write("//Athena Config File\r\n\r\n// Application Title\r\nAPPTITLE:Athena Sms Server\r\n\r\n//SQL HOST\r\nSQL_HOST:127.0.0.1\\SQLEXPRESS\r\nSQL_USER:sa\r\nSQL_PASS:\r\n\r\n// Company Name\r\nCOMPANY:My Company\r\n\r\n//Email Host\r\nEMAILHOST:\r\n\r\n//Email Users\r\nEMAIL_SENDER:\r\n\r\nEMAIL_RECIPIENT:\r\n\r\n//Email Header\r\nEMAIL_HEADER:your app name\r\n\r\n//Modems\r\n// dummy modem\r\nMODEM:DMY0\r\n//MODEM:COM?\r\n\r\n");
                    cfg.Close();
                }
                //readConfigFile();
            }
            catch (Exception e)
            {
                doEventLog("Main.createConfigFile(): " + e.Message, 0);
                ShutDown();
            }
        }

        // LOGGING

        public void doEventLog(String msg, int l)
        {
            try
            {
                switch (l)
                {
                    case 2:
                        mEventLog.WriteEntry(msg, EventLogEntryType.Information);
                        break;
                    case 1:
                        mEventLog.WriteEntry(msg, EventLogEntryType.Warning);
                        break;
                    default:
                        mEventLog.WriteEntry(msg, EventLogEntryType.Error);
                        break;
                }
            }
            catch (Exception e)
            {
                doLogFile(e.Message, 0);
            }
            
        }
        public void doLogFile(String msg, int l)
        {
            switch (l)
            {
                case 1:
                    //send email
                    //sendMail("Athena Logging", msg);
                    break;
            }
            try
            {
                StreamWriter mLogFile = new StreamWriter(Resources.AthenaDir + @"log\info_log", true);
                mLogFile.WriteLine(msg + "\r\n");
                mLogFile.Close();
            }
            catch (Exception e)
            {
                doLogFile(e.Message, 0);
            }
        }




        public void doMail(String sbj, String msg)
        {
            if (Resources.emailHost != null && Resources.emailSender != null && Resources.emailRecipient != null)
            {
                try
                {
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                    message.To.Add(Resources.emailRecipient);
                    message.Subject = sbj;
                    message.From = new System.Net.Mail.MailAddress(Resources.emailSender);
                    message.Body = msg;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Resources.emailHost);
                    smtp.Send(message);
                }
                catch (Exception e)
                {
                    doLogFile(e.Message + "\n\n" + e.StackTrace, 0);
                }
            }
        }

        public void sendPrintJob(String h, String m)
        {

            // printer (not implemented yet)
            // setup default printer
        }

        public void doEmail(String sbj, String msg)
        {

            if (Resources.emailHost != null && Resources.emailSender != null && Resources.emailRecipient != null)
            {
                try
                {
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                    message.To.Add(Resources.emailRecipient);
                    message.Subject = sbj;
                    message.From = new System.Net.Mail.MailAddress(Resources.emailSender);
                    message.Body = msg;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Resources.emailHost);
                    smtp.Send(message);
                }
                catch (Exception e)
                {
                    doEventLog(e.Message + "\n\n" + e.StackTrace, 0);
                }
            }
        }

        public void doNotify(String sbj, String msg)
        {
            switch (modeSysMsg)
            {
                case modeMsgMail:
                    doEmail(sbj, msg);
                    break;

                case modeMsgText:
                    ArrayList cg = mSqlDb.getContactGroup("Notify");
                    if (cg != null)
                    {
                        foreach (String[] s in cg)
                        {
                            mModemManager.sendSmsToModem(s[1], sbj + "\r\n\r\n" + msg);
                        }
                    }
                    break;

                default:
                    // do nothing, turned off
                    break;
            }
        }







        public void StartUp()
        {

            //modemmanager must start before readconfig
            mModemManager = new ModemManager(this);

            // read config file
            readConfigFile();

            //after config
            mSocketManager = new SocketManager(this);
            mSqlDb = new SqlDb(this);
            mGroups = new Groups(this);
            doEventLog("Athena Service Started", 2);

        }


        public void ShutDown()
        {
            if (mModemManager != null)
            {
                mModemManager.ShutDown();
            }
            if (mSocketManager != null)
            {
                mSocketManager.ShutDown();
            }
            if (mSqlDb != null)
            {
                mSqlDb.Disconnect();
                mSqlDb = null;
            }
            doEventLog("Athena Service Stopped", 2);
        }

    }
}
