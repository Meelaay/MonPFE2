using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Windows.Forms;

namespace MonPFE
{
    public class SQLiteConnector : Connector
    {
        private readonly SQLiteConnection _connection;

        public SQLiteConnector(string connectionString) : base(connectionString)
        {
            _connection = new SQLiteConnection(base._connectionString);
        }

        public override int ExecuteInsertQuery(string query)
        {
            //todo return int in execute query and raise exception != 1 and change command to execute query 
            //todo check insert query 

            using (var command = new SQLiteCommand(query, _connection))
            {
                try
                {
                    command.Connection.Open();
                    var rowsAffected = command.ExecuteNonQuery();
                    command.Connection.Close();
                    return rowsAffected;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return -1;
                    //throw...
                }
            }
        }

        public override DataTable ExecuteSelectQuery(string query)
        {
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, _connection))
            {
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                return dataTable;
            }

        }

        //make isempty aproperty and use logic in get
        public bool? IsNotEmpty()
        {
            int rowsCount;
            using (var command = new SQLiteCommand("SELECT COUNT(*) FROM localTable", _connection))
            {
                try
                {
                    command.Connection.Open();
                    rowsCount = Convert.ToInt32(command.ExecuteScalar().ToString());
                    command.Connection.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    rowsCount = -1;
                    return null;
                }
            }
            if (rowsCount > 0)
                return true;
            if (rowsCount == 0)
                return false;

            return null;
        }

        public void ImportFromSqliteToSqlServer(SqlConnection destSqlConnection)
        {
            using (var command = new SQLiteCommand("SELECT * FROM localTable", _connection))
            {
                try
                {
                    command.Connection.Open();

                    using (SQLiteDataReader dr = command.ExecuteReader())
                    {
                        
                        using (destSqlConnection)
                        {
                            using (SqlBulkCopy bc = new SqlBulkCopy(destSqlConnection))
                            {
                                bc.DestinationTableName = "onlineTable";
                                destSqlConnection.Open();
                                bc.WriteToServer(dr);
                                
                                destSqlConnection.Close();
                            }
                        }
                    }
                    command.Connection.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

        }

        public int PurgeTable()
        {
            using (var command = new SQLiteCommand("DELETE FROM localTable", _connection))
            {
                try
                {
                    command.Connection.Open();
                    var rowsAffected = command.ExecuteNonQuery();
                    command.Connection.Close();
                    return rowsAffected;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return -1;
                    //throw...
                }
            }
        }
    }
}