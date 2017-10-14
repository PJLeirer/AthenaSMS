using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthenaCore
{
    public class Core
    {

        public SqlDb mSqlDb;
        public SocketManager mSocketManager;
        public ModemManager mModemManager;

        //public EventLog mEventLog;

        public Core()
        {
            readConfigFile();
            mSqlDb = new SqlDb(this);
            mSocketManager = new SocketManager(this);
            mModemManager = new ModemManager(this);

        }


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
                    mModemManager.addToOutgoingMessages(Resources.todaysHoldTexts, what);
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

        public void readConfigFile()
        {
            try
            {
                StreamReader cfg = new StreamReader(Resources.AthenaDir + "conf\\athenasms.conf");
                string cs = cfg.ReadToEnd();
                cfg.Close();
                string[] configInfo = cs.Split(Resources.seperator1, StringSplitOptions.None);

                for (int i = 0; i < configInfo.Length; i++)
                {
                    if (configInfo[i].Length > 0 && !configInfo[i].StartsWith("//"))
                    {
                        string[] tmp = configInfo[i].Split(Resources.seperator2, StringSplitOptions.None);
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
            }
            catch (Exception e)
            {
                doEventLog("Main.readConfigFile(): " + e.Message, 1);
                createConfigFile();
            }
        }

        public void deleteConfigFile()
        {
            File.Delete(Resources.AthenaDir + "conf\\");
        }

        public void createConfigFile()
        {
            try
            {
                StreamWriter cfg = new StreamWriter(Resources.AthenaDir + @"conf\\athenasms.conf", true);
                cfg.Write("//Athena Config File\r\n\r\n// Application Title\r\nAPPTITLE:Athena Sms Server\r\n\r\n//SQL HOST\r\nSQL_HOST:127.0.0.1\\SQLEXPRESS\r\nSQL_USER:sa\r\nSQL_PASS:\r\n\r\n// Company Name\r\nCOMPANY:My Company\r\n\r\n//Email Host\r\nEMAILHOST:\r\n\r\n//Email Users\r\nEMAIL_SENDER:\r\n\r\nEMAIL_RECIPIENT:\r\n\r\n//Email Header\r\nEMAIL_HEADER:your app name\r\n\r\n//Modems\r\n// dummy modem\r\nMODEM:DMY0\r\n//MODEM:COM?\r\n\r\n");
                cfg.Close();
                readConfigFile();
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
            doLogFile(msg, 0);
            /*
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
            */
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

        public void sendMail(String sbj, String msg)
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

        public void ShutDown()
        {
            mModemManager.ShutDown();
            mSocketManager.ShutDown();
            if (mSqlDb != null)
            {
                mSqlDb.Disconnect();
                mSqlDb = null;
            }
        }

    }
}
