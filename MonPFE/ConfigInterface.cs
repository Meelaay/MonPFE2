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
        private List<CheckBox> _weekDaysList;

        private DateTimePicker _startsAt;

        private Button _okButton;

        public void SetControlsState(bool IsEnabled)
        {
            foreach (var checkBox in _weekDaysList)
                checkBox.Enabled = IsEnabled;

            _startsAt.Enabled = IsEnabled;

            _okButton.Enabled = IsEnabled;
        }

        

        public void CronToInterface(string cronExpression)
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


        public ConfigInterface(DateTimePicker picker, Button okButton, params CheckBox[] weekdayChBox)
        {
            _startsAt = picker;
            _okButton = okButton;

            foreach (var checkBox in weekdayChBox)
                _weekDaysList.Add(checkBox);

        }

        public void InitializeConfigInterface()
        {
            
            //get current config from text file or sqlite db
        }

        
    }
}