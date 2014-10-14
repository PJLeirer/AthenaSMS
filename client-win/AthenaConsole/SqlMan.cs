using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthenaConsole
{
    public class SqlMan
    {
        public MainWindow mMainWin;

        private String dBase = "athenasms";
        private String dbUser = "sa";  //TODO change to new sql user i created
        private String dbPass = ""; // and pass

        public SqlMan(MainWindow w)
        {
            mMainWin = w;
        }

        public void ShowOutgoingtexts()
        {
            /*
            using (SqlConnection conn = new SqlConnection("integrated security=SSPI;data source=" + @"CANDY-PC\ATHENASQL" + ";Persist Security Info=false;UID=" + dbUser + ";PWD=" + dbPass + ";Initial Catalog=" + dBase + ";"))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("select * from OutgoingTexts;", conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string row = "";
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row += " " + reader[i];
                                    if (i + 1 < reader.FieldCount)
                                    {
                                        row += @" | ";
                                    }
                                }
                                mMainWin.OutgoingTextsListUpdater(row);
                            }
                        }
                    }
                    command.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
             */
        }

        public void ShowIncomingTexts()
        {
            /*
            using (SqlConnection conn = new SqlConnection("integrated security=SSPI;data source=" + @"CANDY-PC\ATHENASQL" + ";Persist Security Info=false;UID=" + dbUser + ";PWD=" + dbPass + ";Initial Catalog=" + dBase + ";"))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("select * from IncomingTexts;", conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string row = "";
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row += " " + reader[i];
                                    if (i + 1 < reader.FieldCount)
                                    {
                                        row += @" | ";
                                    }
                                }
                                mMainWin.IncomingTextsListUpdater(row);
                            }
                        }
                    }
                    command.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
             */
        }

        public void ShowFailedTexts()
        {
            /*
            using (SqlConnection conn = new SqlConnection("integrated security=SSPI;data source=" + @"CANDY-PC\ATHENASQL" + ";Persist Security Info=false;UID=" + dbUser + ";PWD=" + dbPass + ";Initial Catalog=" + dBase + ";"))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("select * from FailedOutgoing;", conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string row = "";
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row += " " + reader[i];
                                    if (i + 1 < reader.FieldCount)
                                    {
                                        row += @" | ";
                                    }
                                }
                                mMainWin.FailedTextsListUpdater(row);
                            }
                        }
                    }
                    command.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
             */
        }



    }
}
