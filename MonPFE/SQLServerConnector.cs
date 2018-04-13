using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonPFE
{
    public class SQLServerConnector : Connector 
    {
        private SqlConnection _connection;

        public SQLServerConnector(string connectionString) : base(connectionString)
        {
            try
            {
                _connection = new SqlConnection(connectionString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("SQLServerConnector::SQLServerConnector -> " + e.Message);
                throw;
            }
        }

        //bug fix wait time at launch when not connected
        public int TestConnectivity()
        {
            _connection = new SqlConnection(base._connectionString);
            try
            {
                using (var command = new SqlCommand("SELECT 1", _connection))
                {
                    command.Connection.Open();
                    command.ExecuteScalar();
                    command.Connection.Close();

                    return (int)ExitCode.Success;
                }
            }
            catch (Exception e)
            {
                //bug remove this popup at the end
                Debug.WriteLine(e.Message);

                return (int)ExitCode.UnknownError;
            }
        }

        public int ExecuteScalarQuery(string query)
        {
            int countNumber = -1;
            _connection = new SqlConnection(base._connectionString);
            try
            {
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Connection.Open();
                    countNumber = Convert.ToInt32(command.ExecuteScalar()) ;
                    command.Connection.Close();

                }
            }
            catch (Exception e)
            {
                //bug remove this popup at the end
                Debug.WriteLine(e.Message);
                throw new Exception("error scalar sql server ...");
            }

            return countNumber;
        }

        public int GetNewClientId()
        {
            //get max number in table +1
            return 0;
        }

        public string GetValidCronExpr()
        {
            //var row1 = select * from schedules where id = 1 ?"prob"
            //string row1.cronExpr. +5

            //if IsCronExprValid() return 
            //
            return null;
        }

        public bool IsCronExprValid(string cronExpr)
        {
            //add 2 new columns hour and minutes
            //int a = select count(*) where hour = cronExpr... and min = cron...
            //if a != 0
            //return false
            //if a == 0 return true
            //throw exception
            return false;
            
        }

        


        public string GetClientPublicIP()
        {
            return "";
        }

        public string GetClientPrivateIP()
        {
            return "";
        }



        public override int ExecuteInsertQuery(string query)
        {
            _connection = new SqlConnection(base._connectionString);
            using (var command = new SqlCommand(query, _connection))
            {
                try
                {
                    command.Connection.Open();

                    //MessageBox.Show("SQLServerConnector::ExecuteInsertQuery() -3 opened");
                    //or set command.CommandText here instead of query above
                    int rowsAffected = command.ExecuteNonQuery();


                    command.Connection.Close();

                    return rowsAffected;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    throw e;
                }
            }

        }

        public override DataTable ExecuteSelectQuery(string query)
        {
            DataTable dt = new DataTable();
            _connection = new SqlConnection(base._connectionString);
            //BUG using _connection to be removed ?
                _connection.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(query, _connection))
                {
                    da.Fill(dt);
                }
                _connection.Close();
                _connection.Dispose();

            return dt;
        }

        public SqlConnection GetConnection()
        {
            _connection = new SqlConnection(base._connectionString);
            return _connection;
        }

    }
}