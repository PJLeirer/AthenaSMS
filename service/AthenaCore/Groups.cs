using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthenaCore
{
    public class Groups
    {

        private ArrayList groupTexts;
        private String[] textGroups;// = { "Clevnet", "Techs", "Auto", "Staff" };

        public void textGroup(int g, String msg)
        {
            /*
            textGroups = Program.mSqlDb.getGroupList();
            ArrayList cg = Program.mSqlDb.getContactGroup(textGroups[g]);
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
                String[] what = { "sysmsg", textGroups[g] + " Group Texts" };
                Program.mModemManager.addToOutgoingMessages(groupTexts, what);
             
            }*/
        }





    }
}
