using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
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

        public bool CheckIfFirstTimeClient()
        {
            return true;

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

        public void BulkInsertFolders(DataTable dataTable)
        {
            _connection.Open();

            using (var cmd = new SQLiteCommand(_connection))
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        cmd.CommandText = string.Format("insert into Folders (id_folder, name_folder, parent_folder, created_by_client, is_synced) values({0}, {1}, {2}, {3}, {4})",
                            Convert.ToInt32(row["id_folder"]),
                            row["name_folder"],
                            Convert.ToInt32(row["parent_folder"]),
                            Convert.ToInt32(row["created_by_client"]),
                            1
                        );
                        cmd.ExecuteNonQuery();

                    }

                    transaction.Commit();
                }
            }


            _connection.Close();
        }

        public void BulkInsertFiles(DataTable dataTable)
        {
            _connection.Open();

            using (var cmd = new SQLiteCommand(_connection))
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        cmd.CommandText = string.Format("insert into Folders (id_folder, name_folder, parent_folder, created_by_client, is_synced) values({0}, {1}, {2}, {3}, {4})",
                            Convert.ToInt32(row["id_folder"]),
                            row["name_folder"],
                            Convert.ToInt32(row["parent_folder"]),
                            Convert.ToInt32(row["created_by_client"]),
                            1
                        );
                        cmd.ExecuteNonQuery();

                    }

                    transaction.Commit();
                }
            }


            _connection.Close();
        }



        public bool? IsNotEmpty(string tableName)
        {
            int rowsCount;
            string query = String.Format("SELECT COUNT(*) FROM {0}", tableName);
            using (var command = new SQLiteCommand(query, _connection))
            {
                try
                {
                    command.Connection.Open();
                    rowsCount = Convert.ToInt32(command.ExecuteScalar().ToString());
                    command.Connection.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
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



        public int ExecuteScalarQuery(string query)
        {
            int rowsCount;
            //string query = String.Format("SELECT COUNT(*) FROM {0}", tableName);
            using (var command = new SQLiteCommand(query, _connection))
            {
                try
                {
                    command.Connection.Open();
                    rowsCount = Convert.ToInt32(command.ExecuteScalar().ToString());
                    command.Connection.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return -1;
                }
            }

            return rowsCount;
        }


        public void ImportFromSqliteToSqlServer(SqlConnection destSqlConnection, DataTable dataTable)
        {
            using (SqlBulkCopy bc = new SqlBulkCopy(destSqlConnection))
            {
                bc.DestinationTableName = "Folders";
                destSqlConnection.Open();
                bc.WriteToServer(dataTable);
                destSqlConnection.Close();
            }
        }

        public int PurgeTable(string tableName)
        {
            using (var command = new SQLiteCommand("DELETE FROM " + tableName, _connection))
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