using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonPFE
{
    

    //todo make class or fields static
    public class Client
    {
        private bool _isOnline;
        public bool IsSet;

        private int    _clientID;
        private string _clientName;

        //private string _clientPublicIP;
        //private string _clientPrivateIP;

        public int ScheduleID;

        private string _cronExpression;

        public string GetCron()
        {
            return _cronExpression;
        }

        private int _hour;
        private int _minute;

//        public string CronExpression
//        {
//            get { return this._cronExpression; }
//
//            set
//            {
//                this._cronExpression = value;
//                
//            }
//        }

        public Client()
        {
            //debug ctor
            _isOnline = true;
            this.IsSet = true;
            _clientID = 1;
            ScheduleID = 1;
            _clientName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            _cronExpression = "notSet";
        }

        public Client(SQLServerConnector sqlServerConnector, SQLiteConnector sqLiteConnector, Enum connectivityState)
        {
            if ((ConnectivityState) connectivityState == ConnectivityState.Offline)
                _isOnline = false;
            else _isOnline = true;

            //bool? firstTimeClient = sqLiteConnector.IsNotEmpty(tableName: "Client");

            bool firstTimeClient = false;

            if (_isOnline)
            {
                if (firstTimeClient == true)
                {
                    _clientID = sqlServerConnector.GetNewClientId();
                    _clientName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    
                    //BUG _clientPublicIP = "publicIP"; _clientPrivateIP = "privateIP";

                    //write to sqlite all data singleton
                    _cronExpression = sqlServerConnector.GetValidCronExpr();

                    sqlServerConnector.ExecuteInsertQuery("INSERT INTO ....");
                    sqLiteConnector.ExecuteInsertQuery("INSERT INTO ....");
                }
                else if (firstTimeClient == false)
                {
                    //laaater : update IPs if they're different write them to sqlite and server?

                    SetClientFromSqlite(sqLiteConnector);

                }

                IsSet = true;
            }
            else
            {
                if (firstTimeClient == true)
                {
                    IsSet = false;
                    _clientID = -1;
                    
                    //BUG _clientPrivateIP = _clientPublicIP = "Not Connected";
                    
                    
                    //...
                    //
                }
                else
                {
                    //instantiante from sqlite
                    SetClientFromSqlite(sqLiteConnector);
                    IsSet = true;
                }
                
            }

        }

        private void SetNewClientFromSqlServer() { }

        private void SetClientFromSqlite(SQLiteConnector sqLiteConnector)
        {
            var clientRow = sqLiteConnector.ExecuteSelectQuery("SELECT * FROM Client").Rows[0];

            this._clientID = Convert.ToInt32(clientRow["id_client"]);
            this._clientName = clientRow["host_name"].ToString();

            //int id_schedule = Convert.ToInt32(clientRow["id_schedule"]);

            var scheduleRow = sqLiteConnector.ExecuteSelectQuery("SELECT * FROM Schedule").Rows[0];

            this._cronExpression = scheduleRow["cron_expr"].ToString();
            this._hour = Convert.ToInt32(scheduleRow["hour"]);
            this._minute = Convert.ToInt32(scheduleRow["minute"]);

        }

        public string GetClientPublicIP()
        {
            return "";
        }

        public string GetClientPrivateIP()
        {
            return "";
        }


    }
}
