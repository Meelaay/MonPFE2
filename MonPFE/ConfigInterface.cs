using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quartz;

namespace MonPFE
{
    
    public class ConfigInterface
    {
        private SyncFileManager _fileManager;

        private List<CheckBox> _weekDaysList;

        private DateTimePicker _startsAt;

        //private Button _okButton;
        //private Button _cancelButton;

        public void CronToInterface(string cronExpr)
        {
            List<string> weekDays = new List<string>(capacity:7);

            #region InitList
                weekDays.Add("MON");
            //...

            #endregion

            // iterate through list of dayNames
            //if substr(str"day", cronExpr) returns true check the 
            //corr checkbox[i] using CheckDay()

            //extract date and time from cronExpr and assign it to datetimePicker


        }

        private void CheckDay(string day)
        {
            //switch on day 
            //if == "MON" check weekDaysList[0]
            //...
        }


        public string InterfaceToCron()
        {
            StringBuilder cronExpression = new StringBuilder();

            cronExpression.Append("0 ");
            cronExpression.Append(_startsAt.Value.Minute);
            cronExpression.Append(" ");
            cronExpression.Append(_startsAt.Value.Hour);
            cronExpression.Append(" ? * ");

            List<string> daysList = new List<string>();

            if (_weekDaysList[0].Checked)
                daysList.Add("MON");
            if (_weekDaysList[1].Checked)
                daysList.Add("TUE");
            if (_weekDaysList[2].Checked)
                daysList.Add("WED");
            if (_weekDaysList[3].Checked)
                daysList.Add("THU");
            if (_weekDaysList[4].Checked)
                daysList.Add("FRI");
            if (_weekDaysList[5].Checked)
                daysList.Add("SAT");
            if (_weekDaysList[6].Checked)
                daysList.Add("SUN");

            var f = string.Join(", ", daysList);
            cronExpression.Append(f);
            cronExpression.Append(" *");


            return cronExpression.ToString();
        }


        public ConfigInterface()
        {
            //todo think what to be initialized
        }

        public void InitializeConfigInterface(SyncFileManager fileManager = null)
        {
            _fileManager = fileManager;
            //get current config from text file or sqlite db
        }
        
    }
}