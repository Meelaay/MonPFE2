using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            _formInterface.InitiatlizeFormInterface(pBoxStatIndicator, onlineTView, offlineTView, btnSync, _configMenu.GetConfigInterface());

            _engine = new SyncEngine();
            _engine.InitializeEngine(_formInterface);
            offlineTView.ExpandAll();
            onlineTView.ExpandAll();
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {

            DatabaseDirectory selectedDir = new DatabaseDirectory(-1, "null", -1);

            string promptValue = Prompt.ShowDialog("New folder name :", "New Folder...");


            if (onlineTView.Enabled)
            {
                try
                {
                    if (typeof(DatabaseDirectory) == onlineTView.SelectedNode.Tag.GetType())
                    {
                        selectedDir = (DatabaseDirectory)onlineTView.SelectedNode.Tag;
                    }
                    else if (typeof(DatabaseFile) == onlineTView.SelectedNode.Tag.GetType())
                    {
                        selectedDir = (DatabaseDirectory)onlineTView.SelectedNode.Parent.Tag;

                    }
                }
                catch (NullReferenceException ne)
                {
                    //if no folder was selected, select root
                    selectedDir = (DatabaseDirectory)onlineTView.Nodes[0].Tag;

                }
            }
            else if (offlineTView.Enabled)
            {
                try
                {
                    if (typeof(DatabaseDirectory) == offlineTView.SelectedNode.Tag.GetType())
                    {
                        selectedDir = (DatabaseDirectory)offlineTView.SelectedNode.Tag;
                    }
                    else if (typeof(DatabaseFile) == offlineTView.SelectedNode.Tag.GetType())
                    {
                        selectedDir = (DatabaseDirectory)offlineTView.SelectedNode.Parent.Tag;
                    }

                }
                catch (NullReferenceException ne)
                {
                    //if no folder was selected, select root
                    selectedDir = (DatabaseDirectory)offlineTView.Nodes[0].Tag;
                }
            }

            //===========================================================================
            //FOR DEBUGING 
            //string c = $"selected file : id : {selectedFile.id_file}, name : {selectedFile.name_file}, path : {selectedFile.path_file}, parent : {selectedFile.parent_folder}";
            //string a = $"selected folder : id : {selectedDir.id_folder}, name : {selectedDir.name_Folder}, parent : {selectedDir.parent_folder}";

            //Debug.WriteLine(c);
            //Debug.WriteLine(a);
            //===========================================================================


            if (promptValue == "")
            {

            }
            else
            {
                if(selectedDir.id_folder != -1)
                    _engine.AddFolder(promptValue, selectedDir);

                
                 

            }




            



            /* used to "close" app
            this.WindowState = FormWindowState.Minimized;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.ShowInTaskbar = false;
            */

        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {

            DatabaseDirectory selectedDir = new DatabaseDirectory(-1, "null", -1);


            if (onlineTView.Enabled)
            {
                try
                {
                    if (typeof(DatabaseDirectory) == onlineTView.SelectedNode.Tag.GetType())
                    {
                        selectedDir = (DatabaseDirectory)onlineTView.SelectedNode.Tag;
                    }
                    else if (typeof(DatabaseFile) == onlineTView.SelectedNode.Tag.GetType())
                    {
                        selectedDir = (DatabaseDirectory)onlineTView.SelectedNode.Parent.Tag;

                    }
                }
                catch (NullReferenceException ne)
                {
                    //if no folder was selected, select root
                    selectedDir = (DatabaseDirectory)onlineTView.Nodes[0].Tag;

                }
            }
            else if (offlineTView.Enabled)
            {
                try
                {
                    if (typeof(DatabaseDirectory) == offlineTView.SelectedNode.Tag.GetType())
                    {
                        selectedDir = (DatabaseDirectory)offlineTView.SelectedNode.Tag;
                    }
                    else if (typeof(DatabaseFile) == offlineTView.SelectedNode.Tag.GetType())
                    {
                        selectedDir = (DatabaseDirectory)offlineTView.SelectedNode.Parent.Tag;
                    }

                }
                catch (NullReferenceException ne)
                {
                    //if no folder was selected, select root
                    selectedDir = (DatabaseDirectory)offlineTView.Nodes[0].Tag;
                }
            }




            //===========================================
            
            OpenFileDialog file = new OpenFileDialog();

            if (file.ShowDialog() == DialogResult.OK)
                if(selectedDir.id_folder != -1)
                _engine.AddFile(file.SafeFileName, file.FileName, selectedDir);
                

            //===========================================







        }

        private void schedulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try catch {get state of cnc and cronExpr}
            _configMenu.Show();

            _configMenu.Activate();
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            _engine.Synchronize();


            

            /*
            _engine.test("SET IDENTITY_INSERT Folders ON; " +
                         "INSERT INTO Folders (id_folder, name_folder, parent_folder, created_by_client) VALUES(15555, 'folder1000', 1, 1); " +
                         "SET IDENTITY_INSERT Folders OFF");
            */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _engine.test("");
            //            string a = $"online tView : {onlineTView.Enabled}, offline tView : {offlineTView.Enabled}";
            //          Debug.WriteLine(a);
        }


    }
}
