/*
 * Athena SMS Windows Console Server. Released under GNU GPL. Read LICENSE file.
 * 
 * Main Program file. Start here. Main() is at bottom of file
 * 
 * 
 */




using AthenaService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AthenaService
{
    class Program
    {

        // sys msg mode
        public static int modeSysMsg = 0;
        public const int modeMsgOff = 0;
        public const int modeMsgText = 1;
        public const int modeMsgMail = 2;

        // read from config file
        public static String appTitle = null;
        public static String companyName = null;
        public static String emailHost = null;
        public static String emailSender = null;
        public static String emailRecipient = null;
        public static String mailHeader = null;

        public static SqlDb mSqlDb;
        public static String mSqlHost;

        public static Groups mGroups;

        public static SocketManager mSocketManager;

        public static ModemManager mModemManager;

        public static String AthenaDir = @"C:\AthenaService\";



        public static EventLog mEventLog;

        public static bool isRunning = true;

        private static string[] seperator1 = new string[] { "\r\n" };
        private static string[] seperator2 = new string[] { ":" };

        private static Object jobLock = new Object();







        private static bool addFileToOutGoingMessages(string[] sa, string date_ext)
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

        public static bool runSingleJob(string name)
        {
            bool X = false;
            ArrayList list = Program.mSqlDb.GetJobInfo(name);
            string date_ext = "";
            int sch = -1;
            if (list.Count.Equals(4))
            {
                Int32.TryParse((string)list[3], out sch);
                if(sch >= 0)
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
                    string[] job = {(string)list[0], (string)list[1], (string)list[2] };
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


        public static bool runScheduledJobs(int s)
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





        // LOGGING

        public static void doEventLog(String msg, int l)
        {
            try
            {
                Console.WriteLine(msg + ", " + l); // DIFFERENT THAN SERVICE VERSION
            }
            catch (Exception e)
            {
                //doLogFile(e.Message, 0);
            }
        }

        public static void doNotify(String sbj, String msg)
        {
            switch (modeSysMsg)
            {
                case modeMsgMail:
                    doEmail(sbj, msg);
                    break;
                    
                case modeMsgText:
                    ArrayList cg = Program.mSqlDb.getContactGroup("Notify");
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

        public static void doEmail(String sbj, String msg)
        {
            /* enable on server
             * 
            if (emailHost != null && emailSender != null && emailRecipient != null)
            {
                try
                {
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                    message.To.Add(emailRecipient);
                    message.Subject = sbj;
                    message.From = new System.Net.Mail.MailAddress(emailSender);
                    message.Body = msg;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(emailHost);
                    smtp.Send(message);
                }
                catch (Exception e)
                {
                    doEventLog(e.Message + "\n\n" + e.StackTrace, 0);
                }
            }
             */
        }

        public static void sendPrintJob(String h, String m)
        {

            // printer (not implemented yet)
            // setup default printer
        }



        //CONFIG

        public static void readConfigFile()
        {
                try
                {
                    string cs;
                    using (StreamReader cfg = new StreamReader(AthenaDir + @"conf\athenasms.conf"))
                    {
                        cs = cfg.ReadToEnd();
                        cfg.Close();
                    }
                    string[] configInfo = cs.Split(seperator1, StringSplitOptions.None);

                    for (int i = 0; i < configInfo.Length; i++)
                    {
                        if (configInfo[i].Length > 0 && !configInfo[i].StartsWith("//"))
                        {
                            string[] tmp = configInfo[i].Split(seperator2, StringSplitOptions.None);
                            if (tmp[0].Equals("APPTITLE") && tmp[1].Length > 0)
                            {
                                appTitle = tmp[1];
                            }
                            else if (tmp[0].Equals("COMPANY") && tmp.Length > 1)
                            {
                                companyName = tmp[1].Trim();
                            }
                            else if (tmp[0].Equals("EMAILHOST") && tmp.Length > 1)
                            {
                                emailHost = tmp[1].Trim();
                            }
                            else if (tmp[0].Equals("EMAIL_SENDER") && tmp.Length > 1)
                            {
                                emailSender = tmp[1].Trim();
                            }
                            else if (tmp[0].Equals("EMAIL_RECIPIENT") && tmp.Length > 1)
                            {
                                emailRecipient = tmp[1].Trim();
                            }
                            else if (tmp[0].Equals("EMAIL_HEADER") && tmp.Length > 1)
                            {
                                mailHeader = tmp[1].Trim();
                            }
                            else if (tmp[0].Equals("MODEM") && tmp.Length > 1)
                            {
                                mModemManager.addModem(tmp[1].Trim());
                            }
                            else if (tmp[0].Equals("SQL_HOST") && tmp.Length > 1)
                            {
                                mSqlHost = tmp[1].Trim();
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    doEventLog("Main.readConfigFile(): " + e.Message, 1);
                    createConfigFile();
                }
            
        }

        public static void deleteConfigFile()
        {
            File.Delete(AthenaDir + "conf\\");
        }

        public static void createConfigFile()
        {
            try
            {
                using (StreamWriter cfg = new StreamWriter(AthenaDir + @"conf\\athenasms.conf", true))
                {
                    cfg.Write("//Athena Config File\r\n\r\n// Application Title\r\nAPPTITLE:Athena Sms Server\r\n\r\n//SQL HOST\r\nSQL_HOST:127.0.0.1\\ATHENASQL\r\n\r\n// Company Name\r\nCOMPANY:My Company\r\n\r\n//Email Host\r\nEMAILHOST:\r\n\r\n//Email Users\r\nEMAIL_SENDER:\r\n\r\nEMAIL_RECIPIENT:\r\n\r\n//Email Header\r\nEMAIL_HEADER:your app name\r\n\r\n//Modems\r\n//MODEM:COM?\r\n\r\n");
                    cfg.Close();
                }
                readConfigFile();
            }
            catch (Exception e)
            {
                doEventLog("Main.createConfigFile(): " + e.Message, 0);
                ShutDown();
            }
        }

        public static void ShutDown()
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

        public static void StartUp()
        {

            //modemmanager must start before readconfig
            mModemManager = new ModemManager();

            // read config file
            readConfigFile();

            //after config
            mSocketManager = new SocketManager();
            mSqlDb = new SqlDb();
            mGroups = new Groups();
            doEventLog("Athena Service Started", 2);

        }

        static void Main(string[] args)
        {
            // add service start for service version

            StartUp();
        }
    }
}
