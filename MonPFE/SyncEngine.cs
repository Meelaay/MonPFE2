using System;
using System.CodeDom;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quartz;

namespace MonPFE
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class SyncEngine : IJob
    {
        //to transfer to corr classes
        private const string _sqLiteConnString = @"Data source = ..\..\localSqlite\localDatabase.db;Version=3;";
        private const string _sqlServerConnString = @"Data Source=DESKTOP-DJONOS6\SQLEXPRESS;Initial Catalog=PFE;Integrated Security=True;Connection Timeout = 5";

        private SQLiteConnector _sqLiteConnector;
        private SQLServerConnector _sqlServerConnector;

        //bug private SyncTimeManager _timeManager;

        private Enum _connectivityStateEnum;

        private FormInterface _formInterface;

        private Client _client;
        private DatabaseDirectory _rootDirectory;

        private int times = 0;

        public void test(string a)
        {
            var b = _sqlServerConnector.ExecuteScalarQuery(a);
            MessageBox.Show(b.ToString());
            //_sqLiteConnector.ExecuteInsertQuery("insert into Files (id_file, name_file, path_file, parent_folder, created_by_client, is_synced) values(21, 'test', 'path1', 2, 1, 0 )");

            //_sqLiteConnector.ExecuteInsertQuery("insert into Files (name_file, path_file, parent_folder, created_by_client, is_synced) values('test', 'path2', 3, 1, 0)");

            //
            //string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //MessageBox.Show(userName);

            /*
            times++;
            if (times == 1)
                _timeManager.StartScheduler(2);

            if (times == 2)
                _timeManager.RescheduleFromCronExpr("0/15 0/1 * 1/1 * ? *");

            if (times == 3)
                _timeManager.RescheduleFromCronExpr("0/7 0/1 * 1/1 * ? *");

            if (times == 4)
                _timeManager.RescheduleFromCronExpr("0/2 0/1 * 1/1 * ? *");
            */
        }
        
        private void SetCorrespondantJobDependingOnClientState(bool a)
        {

        }

        public void AddFolder(string name, DatabaseDirectory parentDirectory)
        {
            //check if folder name already exists within same parent folder
            string countQuery = string.Format("select count(*) from Folders where name_folder = '{0}' and parent_folder = {1}", name,
                    parentDirectory.id_folder);
            

            if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Offline)
            {
                int a = _sqLiteConnector.ExecuteScalarQuery(countQuery);

                //throw exception or write function to check validity in sqlserver/lite
                if (a != 0)
                    name = name + " (2)";

                string insertOfflineQuery = String.Format("insert into Folders (name_folder, parent_folder, created_by_client, is_synced) values('{0}', {1}, {2}, {3})",
                    name, parentDirectory.id_folder, _client._clientID, 0);

                _sqLiteConnector.ExecuteInsertQuery(insertOfflineQuery);

                SetOfflineTree();

            }//=======================================================================================================================================================================
            else if ((ConnectivityState) _connectivityStateEnum == ConnectivityState.Online)
            {
                //check if sqlite has no synced items


                int a = _sqlServerConnector.ExecuteScalarQuery(countQuery);

                if (a != 0)
                    name = name + " (2)";

                string insertOnlineQuery = String.Format("insert into Folders (id_folder, name_folder, parent_folder, created_by_client) values(next value for id_folder_seq, '{0}', {1}, {2})",
                    name, parentDirectory.id_folder, _client._clientID);

                _sqlServerConnector.ExecuteInsertQuery(insertOnlineQuery);
                //get last id of inserted record
                int newId = _sqlServerConnector.ExecuteScalarQuery("SELECT MAX(id_folder) FROM Folders");

                string insertOfflineQuery = String.Format("insert into Folders (id_folder, name_folder, parent_folder, created_by_client, is_synced) values({0}, '{1}', {2}, {3}, {4})",
                    newId, name, parentDirectory.id_folder, _client._clientID, 1);

                _sqLiteConnector.ExecuteInsertQuery(insertOfflineQuery);

                SetOnlineTree();
            }
            _formInterface.ExpandTrees();

        }

        public void AddFile(string name, string path, DatabaseDirectory parentDirectory)
        {
            //check if folder name already exists within same parent folder
            string countQuery = string.Format("select count(*) from Files where name_file = '{0}' and parent_folder = {1}", name,
                    parentDirectory.id_folder);

            string filesCountQuery = "select count(*) from Files where is_synced = 0";
            string foldersCountQuery = "select count(*) from Folders where is_synced = 0";
            int nonSyncedFilesCount = _sqLiteConnector.ExecuteScalarQuery(filesCountQuery);
            int nonSyncedFoldersCount = _sqLiteConnector.ExecuteScalarQuery(foldersCountQuery);

            //if nothing to sync return
            //if (nonSyncedFoldersCount + nonSyncedFilesCount == 0)
                //_connectivityStateEnum = ConnectivityState.Offline;




            if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Offline)
            {
                int a = _sqLiteConnector.ExecuteScalarQuery(countQuery);

                //throw exception or write function to check validity in sqlserver/lite
                //while a != 0 i++; originalName + ('i')
                if (a != 0)
                    name = name + " (2)";

                string insertOfflineQuery = String.Format("insert into Files (name_file, path_file, parent_folder, created_by_client, is_synced) values('{0}', '{1}', {2}, {3}, {4})",
                    name, path, parentDirectory.id_folder, _client._clientID, 0);

                _sqLiteConnector.ExecuteInsertQuery(insertOfflineQuery);

                SetOfflineTree();

            }//=======================================================================================================================================================================
            else if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Online)
            {
                int a = _sqlServerConnector.ExecuteScalarQuery(countQuery);

                if (a != 0)
                    name = name + " (2)";

                string insertOnlineQuery = String.Format("insert into Files (id_file, name_file, path_file, parent_folder, created_by_client) values(next value for id_file_seq, '{0}', '{1}', {2}, {3})",
                    name, path, parentDirectory.id_folder, _client._clientID);

                _sqlServerConnector.ExecuteInsertQuery(insertOnlineQuery);
                //get last id of inserted record
                int newId = _sqlServerConnector.ExecuteScalarQuery("SELECT MAX(id_file) FROM Files");

                string insertOfflineQuery = String.Format("insert into Files (id_file, name_file, path_file, parent_folder, created_by_client, is_synced) values({0} ,'{1}', '{2}', {3}, {4}, {5})",
                    newId, name, path, parentDirectory.id_folder, _client._clientID, 1);

                _sqLiteConnector.ExecuteInsertQuery(insertOfflineQuery);

                SetOnlineTree();
            }
            _formInterface.ExpandTrees();





        }

        public void Synchronize()
        {
            int state = Convert.ToInt32(_connectivityStateEnum);
            //todo try to reconnect sqlserver instance
            switch (state)
            {
                case (int)ConnectivityState.Offline:
                    Debug.WriteLine("Cannot connect to internet");


                    //bug  _timeManager.StartScheduler(1);

                    break;

                case (int)ConnectivityState.Online:
                    //check if there's something to sync in files or folders
                    string filesCountQuery = "select count(*) from Files where is_synced = 0";
                    string foldersCountQuery = "select count(*) from Folders where is_synced = 0";
                    int nonSyncedFilesCount = _sqLiteConnector.ExecuteScalarQuery(filesCountQuery);
                    int nonSyncedFoldersCount = _sqLiteConnector.ExecuteScalarQuery(foldersCountQuery);

                    //if nothing to sync return
                    if (nonSyncedFoldersCount + nonSyncedFilesCount == 0)
                    {
                        MessageBox.Show("Nothing to synchronize.");
                        return;
                    }
                    else
                    {
                        //lock db or use transaction here
                        if (nonSyncedFoldersCount != 0 && nonSyncedFilesCount == 0)
                        {
                            //get last id of folders :                                                ===     +1
                            int newValidID =
                                _sqlServerConnector.ExecuteScalarQuery("SELECT MAX(id_folder) FROM Folders") + 1;
                            //fill datatable of non synced items :
                            DataTable nonSyncedFoldersTable = _sqLiteConnector.ExecuteSelectQuery("select * from Folders where is_synced = 0");

                            int oldFolderID;

                            nonSyncedFoldersTable.Columns.Remove("is_synced");


                            int lastRowValidID = newValidID + nonSyncedFoldersTable.Rows.Count - 1;

                            //change ids accordignly
                            for (int i = nonSyncedFoldersTable.Rows.Count; i > 0; i--)
                            {
                                oldFolderID = Convert.ToInt32(nonSyncedFoldersTable.Rows[i - 1]["id_folder"]);
                                nonSyncedFoldersTable.Rows[i - 1]["id_folder"] = lastRowValidID;

                                for (int j = nonSyncedFoldersTable.Rows.Count; j > 0; j--)
                                    if (Convert.ToInt32(nonSyncedFoldersTable.Rows[j - 1]["parent_folder"]) == oldFolderID)
                                        nonSyncedFoldersTable.Rows[j - 1]["parent_folder"] = lastRowValidID;

                                _sqlServerConnector.ExecuteSelectQuery("SELECT NEXT VALUE FOR id_folder_seq");

                                lastRowValidID--;
                            }

                            _sqLiteConnector.ExportFromSqliteToSqlServer(_sqlServerConnector.GetConnection(), nonSyncedFoldersTable, "Folders");

                            //empty sqlite
                            _sqLiteConnector.PurgeTable("Folders");


                            //fill sqlite from sqlserver
                            _sqLiteConnector.BulkInsertFolders(_sqlServerConnector.ExecuteSelectQuery(
                                "select * from Folders where created_by_client = " + _client._clientID
                            ));

                        }else if (nonSyncedFoldersCount == 0 && nonSyncedFilesCount != 0)
                        {
                            DataTable nonSyncedFilesTable = _sqLiteConnector.ExecuteSelectQuery("select * from Files where is_synced = 0");

                            int newFilesValidID =
                                _sqlServerConnector.ExecuteScalarQuery("SELECT MAX(id_file) FROM Files") + 1;
                            nonSyncedFilesTable.Columns.Remove("is_synced");
                            foreach (DataRow row in nonSyncedFilesTable.Rows)
                            {
                                row["id_file"] = newFilesValidID;
                                newFilesValidID++;
                                _sqlServerConnector.ExecuteSelectQuery("SELECT NEXT VALUE FOR id_file_seq");
                            }

                            _sqLiteConnector.ExportFromSqliteToSqlServer(_sqlServerConnector.GetConnection(), nonSyncedFilesTable, "Files");
                            _sqLiteConnector.PurgeTable("Files");

                            _sqLiteConnector.BulkInsertFiles(_sqlServerConnector.ExecuteSelectQuery(
                                "select * from Files where created_by_client = " + _client._clientID
                            ));


                        }
                        else if(nonSyncedFoldersCount !=0 && nonSyncedFilesCount != 0)
                        {
                            int newFoldersValidID =
                                _sqlServerConnector.ExecuteScalarQuery("SELECT MAX(id_folder) FROM Folders") + 1;

                            int newFilesValidID =
                                _sqlServerConnector.ExecuteScalarQuery("SELECT MAX(id_file) FROM Files") + 1;
                            
                            //fill datatable of non synced items :
                            DataTable nonSyncedFoldersTable = _sqLiteConnector.ExecuteSelectQuery("select * from Folders where is_synced = 0");
                            DataTable nonSyncedFilesTable = _sqLiteConnector.ExecuteSelectQuery("select * from Files where is_synced = 0");

                            int oldFolderID;
                            
                            nonSyncedFoldersTable.Columns.Remove("is_synced");
                            nonSyncedFilesTable.Columns.Remove("is_synced");

                            int lastFoldersRowValidID = newFoldersValidID + nonSyncedFoldersTable.Rows.Count - 1;

                            //change ids accordignly
                            for (int i = nonSyncedFoldersTable.Rows.Count; i > 0; i--)
                            {
                                oldFolderID = Convert.ToInt32(nonSyncedFoldersTable.Rows[i - 1]["id_folder"]);
                                nonSyncedFoldersTable.Rows[i - 1]["id_folder"] = lastFoldersRowValidID;

                                for (int j = nonSyncedFoldersTable.Rows.Count; j > 0; j--)
                                    if (Convert.ToInt32(nonSyncedFoldersTable.Rows[j - 1]["parent_folder"]) == oldFolderID)
                                        nonSyncedFoldersTable.Rows[j - 1]["parent_folder"] = lastFoldersRowValidID;

                                for (int j = nonSyncedFilesTable.Rows.Count; j > 0; j--)
                                    if (Convert.ToInt32(nonSyncedFilesTable.Rows[j - 1]["parent_folder"]) == oldFolderID)
                                        nonSyncedFilesTable.Rows[j - 1]["parent_folder"] = lastFoldersRowValidID;


                                _sqlServerConnector.ExecuteSelectQuery("SELECT NEXT VALUE FOR id_folder_seq");

                                lastFoldersRowValidID--;
                            }


                            foreach (DataRow row in nonSyncedFilesTable.Rows)
                            {
                                row["id_file"] = newFilesValidID;
                                newFilesValidID++;
                                _sqlServerConnector.ExecuteSelectQuery("SELECT NEXT VALUE FOR id_file_seq");

                            }


                            //loop

                            _sqLiteConnector.ExportFromSqliteToSqlServer(_sqlServerConnector.GetConnection(), nonSyncedFoldersTable, "Folders");
                            _sqLiteConnector.ExportFromSqliteToSqlServer(_sqlServerConnector.GetConnection(), nonSyncedFilesTable, "Files");
                            //empty sqlite
                            _sqLiteConnector.PurgeTable("Folders");
                            _sqLiteConnector.PurgeTable("Files");


                            //fill sqlite from sqlserver
                            _sqLiteConnector.BulkInsertFolders(_sqlServerConnector.ExecuteSelectQuery(
                                "select * from Folders where created_by_client = " + _client._clientID
                            ));

                            _sqLiteConnector.BulkInsertFiles(_sqlServerConnector.ExecuteSelectQuery(
                                "select * from Files where created_by_client = " + _client._clientID
                            ));
                        }

                        //reset directories
                        SetOfflineTree();
                        SetOnlineTree();

                        _formInterface.EnableOnlineTree(true);
                        _formInterface.EnableOfflineTree(false);
                        //disable offline tree and enable online tree
                    }
                    
                    //sudo code :
                    //lock db
                    //get last id of folders or files
                    //fill 2 datatables of nonsynced items and return it
                    //bulk copy to sqlserver
                    //delete * from lite
                    //fill sqlite from sqlserver where idclient = {}
                    //unlock db

                    break;
            }
        }


        public void InitializeEngine(FormInterface formInterface)
        {
            _formInterface = formInterface;
            _sqLiteConnector = new SQLiteConnector(_sqLiteConnString);
            _formInterface = formInterface;



            //bug _timeManager = new SyncTimeManager();
            //bug _timeManager.Init(this);





            SetConnectivityState();
            DatabaseDirectory.SetConnectors(_sqlServerConnector, _sqLiteConnector);


            _formInterface.SetCorrStatus((ConnectivityState)_connectivityStateEnum);

            //_client = new Client(_sqlServerConnector, _sqLiteConnector, _connectivityStateEnum);
            
            
            //Debug:
            _client = new Client();


            if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Offline)
            {
                if (_client.IsSet)
                {
                    //start last set schedule by client (cronExpr from sqlite)
                    string cronExpr = _client.GetCron();
                    //bug  _timeManager.RescheduleFromCronExpr(cronExpr);
                    //bug  _timeManager.StartScheduler(2);

                    //set interface depending on cronExpression
                    _formInterface.SetConfigInterfaceCron(cronExpr);
                }
                else
                {
                    //no sync for him
                }


                //disables interface for changing jobs
                formInterface.EnableConfigInterface(false);

                //todo code for tree:
                SetOfflineTree();
                _formInterface.EnableOfflineTree(true);
                _formInterface.EnableOnlineTree(false);

                //throw new Exception("not yet configured");

            }
            else if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Online)
            {
                //launch job from cronExpr
                string cronExpr = _client.GetCron();
                //bug  _timeManager.RescheduleFromCronExpr(cronExpr);
                //bug  _timeManager.StartScheduler(2);

                //Enable formInterface for change of cronEpxr
                formInterface.EnableConfigInterface(IsEnabled: true);

                //todo code to load tree
                int nonSyncedFiles = _sqLiteConnector.ExecuteScalarQuery("SELECT COUNT(*) FROM Files WHERE is_synced = 0");
                int nonSyncedFolders = _sqLiteConnector.ExecuteScalarQuery("SELECT COUNT(*) FROM Folders WHERE is_synced = 0");

                if (nonSyncedFolders != 0 || nonSyncedFiles != 0)
                {
                    //formInterface --> disable sqlserver tree and enable sqlite tree
                    _formInterface.EnableOnlineTree(isEnabled: false);
                    _formInterface.EnableOfflineTree(isEnabled: true);

                    SetOnlineTree();
                    SetOfflineTree();
                    
                    //bug prompt to sync and lock database if accepted to sync ??
                }
                else
                {
                    _formInterface.EnableOnlineTree(isEnabled: true);
                    _formInterface.EnableOfflineTree(isEnabled: false);
                    SetOnlineTree();
                }

            }

            


        }


        

        public void stop()
        {
            //bug  _timeManager.StopScheduler(1);
        }

        private void SetOnlineTree()
        {
            _rootDirectory = new DatabaseDirectory(clientID: _client._clientID);

            _rootDirectory = _rootDirectory.CreateNode(
                _sqlServerConnector.ExecuteSelectQuery("select * from Folders where id_folder = 1").Rows[0],
                _sqlServerConnector
            );
            
            _formInterface.DrawTree(_rootDirectory, isOnline: true);
        }

        private void SetOfflineTree()
        {
            _rootDirectory = new DatabaseDirectory(clientID: _client._clientID);

            _rootDirectory = _rootDirectory.CreateNode(
                _sqLiteConnector.ExecuteSelectQuery("select * from Folders where id_folder = 1").Rows[0],
                _sqLiteConnector
            );

            _formInterface.DrawTree(_rootDirectory, isOnline: false);
        }
        

        public async Task Execute(IJobExecutionContext context)
        {
            Console.Beep();
            Debug.WriteLine("Job Started");

            var schedulerContext = context.Scheduler.Context;

            var currentEngine    = (SyncEngine)    schedulerContext.Get("engine");
            //var currentInterface = (FormInterface) schedulerContext.Get("formInterface");

            currentEngine.SetConnectivityState();
            //MessageBox.Show("EnumStr : " + currentEngine.GetEnum());
            //MessageBox.Show("EnumItself: " + currentEngine._connectivityStateEnum.ToString());

            if ((ConnectivityState) currentEngine._connectivityStateEnum == ConnectivityState.Online)
            {
                currentEngine._formInterface.PassOnline();
                //currentEngine.stop();
            }
                
            else
                currentEngine._formInterface.PassOffline();

            Debug.WriteLine("Job Ended");
            //currentInterface.EnableSyncButton();
            //currentEngine.SetConnectivityState();
            //currentInterface.SetCorrStatus(currentConnectivity);
            //return null;
        }


        private void SetConnectivityState()
        {

            if (_sqlServerConnector != null && _sqlServerConnector.TestConnectivity() == (int) ExitCode.Success)
            {
                _connectivityStateEnum = ConnectivityState.Online;
                //bug  _timeManager.StopScheduler(1);
            }
            else
            {
                try
                {
                    InstantiateSQLServerConnection();

                    if (_sqlServerConnector.TestConnectivity() == (int)ExitCode.Success)
                    {
                        Debug.WriteLine("connectivityenum SET ONLINE");
                        _connectivityStateEnum = ConnectivityState.Online;
                        //bug  _timeManager.StopScheduler(1);
                    }
                    else
                        throw new Exception("Cannot connect to sqlserver.");

                }
                catch (Exception e)
                {
                    Debug.WriteLine("SyncEngine::SetConnectivityState() -> " + e.Message);
                    //bug  _timeManager.StartScheduler(1);
                    _connectivityStateEnum = ConnectivityState.Offline;
                }
            }
        }

        

        

        private void InstantiateSQLServerConnection()
        {
            try
            {
                _sqlServerConnector = new SQLServerConnector(_sqlServerConnString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("SyncEngine::InstantiateSQLServer() -> " + e.Message);
                _sqlServerConnector = null;
                throw e;
            }
        }




    }
    

    
}