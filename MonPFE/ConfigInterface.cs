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
            //0 5 12 ? * MON,TUE,WED,THU,FRI,SAT,SUN *
            #region InitList
            weekDays.Add("MON");
                weekDays.Add("TUE");
                weekDays.Add("WED");
                weekDays.Add("THU");
                weekDays.Add("FRI");
                weekDays.Add("SAT");
                weekDays.Add("SUN");
            //...

            #endregion

            // iterate through list of dayNames
            foreach (var weekDay in weekDays)
                if (cronExpression.Contains(weekDay))
                    CheckDay(weekDay);

            //if substr(str"day", cronExpr) returns true check the 
            //corr checkbox[i] using CheckDay()
            var minutes = cronExpression.Substring(2, 2);
            var hours = cronExpression.Substring(4, 3);

            DateTime dateTime = new DateTime(2018, 4, 2, Convert.ToInt32(hours), Convert.ToInt32(minutes), 0);

            _startsAt.Value = dateTime;

            //extract date and time from cronExpr and assign it to datetimePicker


        }

        private void CheckDay(string day)
        {
            //switch on day 
            switch (day)
            {
                case "MON":
                    _weekDaysList[0].Checked = true;
                    break;
                case "TUE":
                    _weekDaysList[1].Checked = true;
                    break;
                case "WED":
                    _weekDaysList[2].Checked = true;
                    break;
                case "THU":
                    _weekDaysList[3].Checked = true;
                    break;
                case "FRI":
                    _weekDaysList[4].Checked = true;
                    break;
                case "SAT":
                    _weekDaysList[5].Checked = true;
                    break;
                case "SUN":
                    _weekDaysList[6].Checked = true;
                    break;
            }
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
            _weekDaysList = new List<CheckBox>();
            foreach (var checkBox in weekdayChBox)
                _weekDaysList.Add(checkBox);

        }

        public void InitializeConfigInterface()
        {
            
            //get current config from text file or sqlite db
        }

        
    }
}