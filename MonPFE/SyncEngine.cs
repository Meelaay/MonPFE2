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
        private const string _sqLiteConnString = @"Data source = ..\..\localSqlite\localDatabase.db;Version=3;";
        private const string _sqlServerConnString = @"Data Source=DESKTOP-DJONOS6\SQLEXPRESS;Initial Catalog=PFE;Integrated Security=True;Connection Timeout = 5";

        private SQLiteConnector _sqLiteConnector;
        private SQLServerConnector _sqlServerConnector;

        private SyncTimeManager _timeManager;

        public Enum _connectivityStateEnum;

        public FormInterface _formInterface;

        public Client _client;
        private DatabaseDirectory _rootDirectory;
        

        public void ChangeCronExpr(string cronExpr)
        {
            try
            {
                bool a = _sqlServerConnector.IsCronExprValid(cronExpr);
                if (a)
                {
                    _sqlServerConnector.UpdateCronExprForClient(_client._clientID, cronExpr);
                    _sqLiteConnector.UpdateCronExprForClient(_client._clientID, cronExpr);
                    _timeManager.RescheduleFromCronExpr(cronExpr);
                    _client._cronExpression = cronExpr;
                    _formInterface._configInterface.Close();
                }
                else
                {
                    MessageBox.Show("Date/Time already picked.");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot connect to SQL Server.");
            }

            
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
                {
                    MessageBox.Show("Name already exists, please change it.");
                    return;
                }

                string insertOfflineQuery = String.Format("insert into Folders (name_folder, parent_folder, created_by_client, is_synced) values('{0}', {1}, {2}, {3})",
                    name, parentDirectory.id_folder, _client._clientID, 0);

                _sqLiteConnector.ExecuteInsertQuery(insertOfflineQuery);

                SetOfflineTree();

            }//=======================================================================================================================================================================
            else if ((ConnectivityState) _connectivityStateEnum == ConnectivityState.Online)
            {
                //check if sqlite has no synced items

                try
                {
                    int a = _sqlServerConnector.ExecuteScalarQuery(countQuery);

                    if (a != 0)
                    {
                        MessageBox.Show("Name already exists, please change it.");
                        return;
                    }

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
                catch (Exception e)
                {
                    MessageBox.Show("Cannot connect in SQL Server.");

                    _connectivityStateEnum = ConnectivityState.Offline;
                    SetOfflineTree();
                    _formInterface.EnableOnlineTree(isEnabled: false);
                    _formInterface.EnableOfflineTree(isEnabled: true);
                    _formInterface.PassOffline();
                    _timeManager.ContinueScheduler();

                }
                
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
            
            //if nothing to sync return


            if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Offline)
            {
                int a = _sqLiteConnector.ExecuteScalarQuery(countQuery);

                //throw exception or write function to check validity in sqlserver/lite
                //while a != 0 i++; originalName + ('i')
                if (a != 0)
                {
                    MessageBox.Show("Name already exists, please change it.");
                    return;
                }

                string insertOfflineQuery = String.Format("insert into Files (name_file, path_file, parent_folder, created_by_client, is_synced) values('{0}', '{1}', {2}, {3}, {4})",
                    name, path, parentDirectory.id_folder, _client._clientID, 0);

                _sqLiteConnector.ExecuteInsertQuery(insertOfflineQuery);

                SetOfflineTree();

            }//=======================================================================================================================================================================
            else if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Online)
            {
                try
                {
                    int a = _sqlServerConnector.ExecuteScalarQuery(countQuery);

                    if (a != 0)
                    {
                        MessageBox.Show("Name already exists, please change it.");
                        return;
                    }

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
                catch (Exception e)
                {
                    MessageBox.Show("Cannot connect in SQL Server.");

                    _connectivityStateEnum = ConnectivityState.Offline;
                    SetOfflineTree();
                    _formInterface.EnableOnlineTree(isEnabled: false);
                    _formInterface.EnableOfflineTree(isEnabled: true);
                    _formInterface.PassOffline();
                    _timeManager.ContinueScheduler();
                }

                
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
                    
                    MessageBox.Show("Cannot connect to SQL Server.");
                    break;

                case (int)ConnectivityState.Online:
                    try
                    {


                        //check if there's something to sync in files or folders
                        string filesCountQuery = "select count(*) from Files where is_synced = 0";
                        string foldersCountQuery = "select count(*) from Folders where is_synced = 0";

                        int nonSyncedFilesCount = _sqLiteConnector.ExecuteScalarQuery(filesCountQuery);
                        int nonSyncedFoldersCount = _sqLiteConnector.ExecuteScalarQuery(foldersCountQuery);

                        //if nothing to sync return
                        if (nonSyncedFoldersCount + nonSyncedFilesCount == 0)
                        {
                            MessageBox.Show("Nothing to synchronize.");
                            //disable offline tree
                            SetOnlineTree();
                            SetOfflineTree();
                            _formInterface.ExpandTrees();
                            _formInterface.EnableOnlineTree(true);
                            _formInterface.EnableOfflineTree(false);

                            //enable online tree
                            //reload directories


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
                                DataTable nonSyncedFoldersTable =
                                    _sqLiteConnector.ExecuteSelectQuery("select * from Folders where is_synced = 0");

                                int oldFolderID;

                                nonSyncedFoldersTable.Columns.Remove("is_synced");


                                int lastRowValidID = newValidID + nonSyncedFoldersTable.Rows.Count - 1;

                                //change ids accordignly
                                for (int i = nonSyncedFoldersTable.Rows.Count; i > 0; i--)
                                {
                                    oldFolderID = Convert.ToInt32(nonSyncedFoldersTable.Rows[i - 1]["id_folder"]);
                                    nonSyncedFoldersTable.Rows[i - 1]["id_folder"] = lastRowValidID;

                                    for (int j = nonSyncedFoldersTable.Rows.Count; j > 0; j--)
                                        if (Convert.ToInt32(nonSyncedFoldersTable.Rows[j - 1]["parent_folder"]) ==
                                            oldFolderID)
                                            nonSyncedFoldersTable.Rows[j - 1]["parent_folder"] = lastRowValidID;

                                    _sqlServerConnector.ExecuteSelectQuery("SELECT NEXT VALUE FOR id_folder_seq");

                                    lastRowValidID--;
                                }

                                _sqLiteConnector.ExportFromSqliteToSqlServer(_sqlServerConnector.GetConnection(),
                                    nonSyncedFoldersTable, "Folders");

                                //empty sqlite
                                _sqLiteConnector.PurgeTable("Folders");


                                //fill sqlite from sqlserver
                                _sqLiteConnector.BulkInsertFolders(_sqlServerConnector.ExecuteSelectQuery(
                                    "select * from Folders where created_by_client = " + _client._clientID
                                ));

                            }
                            else if (nonSyncedFoldersCount == 0 && nonSyncedFilesCount != 0)
                            {
                                DataTable nonSyncedFilesTable =
                                    _sqLiteConnector.ExecuteSelectQuery("select * from Files where is_synced = 0");

                                int newFilesValidID =
                                    _sqlServerConnector.ExecuteScalarQuery("SELECT MAX(id_file) FROM Files") + 1;
                                nonSyncedFilesTable.Columns.Remove("is_synced");
                                foreach (DataRow row in nonSyncedFilesTable.Rows)
                                {
                                    row["id_file"] = newFilesValidID;
                                    newFilesValidID++;
                                    _sqlServerConnector.ExecuteSelectQuery("SELECT NEXT VALUE FOR id_file_seq");
                                }

                                _sqLiteConnector.ExportFromSqliteToSqlServer(_sqlServerConnector.GetConnection(),
                                    nonSyncedFilesTable, "Files");
                                _sqLiteConnector.PurgeTable("Files");

                                _sqLiteConnector.BulkInsertFiles(_sqlServerConnector.ExecuteSelectQuery(
                                    "select * from Files where created_by_client = " + _client._clientID
                                ));


                            }
                            else if (nonSyncedFoldersCount != 0 && nonSyncedFilesCount != 0)
                            {
                                int newFoldersValidID =
                                    _sqlServerConnector.ExecuteScalarQuery("SELECT MAX(id_folder) FROM Folders") + 1;

                                int newFilesValidID =
                                    _sqlServerConnector.ExecuteScalarQuery("SELECT MAX(id_file) FROM Files") + 1;

                                //fill datatable of non synced items :
                                DataTable nonSyncedFoldersTable =
                                    _sqLiteConnector.ExecuteSelectQuery("select * from Folders where is_synced = 0");
                                DataTable nonSyncedFilesTable =
                                    _sqLiteConnector.ExecuteSelectQuery("select * from Files where is_synced = 0");

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
                                        if (Convert.ToInt32(nonSyncedFoldersTable.Rows[j - 1]["parent_folder"]) ==
                                            oldFolderID)
                                            nonSyncedFoldersTable.Rows[j - 1]["parent_folder"] = lastFoldersRowValidID;

                                    for (int j = nonSyncedFilesTable.Rows.Count; j > 0; j--)
                                        if (Convert.ToInt32(nonSyncedFilesTable.Rows[j - 1]["parent_folder"]) ==
                                            oldFolderID)
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

                                _sqLiteConnector.ExportFromSqliteToSqlServer(_sqlServerConnector.GetConnection(),
                                    nonSyncedFoldersTable, "Folders");
                                _sqLiteConnector.ExportFromSqliteToSqlServer(_sqlServerConnector.GetConnection(),
                                    nonSyncedFilesTable, "Files");
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
                    }
                    catch (InvalidOperationException e)
                    {
                        return;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Cannot connect to SQL Server.");
                        _connectivityStateEnum = ConnectivityState.Offline;

                        SetOfflineTree();
                        _formInterface.EnableOnlineTree(isEnabled: false);
                        _formInterface.EnableOfflineTree(isEnabled: true);
                        _formInterface.PassOffline();
                        _timeManager.ContinueScheduler();

                    }
                    
                    break;
            }
        }

        public void InitializeEngine(FormInterface formInterface)
        {
            _formInterface = formInterface;
            _sqLiteConnector = new SQLiteConnector(_sqLiteConnString);
            _formInterface = formInterface;



            _timeManager = new SyncTimeManager();
            _timeManager.Init(this);
            


            SetConnectivityState();
            DatabaseDirectory.SetConnectors(_sqlServerConnector, _sqLiteConnector);


            _formInterface.SetCorrStatus((ConnectivityState)_connectivityStateEnum);

            //_client = new Client(_sqlServerConnector, _sqLiteConnector, _connectivityStateEnum);
            
            
            //Debug:
            _client = new Client(_sqlServerConnector, _sqLiteConnector, _connectivityStateEnum);
            _timeManager.RescheduleFromCronExpr(_client._cronExpression);

            if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Offline)
            {
                if (_client.IsSet)
                {
                    //start last set schedule by client (cronExpr from sqlite)
                    string cronExpr = _client.GetCron();

                    //_timeManager.RescheduleFromCronExpr(cronExpr);
                    _timeManager.StartScheduler(2);

                    //set interface depending on cronExpression
                    _formInterface.SetConfigInterfaceCron(cronExpr);
                }
                else
                {
                    //no sync for him
                }


                //disables interface for changing jobs

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
                //_timeManager.RescheduleFromCronExpr(cronExpr);
                _timeManager.StartScheduler(2);

                //Enable formInterface for change of cronEpxr
                

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
            Debug.WriteLine("Job Started");

            var schedulerContext = context.Scheduler.Context;

            var currentEngine    = (SyncEngine) schedulerContext.Get("engine");

            currentEngine.SetConnectivityState();

            if ((ConnectivityState) currentEngine._connectivityStateEnum == ConnectivityState.Online)
                currentEngine._formInterface.PassOnline();
            else
                currentEngine._formInterface.PassOffline();


            Debug.WriteLine("Job Ended");
        }

        public void SetConnectivityState()
        {

            if (_sqlServerConnector != null && _sqlServerConnector.TestConnectivity() == (int) ExitCode.Success)
            {
                _connectivityStateEnum = ConnectivityState.Online;
                _timeManager.StopScheduler(1);
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
                        _timeManager.StopScheduler(1);
                    }
                    else
                        throw new Exception("Cannot connect to sqlserver.");

                }
                catch (Exception e)
                {
                    Debug.WriteLine("SyncEngine::SetConnectivityState() -> " + e.Message);
                    _timeManager.StartScheduler(1);
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