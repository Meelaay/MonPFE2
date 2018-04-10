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

        private SyncTimeManager _timeManager;

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

            _timeManager = new SyncTimeManager();
            _timeManager.Init(this);

            SetConnectivityState();
            DatabaseDirectory.SetConnectors(_sqlServerConnector, _sqLiteConnector);


            _formInterface.SetCorrStatus((ConnectivityState)_connectivityStateEnum);

            _client = new Client(_sqlServerConnector, _sqLiteConnector, _connectivityStateEnum);

            if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Offline)
            {
                if (_client.IsSet)
                {
                    //start last set schedule by client (cronExpr from sqlite)
                    //set interface of cronExpression
                }
                else
                {
                    //no sync for him
                }


                //disable interface
            }
            else
            {
                //Load online tree
                //launch job from cronExpr
                //Enable formInterface for change of cronEpxr
            }
            





            if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Online)
                SetOnlineTree();
            else if ((ConnectivityState)_connectivityStateEnum == ConnectivityState.Offline)
                SetOfflineTree();
            

            
        }

        


        public void stop()
        {
            _timeManager.StopScheduler(1);
        }

        private void SetOnlineTree()
        {
            _rootDirectory = new DatabaseDirectory();

            _rootDirectory = _rootDirectory.CreateNode(
                _sqlServerConnector.ExecuteSelectQuery("select * from Folders where id_folder = 0").Rows[0]
            );

            _formInterface.DrawTree(_rootDirectory);
        }

        private void SetOfflineTree()
        {
           // Debug.WriteLine("DEBUG : OFFLINE TREE SET");

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

        

        private void Synchronize()
        {
            int state = Convert.ToInt32(_connectivityStateEnum);
            //todo try to reconnect sqlserver instance
            switch (state)
            {
                case (int)ConnectivityState.Offline:
                    Debug.WriteLine("Cannot connect to internet");
                   
                    //BUG ???
                    _timeManager.StartScheduler(1);

                    break;

                case (int)ConnectivityState.Online:
                    bool? a = _sqLiteConnector.IsNotEmpty(null);

                    if (a == true)
                    {
                        //BUG careful of GetConnection() dispose prblm
                        _sqLiteConnector.ImportFromSqliteToSqlServer(_sqlServerConnector.GetConnection());
                        this._sqLiteConnector.PurgeTable();
                        
                        //BUG ???
                        _timeManager.StopScheduler(1);
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