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
        private int    _clientID;
        private string _clientPublicIP;
        private string _clientPrivateIP;
        private string _machineName;
        private string _username;


        public Client(Connector connector)
        {
            /*
             * depending on type of connector thats passed as arg
             * we should see if we need to get ip from whatsmyip
             *
             *
             */



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
