using System;
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

        public void test()
        {
            //
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            MessageBox.Show(userName);

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
                _sqlServerConnector.ExecuteSelectQuery("select * from Folders where id_folder = 0").Rows[0],
                _sqlServerConnector
            );
            
            _formInterface.DrawTree(_rootDirectory, isOnline: true);
        }

        private void SetOfflineTree()
        {
            _rootDirectory = new DatabaseDirectory(clientID: _client._clientID);

            _rootDirectory = _rootDirectory.CreateNode(
                _sqLiteConnector.ExecuteSelectQuery("select * from Folders where id_folder = 0").Rows[0],
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

        

        private void Synchronize()
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
                    bool? a = _sqLiteConnector.IsNotEmpty(null);

                    if (a == true)
                    {
                        //BUG careful of GetConnection() dispose prblm
                        _sqLiteConnector.ImportFromSqliteToSqlServer(_sqlServerConnector.GetConnection());
                        this._sqLiteConnector.PurgeTable();

                        //BUG ???
                        //bug     _timeManager.StopScheduler(1);
                    }

                    break;
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