using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Shun
{
    public class MySqlDatabase
    {
        private MySqlConnection _connection;
        private string _server;
        private string _database;
        private string _userId;
        private string _password;
        private MySqlConnectionStringBuilder _connectionString;

        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public MySqlConnection Connection
        {
            get
            {
                return _connection;
            }
            set { _connection = value; }
        }

        public MySqlConnectionStringBuilder ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        #region Properties


        #endregion

        #region Constructors

        public MySqlDatabase(string server, string database, string userId, string password)
        {
            Server = server;
            Database = database;
            UserId = userId;
            Password = password;
            ConnectionString = new MySqlConnectionStringBuilder
            {
                Server = server,
                Database = database,
                UserID = userId,
                Password = password
            };

            Connection = new MySqlConnection(ConnectionString.ToString());
        }

        #endregion

        #region Methods

        public bool Read (string key)
        {
            try
            {
                //var sqlCommand = @"SELECT * FROM Licenses";
                var sqlCommand = @"SELECT " + key + " FROM " + Database;

                using (MySqlConnection conn = new MySqlConnection(ConnectionString.ToString()))
                {
                    conn.Open();
                    var schema = conn.GetSchema();
                    using (MySqlCommand cmd = new MySqlCommand(sqlCommand, conn))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                var z = dr.GetDataTypeName(0);
                                z = dr.GetDataTypeName(1);
                                z = dr.GetDataTypeName(2);
                                z = dr.GetDataTypeName(3);
                                z = dr.GetDataTypeName(4);
                                z = dr.GetDataTypeName(5);
                                z = dr.GetDataTypeName(6);
                                z = dr.GetDataTypeName(7);
                                var ab = dr.FieldCount;
                                var ac = dr.GetName(1);

                                var x = dr.GetString("LicenseKey");
                                var license = dr["LicenseKey"].ToString();
                                var currentUser = dr["CurrentUser"].ToString();

                                var y = dr.GetDateTime("ExpirationDate");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return true;
        }

        //Initialize values
        private void Initialize()
        {

        }

        //Insert statement
        public void Insert()
        {

            string query = "INSERT INTO " + Database + " (LicenseKey, CurrentUser) VALUES ('1234567890abc', 'TestUser2')";
            //open connection
            if (this.OpenConnection(out var errorMessage) == true)
            {
                //create command and assign the query and connection from the constructor
                using (MySqlCommand cmd = new MySqlCommand(query, Connection))
                {
                    //Execute command
                    cmd.ExecuteNonQuery();
                };
                //close connection
                this.CloseConnection(out errorMessage);
            }
        }

        //Insert statement
        public void Insert(List<Tuple<string, string>> colValPairs)
        {
            string query = "INSERT INTO " + Database + " (";

            foreach (var pair in colValPairs)
            {
                query = string.Concat(query, pair.Item1, ", ");
            }

            //remove last comma and space
            query = query.Remove(query.Length - 2, 2);
            query = query + ") VALUES (";

            foreach (var pair in colValPairs)
            {
                string temp = "'" + pair.Item2 + "'";
                query = string.Concat(query, temp, ", ");
            }
            //remove last comma and space
            query = query.Remove(query.Length - 2, 2);
            query = query + ")";

 //           string query2 = "INSERT INTO " + Database + " (LicenseKey, CurrentUser) VALUES ('1234567890abc', 'TestUser2')";

            //open connection
            if (this.OpenConnection(out var errorMessage) == true)
            {
                //create command and assign the query and connection from the constructor
                using (MySqlCommand cmd = new MySqlCommand(query, Connection))
                {
                    //Execute command
                    cmd.ExecuteNonQuery();
                };
                //close connection
                this.CloseConnection(out errorMessage);
            }
        }

        //Update statement
        public void Update()
        {
            string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";

            //Open connection
            if (this.OpenConnection(out var errorMessage) == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = Connection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection(out errorMessage);
            }
        }
        
        //Delete statement
        public void Delete()
        {
            string query = "DELETE FROM tableinfo WHERE name='John Smith'";

            if (this.OpenConnection(out var errorMessage) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection(out errorMessage);
            }
        }

        //Select statement
        public List<string>[] Select()
        {
            string query = "SELECT * FROM tableinfo";

            //Create a list to store the result
            List<string>[] list = new List<string>[3];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();

            //Open connection
            if (this.OpenConnection(out var errorMessage) == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["id"] + "");
                    list[1].Add(dataReader["name"] + "");
                    list[2].Add(dataReader["age"] + "");
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection(out errorMessage);

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        //Open connection to dababase
        private bool OpenConnection(out string errorMessage)
        {
            try
            {
                Connection.Open();
                errorMessage = string.Empty;
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        errorMessage = "Cannot connect to server.  Contact administrator";
                        break;

                    case 1045:
                        errorMessage = "Invalid username/password, please try again";
                        break;
                    default:
                        errorMessage = errorMessage = "Error opening connection";
                        break;

                }
                return false;
            }
        }

        //Close connection to dababase
        private bool CloseConnection(out string errorMessage)
        {
            try
            {
                Connection.Close();
                errorMessage = string.Empty;
                return true;
            }
            catch (MySqlException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        //Count statement
        private int Count()
        {
            string query = "SELECT Count(*) FROM tableinfo";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection(out var erorMessage) == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, Connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection(out erorMessage);

                return Count;
            }
            else
            {
                return Count;
            }
        }

        //Backup
        public void Backup(out string errorMessage)
        {
            try
            {
                DateTime Time = DateTime.Now;
                int year = Time.Year;
                int month = Time.Month;
                int day = Time.Day;
                int hour = Time.Hour;
                int minute = Time.Minute;
                int second = Time.Second;
                int millisecond = Time.Millisecond;

                //Save file to C:\ with the current date as a filename
                string path;
                path = "C:\\MySqlBackup" + year + "-" + month + "-" + day +
                       "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                StreamWriter file = new StreamWriter(path);
                
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
                    UserId, Password, Server, Database);
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output;
                output = process.StandardOutput.ReadToEnd();
                file.WriteLine(output);
                process.WaitForExit();
                file.Close();
                process.Close();
                errorMessage = string.Empty;
            }
            catch (IOException ex)
            {
                errorMessage = "Error , unable to backup!";
            }
        }

        //Restore
        public void Restore(out string errorMessage)
        {
            try
            {
                //Read file from C:\
                string path;
                path = "C:\\MySqlBackup.sql";
                StreamReader file = new StreamReader(path);
                string input = file.ReadToEnd();
                file.Close();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysql";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
                    UserId, Password, Server, Database);
                psi.UseShellExecute = false;


                Process process = Process.Start(psi);
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
                errorMessage = string.Empty;
            }
            catch (IOException ex)
            {
                errorMessage = "Error , unable to Restore!";
            }
        }
        #endregion

        //Testing
        //try
        //{
        //    var conn = new MySqlConnection(@"Server=wibsarlicencias.csqn2onotlww.us-east-1.rds.amazonaws.com;Database=Licenses;Uid=armoag;Pwd=Yadira00;");
        //    conn.Open();

        //    string sql = @"SELECT LicenseKey, CurrentUser FROM Licenses WHERE idLicenses=2";

        //    var cmd = new MySqlCommand(sql, conn);

        //    MySqlDataReader reader = cmd.ExecuteReader();
        //    if (reader.Read())
        //    {
        //        var license = reader["LicenseKey"].ToString();
        //        var currentUser = reader["CurrentUser"].ToString();
        //    }

        //    sql = @"SELECT LicenseKey, CurrentUser FROM Licenses WHERE idLicenses=1";

        //    var cmd2 = new MySqlCommand(sql, conn);
        //    //         MySqlDataReader reader = cmd.ExecuteReader();
        //    if (reader.Read())
        //    {
        //        var license = reader["LicenseKey"].ToString();
        //        var currentUser = reader["CurrentUser"].ToString();
        //    }

        //    conn.Close();
        //}
        //catch (Exception e)
        //{
        //    var x = 1;
        //}


    }
}


//var test = new MySqlConnectionStringBuilder();
//test.Server = "wibsarlicencias.csqn2onotlww.us-east-1.rds.amazonaws.com";
//test.Database = "Licenses";
//test.UserID = "armoag";
//test.Password = "Yadira00";


//    var connTest = new MySqlConnection(ConnectionString.ToString());
//connTest.Open();

////SELECT LicenseKey, CurrentUser FROM Licenses WHERE idLicenses=2
//string sqlTest = @"SELECT * FROM Licenses";

////MySqlCommand can be reused but the execute reader cannot be called twice, it needs to be closed before

//var cmdTest = new MySqlCommand(sqlTest, connTest);
//var readerTest = cmdTest.ExecuteReader();
//if (readerTest.Read())
//{
//   var z = readerTest.GetDataTypeName(0);
//    z = readerTest.GetDataTypeName(1);
//    z = readerTest.GetDataTypeName(2);
//    z = readerTest.GetDataTypeName(3);
//    z = readerTest.GetDataTypeName(4);
//    z = readerTest.GetDataTypeName(5);
//    z = readerTest.GetDataTypeName(6);
//    z = readerTest.GetDataTypeName(7);
//    var ab = readerTest.FieldCount;
//    var ac = readerTest.GetName(1);

//    var x = readerTest.GetString("LicenseKey");
//    var license = readerTest["LicenseKey"].ToString();
//    var currentUser = readerTest["CurrentUser"].ToString();

//    var y = readerTest.GetDateTime("ExpirationDate");
//}

//readerTest.Close();

//connTest.Close();
//connTest.Dispose();

//var conn = new MySqlConnection(@"Server=wibsarlicencias.csqn2onotlww.us-east-1.rds.amazonaws.com;Database=Licenses;Uid=armoag;Pwd=Yadira00;");
//conn.Open();

//string sql = @"SELECT LicenseKey, CurrentUser FROM Licenses WHERE idLicenses=2";

//var cmd = new MySqlCommand(sql, conn);

//MySqlDataReader reader = cmd.ExecuteReader();
//if (reader.Read())
//{
//    var license = reader["LicenseKey"].ToString();
//    var currentUser = reader["CurrentUser"].ToString();
//}

//sql = @"SELECT LicenseKey, CurrentUser FROM Licenses WHERE idLicenses=1";

//var cmd2 = new MySqlCommand(sql, conn);
////         MySqlDataReader reader = cmd.ExecuteReader();
//if (reader.Read())
//{
//    var license = reader["LicenseKey"].ToString();
//    var currentUser = reader["CurrentUser"].ToString();
//}

//conn.Close();