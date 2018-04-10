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



        public Client(SQLServerConnector sqlServerConnector, SQLiteConnector sqLiteConnector, Enum connectivityState)
        {
            if ((ConnectivityState) connectivityState == ConnectivityState.Offline)
                _isOnline = false;
            else _isOnline = true;

            bool? firstTimeClient = sqLiteConnector.IsNotEmpty(tableName: "Client");

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
                    var clientRow = sqLiteConnector.ExecuteSelectQuery("SELECT * FROM Client").Rows[0];
                    
                    _clientID = Convert.ToInt32(clientRow["..."]);
                    _clientName = clientRow["..."].ToString();
                    //...
                    //update IPs if they're different write them to sqlite and server?

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

                }
                
            }

        }

        public void SetClientState()
        {
            //
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
