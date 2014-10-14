using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AthenaService
{
    static class Program
    {
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

        public static String AthenaDir = @"C:\Athena\";
        public static bool notifyOnStartup = false;

        
        public static String FileDir = AthenaDir + @"Sirsi\";
        public static String holdNoticeFile = "daily_smstext_hold_notification-";
        public static String[] todaysHoldNotices;
        public static ArrayList todaysHoldTexts;
        //public static int todaysHoldCount;


        public static EventLog mEventLog;

        public static bool isRunning = true;

        private static string[] seperator1 = new string[] { "\r\n" };
        private static string[] seperator2 = new string[] { ":" };



        

        public static bool runDailyHoldNotices()
        {
            bool X = false;


            try
            {

                string date_ext = DateTime.Now.ToString("MM_dd_yyyy");

                FileStream fis = new FileStream(FileDir + holdNoticeFile + date_ext, FileMode.Open);
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
                    todaysHoldNotices = fileStr.Split(cr, StringSplitOptions.None);
                    todaysHoldTexts = new ArrayList();
                    for (int i = 0; i < todaysHoldNotices.Length; i++)
                    {
                        if (todaysHoldNotices[i].Length > 0)
                        {
                            string[] pipe = new string[] { "|" };

                            Console.WriteLine(todaysHoldNotices[i]);
                            string[] txts = todaysHoldNotices[i].Split(pipe, StringSplitOptions.None);

                            todaysHoldTexts.Add(txts);

                        }
                    }

                    String[] what = { "sysmsg", "Daily Hold Texts" };
                    mModemManager.addToOutgoingMessages(todaysHoldTexts, what);
                    X = true;
                }
                else
                {
                    doEventLog("Main.runDailyHoldNotices(): Failed to read " + FileDir + holdNoticeFile + date_ext, 0);
                    sendMail("Athena failed to open the Daily Hold Notices file", "Program.runDailyHoldNotices(): Failed to read " + FileDir + holdNoticeFile + date_ext);
                }

            }
            catch (Exception e)
            {
                doEventLog("Program.runDailyHoldNotices(): " + e.Message + "\n\n" + e.StackTrace, 0);
            }
            return X;
        }



        // LOGGING

        public static void doEventLog(String msg, int l)
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
        public static void doLogFile(String msg, int l)
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
                StreamWriter mLogFile = new StreamWriter(AthenaDir + @"log\info_log", true);
                mLogFile.WriteLine(msg + "\r\n");
                mLogFile.Close();
            }
            catch (Exception e)
            {
                doLogFile(e.Message, 0);
            }
        }

        public static void sendMail(String sbj, String msg)
        {
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
                    doLogFile(e.Message + "\n\n" + e.StackTrace, 0);
                }
            }
        }



        //CONFIG

        public static void readConfigFile()
        {
            try
            {
                StreamReader cfg = new StreamReader(AthenaDir + "conf\\athenasms.conf");
                string cs = cfg.ReadToEnd();
                cfg.Close();
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
                StreamWriter cfg = new StreamWriter(AthenaDir + @"conf\\athenasms.conf", true);
                cfg.Write("//Athena Config File\r\n\r\n// Application Title\r\nAPPTITLE:Athena Sms Server\r\n\r\n//SQL HOST\r\nSQL_HOST:127.0.0.1\\ATHENASQL\r\n\r\n// Company Name\r\nCOMPANY:My Company\r\n\r\n//Email Host\r\nEMAILHOST:\r\n\r\n//Email Users\r\nEMAIL_SENDER:\r\n\r\nEMAIL_RECIPIENT:\r\n\r\n//Email Header\r\nEMAIL_HEADER:your app name\r\n\r\n//Modems\r\n//MODEM:COM?\r\n\r\n");
                cfg.Close();
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
            mModemManager.ShutDown();
            mSocketManager.ShutDown();
            if (mSqlDb != null)
            {
                mSqlDb.Disconnect();
                mSqlDb = null;
            }
        }

        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Service1() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
