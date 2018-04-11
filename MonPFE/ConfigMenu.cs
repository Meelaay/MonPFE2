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
        public ConfigMenu()
        {
            InitializeComponent();

            #region Formatting day picker
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "HH:mm"; // Only use hours and minutes
            dateTimePicker1.ShowUpDown = true;
            
            #endregion

            //todo add check connection every 2 3 5 min ?

            //ConfigInterface configInter = new ConfigInterface();

        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //on ok click check if taken and signal failure if exists
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
