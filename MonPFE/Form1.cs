using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonPFE
{
    public partial class Form1 : Form
    {
        private FormInterface _formInterface;
        private SyncEngine _engine;
        private ConfigMenu _configMenu = new ConfigMenu();
           


        //todo override what close button does so it keeps app running in bg
        //=====================================================================
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
        //=====================================================================


        public Form1()
        {
            InitializeComponent();


            _formInterface = new FormInterface();
            _formInterface.InitiatlizeFormInterface(pBoxStatIndicator, tView, treeView1, btnSync, _configMenu.GetConfigInterface());

            _engine = new SyncEngine();
            _engine.InitializeEngine(_formInterface);

        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {   
            /* used to close app
            this.WindowState = FormWindowState.Minimized;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.ShowInTaskbar = false;
            */

        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            


            //Sdfgsdfgdsfgsdffgh 

            /*
                string path = "init";
                OpenFileDialog file = new OpenFileDialog();
                if (file.ShowDialog() == DialogResult.OK)
                {
                    path = file.FileName;
                }
                MessageBox.Show(path);
            
            */
            
        }

        private void schedulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try catch {get state of cnc and cronExpr}
            _configMenu.Show();

            _configMenu.Activate();
        }

        private void btnSync_Click(object sender, EventArgs e)
        {

        }
    }
}
