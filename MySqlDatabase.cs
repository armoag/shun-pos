using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

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
        private string _table;
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

        public string Table
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

        public MySqlDatabase(string server, string database, string table, string userId, string password)
        {
            Server = server;
            Database = database;
            UserId = userId;
            Password = password;
            Table = table;
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
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
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

        /// <summary>
        /// Select based on single column key and value pair
        /// </summary>
        /// <param name="colKey"></param>
        /// <param name="value"></param>
        /// <param name="keyColNameValuePair"></param>
        /// <returns></returns>
        public bool Read (string colKey, string value, out List<Tuple<string, List<Tuple<string, string>>>> keyColNameValuePair)
        {
            try
            {
                //var sqlCommand = @"SELECT * FROM Licenses";
                var query = @"SELECT * FROM " + Table + " WHERE ";
                query = string.Concat(query, colKey, "='", value, "'");

                //list of key, list of colname, colvalue tuples
                var keyNameValue = new List<Tuple<string, List<Tuple<string, string>>>>();

                //Open connection
                if (this.OpenConnection(out var errorMessage) == true)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, Connection))
                    {
                        using (MySqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                var nameValue = new List<Tuple<string, string>>();
                                var colCount = dataReader.FieldCount;

                                //get all column names and values
                                for (var i = 0; i < colCount; i++)
                                {
                                    if (dataReader.GetName(i).Contains("Fecha"))
                                    {
                                        nameValue.Add(new Tuple<string, string>(dataReader.GetName(i), 
                                            DateTime.Parse(dataReader[i].ToString()).ToString(CultureInfo.CurrentCulture)));
                                        continue;
                                    }
                                    nameValue.Add(new Tuple<string, string>(dataReader.GetName(i), dataReader[i].ToString()));
                                }
                                keyNameValue.Add(new Tuple<string, List<Tuple<string, string>>>(value, nameValue));
                            }
                        }
                    }
                    //close connection
                    this.CloseConnection(out errorMessage);
                }

                keyColNameValuePair = keyNameValue; 
                return true;
            }
            catch (Exception e)
            {
                this.CloseConnection(out var errorMessage);
                keyColNameValuePair = null;
                return false;
            }
        }

        /// <summary>
        /// Select based on multiple column key and value pairs
        /// </summary>
        /// <param name="colValPairs"></param>
        /// <param name="keyColNameValuePair"></param>
        /// <returns></returns>
        public void Read(List<Tuple<string, string>> colValPairs, out List<Tuple<string, List<Tuple<string, string>>>> keyColNameValuePair)
        {

            //WHERE CustomerName LIKE 'a%'	Finds any values that start with "a"
            //WHERE CustomerName LIKE '%a'    Finds any values that end with "a"
            //WHERE CustomerName LIKE '%or%'  Finds any values that have "or" in any position
            try
            {
                string query = "SELECT * FROM " + Table + " WHERE ";

                foreach (var pair in colValPairs)
                {
                    query = string.Concat(query, pair.Item1, " LIKE '", "%", pair.Item2, "%'", " AND ");
                }

                //remove AND
                query = query.Remove(query.Length - 5, 5);

                //var sqlCommand = @"SELECT * FROM Licenses";
                //   var query = @"SELECT * FROM " + Database + " WHERE ";
                //  string.Concat(query, colKey, "='", value, "'");

                //list of key, list of colname, colvalue tuples
                var keyNameValue = new List<Tuple<string, List<Tuple<string, string>>>>();

                //Open connection
                if (this.OpenConnection(out var errorMessage) == true)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, Connection))
                    {
                        using (MySqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                var nameValue = new List<Tuple<string, string>>();
                                var colCount = dataReader.FieldCount;
                                //get all column names and values
                                for (var i = 0; i < colCount; i++)
                                {
                                    if (colValPairs[i].Item1.Contains("Fecha"))
                                    {
                                        nameValue.Add(new Tuple<string, string>(dataReader.GetName(i),
                                            DateTime.Parse(dataReader[i].ToString()).ToString(CultureInfo.CurrentCulture)));
                                        continue;
                                    }
                                    nameValue.Add(new Tuple<string, string>(dataReader.GetName(i),
                                        dataReader[i].ToString()));
                                }
                                keyNameValue.Add(new Tuple<string, List<Tuple<string, string>>>(colValPairs.First().Item2, nameValue));
                            }
                        }
                    }
                    //close connection
                    this.CloseConnection(out errorMessage);
                }

                keyColNameValuePair = keyNameValue;
            }
            catch (Exception e)
            {
                this.CloseConnection(out var errorMessage);
                keyColNameValuePair = null;
            }
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

            try
            {
                string query = "INSERT INTO " + Table + " (";

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
                    }

                    ;
                    //close connection
                    this.CloseConnection(out errorMessage);
                }
            }
            catch (Exception e)
            {
                this.CloseConnection(out var errorMessage);
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

            try
            {
                //string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";
                string query = "UPDATE " + Table + " SET ";

                foreach (var pair in colValPairs)
                {
                    query = string.Concat(query, pair.Item1, "='", pair.Item2, "', ");
                }

                //remove last comma, quote, and space
                query = query.Remove(query.Length - 2, 2);
                //identifier selector
                query = string.Concat(query, " WHERE ", identifierCol, "='", identifierValue, "'");

                //Open connection
                if (this.OpenConnection(out var errorMessage) == true)
                {
                    //create command and assign the query and connection from the constructor
                    using (MySqlCommand cmd = new MySqlCommand(query, Connection))
                    {
                        //Execute command
                        cmd.ExecuteNonQuery();
                    }

                    ;
                    //close connection
                    this.CloseConnection(out errorMessage);
                }
            }
            catch (Exception e)
            {
                this.CloseConnection(out var errorMessage);

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

            string query = "DELETE FROM " + Table + " WHERE ";
            query = string.Concat(query, identifierCol, "='", identifierValue, "'");

            try
            {
                //Open connection
                if (this.OpenConnection(out var errorMessage) == true)
                {
                    //create command and assign the query and connection from the constructor
                    using (MySqlCommand cmd = new MySqlCommand(query, Connection))
                    {
                        //Execute command
                        cmd.ExecuteNonQuery();
                    }

                    ;
                    //close connection
                    this.CloseConnection(out errorMessage);
                }
            }
            catch (Exception e)
            {
                this.CloseConnection(out var errorMessage);

            }

        }

        /// <summary>
        /// Delete mysql statement
        /// </summary>
        /// <param name="query"></param>
        public void Delete(string query)
        {
            try
            {
                //Open connection
                if (this.OpenConnection(out var errorMessage) == true)
                {
                    //create command and assign the query and connection from the constructor
                    using (MySqlCommand cmd = new MySqlCommand(query, Connection))
                    {
                        //Execute command
                        cmd.ExecuteNonQuery();
                    }

                    ;
                    //close connection
                    this.CloseConnection(out errorMessage);
                }
            }
            catch (Exception e)
            {
                this.CloseConnection(out var errorMessage);
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

        //Select statement
        public DataTable SelectAll(List<string> colNames)
        {
            var dataTable = new DataTable();

            //Create columns
            foreach (var colName in colNames)
            {
                var column = new DataColumn()
                {
                    DataType = System.Type.GetType("System.String"),
                    ColumnName = colName
                };
                dataTable.Columns.Add(column);
            }

            //Search for all data in db
            string query = "SELECT * FROM " + Table;

            ////Create a list to store the result
            //List<string>[] list = new List<string>[3];
            //list[0] = new List<string>();
            //list[1] = new List<string>();
            //list[2] = new List<string>();

            //Open connection
            if (this.OpenConnection(out var errorMessage) == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(query, Connection))
                {
                    using (MySqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        while(dataReader.Read())
                        {
//                          var z = dataReader.GetDataTypeName(0);
//                          z = dataReader.GetDataTypeName(1);
//                          var ab = dataReader.FieldCount;
//                          var ac = dataReader.GetName(1);
                            var colCount = dataReader.FieldCount;
                            var row = dataTable.NewRow();
                            for (var i = 0; i < colCount; i++)
                            {
                                try
                                {
                                    if (colNames[i].Contains("Fecha"))
                                    {

                                        var datestring = dataReader[colNames[i]].ToString();
                                        row[colNames[i]] = DateTime.Parse(datestring)
                                            .ToString(CultureInfo.CurrentCulture);
                                        continue;
                                    }

                                    row[colNames[i]] = dataReader[colNames[i]].ToString();
                                }
                                catch (Exception e)
                                {
                                    errorMessage = "Error in reading col " + i.ToString();
                                    row[colNames[i]] = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                                }
                            }
                            dataTable.Rows.Add(row);

    //                        var x = dataReader.GetString("LicenseKey");
  //                          var license = dataReader["LicenseKey"].ToString();
  //                          var currentUser = dataReader["CurrentUser"].ToString();

//                            var y = dataReader.GetDateTime("ExpirationDate");
                        }
                    }
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Get the number of entries found
        /// </summary>
        /// <returns></returns>
        private int Count()
        {
            string query = "SELECT Count(*) FROM " + Table;
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
            ///TODO Need to me impelemented
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
           
            //try
            //{
            //    //Read file from C:\
            //    string path;
            //    path = "C:\\MySqlBackup.sql";
            //    StreamReader file = new StreamReader(path);
            //    string input = file.ReadToEnd();
            //    file.Close();

            //    ProcessStartInfo psi = new ProcessStartInfo();
            //    psi.FileName = "mysql";
            //    psi.RedirectStandardInput = true;
            //    psi.RedirectStandardOutput = false;
            //    psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
            //        UserId, Password, Server, Database);
            //    psi.UseShellExecute = false;

            //    Process process = Process.Start(psi);
            //    process.StandardInput.WriteLine(input);
            //    process.StandardInput.Close();
            //    process.WaitForExit();
            //    process.Close();
            //    errorMessage = string.Empty;
            //}
            //catch (IOException ex)
            //{
            //    errorMessage = "Error , unable to Restore!";
            //}
            errorMessage = "";
        }
        #endregion
    }
}