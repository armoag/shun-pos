using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Shun
{
    public class MySqlDatabase
    {
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
        }

        #endregion

        #region Methods

        public bool Read (string key)
        {
            try
            {
                var sqlCommand = @"SELECT * FROM Licenses";

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
            }
            catch (Exception e)
            {

            }

            return true;
        }

        public bool Write()
        {
            return true;
        }

        public bool Update()
        {
            return true;
        }

        public bool Remove()
        {
            return true;
        }

        public bool CheckConnection()
        {
            return true;
        }

        private bool OpenConnection()
        {
            return true;
        }

        private bool CloseConnection()
        {
            return true;
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
