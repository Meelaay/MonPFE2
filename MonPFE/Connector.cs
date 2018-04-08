using System.Data;

namespace MonPFE
{
    public abstract class Connector
    {
        //_isUsed can be used later as a switch to change state (online/offline)
        //todo set isActive in connectors
        protected bool _isActive { get; }

        protected readonly string _connectionString;

        const string host = "";
        const string port = "";
        const string user = "";
        const string pass = "";

        public abstract DataTable ExecuteSelectQuery(string query);

        public abstract int ExecuteInsertQuery(string query);

        public Connector(string connectionString)
        {
            this._connectionString = connectionString;
        }
    }
}