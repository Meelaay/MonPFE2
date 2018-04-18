using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MonPFE
{


    public partial class ConfigMenu : Form
    {
        public ConfigInterface configInter;

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public ConfigMenu()
        {
            InitializeComponent();

            #region Formatting day picker
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "HH:mm"; // Only use hours and minutes
            dateTimePicker1.ShowUpDown = true;
            
            #endregion

            //todo add check connection every 2 3 5 min ?

            configInter = new ConfigInterface(
                dateTimePicker1, okButton, this, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9);

        }

        public ConfigInterface GetConfigInterface()
        {
            return configInter;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //cancel button
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //on ok click check if taken and signal failure if exists
            if ((ConnectivityState)Form1._engine._connectivityStateEnum == ConnectivityState.Offline)
                MessageBox.Show("Cannot connect to SQL Server.");
            else if ((ConnectivityState)Form1._engine._connectivityStateEnum == ConnectivityState.Online)
            {
                string cronExpr = configInter.InterfaceToCron();

                Form1._engine.ChangeCronExpr(cronExpr);


            }



            //else accept values and register it in database
        }

        public void Close()
        {
            button2_Click(null, null);
        }
        private void ConfigMenu_Load(object sender, EventArgs e)
        {


        }

        private void dateTimePicker1_KeyDown(object sender, KeyEventArgs e)
        {
            //e.SuppressKeyPress = true;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
           /*
           if (this.dateTimePicker1.Value.Minute % 5 == 0)
               return;

           if (this.dateTimePicker1.Value.Minute % 5 == 1)
               this.dateTimePicker1.Value = this.dateTimePicker1.Value.AddMinutes(4);

           if (this.dateTimePicker1.Value.Minute % 5 == 4)
               this.dateTimePicker1.Value = this.dateTimePicker1.Value.AddMinutes(-4);
            */
        }

        
    }

}
