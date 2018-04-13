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
        private ConfigInterface configInter;

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
                dateTimePicker1, okButton, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9 );

        }

        public ConfigInterface GetConfigInterface()
        {
            return configInter;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //on ok click check if taken and signal failure if exists
            Form1._engine.test("");
            //else accept values and register it in database
        }

        private void ConfigMenu_Load(object sender, EventArgs e)
        {
            //sm.Init();

            MessageBox.Show("loaded.");




            /* select 
             *
             */




        }
    }

}
