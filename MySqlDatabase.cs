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
        #region Fields

        private MySqlConnection _connection;
        private string _server;
        private string _database;
        private string _userId;
        private string _password;
        private MySqlConnectionStringBuilder _connectionString;

        #endregion

        #region Properties

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

        /// <summary>
        /// Open connection to mysql db
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Close connection to mysql db
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Insert mysql statement
        /// </summary>
        /// <param name="query"></param>
        public void Insert(string query)
        {
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

        /// <summary>
        /// Insert mysql statement with column data pairs
        /// </summary>
        /// <param name="colValPairs"></param>
        public void Insert(List<Tuple<string, string>> colValPairs)
        {
            if (colValPairs == null) return;

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

            //Open connection
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

        /// <summary>
        /// Update mysql statement
        /// </summary>
        /// <param name="identifierCol"></param>
        /// <param name="identifierValue"></param>
        /// <param name="colValPairs"></param>
        public void Update(string identifierCol, string identifierValue, List<Tuple<string, string>> colValPairs)
        {
            if (colValPairs == null || identifierCol == string.Empty || identifierValue == string.Empty) return;

            //string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";
            string query = "UPDATE " + Database + " SET ";

            foreach (var pair in colValPairs)
            {
                query = string.Concat(query, pair.Item1, "='", pair.Item2, "', ");
            }
            //remove last comma, quote, and space
            query = query.Remove(query.Length - 3, 3);
            //identifier selector
            query = string.Concat(" WHERE ", identifierCol, "='", identifierValue, "'");

            //Open connection
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

        /// <summary>
        /// Update mysql statement
        /// </summary>
        /// <param name="query"></param>
        public void Update(string query)
        {
            //Open connection
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

        /// <summary>
        /// Delete mysql statement
        /// </summary>
        /// <param name="identifierCol"></param>
        /// <param name="identifierValue"></param>
        public void Delete(string identifierCol, string identifierValue)
        {
            if (identifierCol == string.Empty || identifierValue == string.Empty) return;

            string query = "DELETE FROM " + Database + " WHERE ";
            query = string.Concat(identifierCol, "='", identifierValue, "'");

            //Open connection
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

        /// <summary>
        /// Delete mysql statement
        /// </summary>
        /// <param name="query"></param>
        public void Delete(string query)
        {
            //Open connection
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

        /// <summary>
        /// Get the number of entries found
        /// </summary>
        /// <returns></returns>
        private int Count()
        {
            string query = "SELECT Count(*) FROM " + Database;
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

        /// <summary>
        /// Restore DB based on .sql file
        /// </summary>
        /// <param name="errorMessage"></param>
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