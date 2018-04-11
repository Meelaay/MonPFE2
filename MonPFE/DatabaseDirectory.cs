using System;
using System.Collections.Generic;
using System.Data;



namespace MonPFE
{
    public class DatabaseDirectory
    {
        public int id_folder;
        public string name_Folder;
        public int parent_folder;

        private static SQLServerConnector SqlServerConnector;
        private static SQLiteConnector SqlLiteConnector;

        public List<DatabaseDirectory> FoldersList = new List<DatabaseDirectory>();
        public List<DatabaseFile> FilesList = new List<DatabaseFile>();

        public DatabaseDirectory(int idFolder, string nameFolder, int parentFolder = 0)
        {
            id_folder = idFolder;
            name_Folder = nameFolder;
            parent_folder = parentFolder;
        }

        public static void SetConnectors(SQLServerConnector sqlServerConnector = null, SQLiteConnector sqlLiteConnector = null)
        {
            DatabaseDirectory.SqlServerConnector = sqlServerConnector;
            DatabaseDirectory.SqlLiteConnector = sqlLiteConnector;
        }

        public DatabaseDirectory()
        {
            SqlServerConnector = 
                new SQLServerConnector(@"Data Source=DESKTOP-DJONOS6\SQLEXPRESS;Initial Catalog=PFE;Integrated Security=True;Connection Timeout = 5");

            SqlLiteConnector = 
                new SQLiteConnector(@"Data source = ..\..\localSqlite\localDatabase.db;Version=3;");
        }
        

        public DatabaseDirectory CreateNode(DataRow nodeRow, Connector connector)
        {
            var dirNode = new DatabaseDirectory(
                Convert.ToInt32(nodeRow["id_folder"]),
                nodeRow["name_folder"].ToString(),
                Convert.ToInt32(nodeRow["parent_folder"])
            );

            var childFoldersDataTable = connector.ExecuteSelectQuery(
                string.Format("select * from Folders where parent_folder = {0}", nodeRow["id_folder"])
            );
            var childFilesDataTable = connector.ExecuteSelectQuery(
                string.Format("select * from Files where parent_folder = {0}", nodeRow["id_folder"])
            );

            foreach (DataRow row in childFoldersDataTable.Rows)
                dirNode.FoldersList.Add(CreateNode(row, connector));

            foreach (DataRow row in childFilesDataTable.Rows)
                dirNode.FilesList.Add(DatabaseDirectory.AddFile(row));

            return dirNode;
             
        }

        private static DatabaseFile AddFile(DataRow fileRow)
        {
            DatabaseFile df = new DatabaseFile(
                Convert.ToInt32(fileRow["id_file"]),
                fileRow["name_file"].ToString(),
                fileRow["path_file"].ToString(),
                Convert.ToInt32(fileRow["parent_folder"])
                );

            return df;
        }

        public List<DatabaseDirectory> GetDirectories() { return FoldersList; }

        public List<DatabaseFile> GetFiles() { return FilesList; }
    }

    public class DatabaseFile
    {
        public int id_file;
        public string name_file;
        public string path_file;
        public int parent_folder;

        public DatabaseFile(int idFile, string nameFile, string pathFile, int parentFolder)
        {
            id_file = idFile;
            name_file = nameFile;
            path_file = pathFile;
            parent_folder = parentFolder;
        }


        public DatabaseFile()
        {
            //use connector to select file from
        }

    }
}