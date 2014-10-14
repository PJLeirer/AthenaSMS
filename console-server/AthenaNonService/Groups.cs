/*
 * Groups
 * 
 * just sends group messages, may be moved to ModemManager soon
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthenaService
{
    public class Groups
    {

        private ArrayList groupTexts;

        public bool textGroup(string g, String msg)
        {
            bool X = false;
            ArrayList cg = Program.mSqlDb.getContactGroup(g);
            if (cg != null)
            {
                groupTexts = new ArrayList();
                for (int i = 0; i < cg.Count; i++)
                {
                    if (cg[i] != null)
                    {
                        String[] row = (String[])cg[i];
                        groupTexts.Add(row);
                    }
                }
                String[] what = { "sysmsg", g + " Group Text sent to '" + g +"' group\n\n" + msg };
                X = Program.mModemManager.addToAndProcessOutgoingMessages(groupTexts, what);
            }
            return X;
        }





    }
}
