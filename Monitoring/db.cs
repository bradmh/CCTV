using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using MySql.Data.MySqlClient;
using MySqlConnector;
using Windows.Storage;

namespace Monitoring
{
    class Db
    {

        private static MySqlConnection CreateConnection()
        {
            var connString = String.Format("Server={0}; Port={1}; User ID={2};Password={3};Database={4}",
                                DbCreds.server, DbCreds.port, DbCreds.userid, DbCreds.password, DbCreds.dbName);
            MySqlConnection connection = new MySqlConnection(connString);
            return connection;
        }
        public static void User(string userName, string password = null, string operation = "Add", string column = "operator", string table = "operatorzy")
        {
            try
            {
                string passwordColumn = "hash";
                string pwd = @BCrypt.Net.BCrypt.HashPassword(@password);
                MySqlCommand cmd = new MySqlCommand();

                switch (operation.ToLower())
                {
                    case "add":
                        cmd.CommandText = String.Format("INSERT INTO {0}({1}, {2}) VALUES (@{1}, @{2})", table, column, passwordColumn);
                        cmd.Parameters.AddWithValue(column, @userName);
                        cmd.Parameters.AddWithValue(passwordColumn, @pwd);
                        break;
                    case "delete":
                        cmd.CommandText = String.Format(@"DELETE FROM {0} WHERE {1} = '{2}'", table, column, @userName);
                        break;
                    case "change":
                        cmd.CommandText = String.Format("UPDATE {0} SET {2} = '{3}' WHERE {1} = '{4}'", table, column, passwordColumn, @pwd, @userName);
                        break;
                    default:
                        throw new Exception("Possible operations for User method: Add/Delete/Change!");
                }

                MySqlConnection connection = CreateConnection();
                connection.Open();
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception(String.Format("Bład podczas przetwarzania {0} w kolumnie {1} w tabeli {2}", userName, column, table));
            }

        }
        public static void AddToSQLBulk(List<string> values, string table, string column)
        {
            try
            {
                MySqlCommand insertSQL = new MySqlCommand();
                insertSQL.CommandText = String.Format("INSERT INTO {0} ({1}) VALUES (@{1})", table, column);
                MySqlConnection connection = CreateConnection();
                connection.Open();
                insertSQL.Connection = connection;

                //Executing each paramether from values list
                foreach (string value in values)
                {
                    insertSQL.Parameters.Clear();
                    insertSQL.Parameters.AddWithValue(@column, value);
                    insertSQL.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception(String.Format("Bład podczas dodawania danych do kolumny {0} w tabeli {1}", column, table));
            }
        }
        public static void AddToSQL(string value, string table, string column)
        {
            try
            {
                MySqlCommand insertSQL = new MySqlCommand();
                insertSQL.CommandText = String.Format("INSERT INTO {0} ({1}) VALUES (@{1})", table, column);
                insertSQL.Parameters.AddWithValue(column, value);

                MySqlConnection connection = CreateConnection();
                connection.Open();
                insertSQL.Connection = connection;
                insertSQL.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception(String.Format("Bład podczas dodawania {0} do kolumny {1} w tabeli {2}", value, column, table));
            }

        }
        public static void CreateTables()
        {
            //Creates predefinied tables in DB
            try
            {
                List<string> tables = new List<string>
            {
                "create table zdarzenia (rodzaj_zdarzenia text, lokalizacja text, przekazanie text, operator varchar(60), data_godzina_zdarzenia datetime, kamera integer, zmiana integer, utworzone_data datetime)",
                "create table rodzaje_zdarzen (rodzaj_zdarzenia text)",
                "create table przekazanie (przekazanie text)",
                "create table operatorzy (operator text, hash text)",
                "create table kamery (kamery integer)"
            };
                foreach (string table in tables) CreateTable(table);
                void CreateTable(string commandText)
                {

                    MySqlConnection connection = CreateConnection();
                    MySqlCommand insertSQL = new MySqlCommand();
                    insertSQL.CommandText = commandText;
                    connection.Open();
                    insertSQL.Connection = connection;
                    insertSQL.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception("Błąd podczas tworzenia tabel w bazie danych");
            }

        }
        public static void RemoveFromSQL(string table, string column, string value)
        {
            try
            {
                MySqlCommand insertSQL = new MySqlCommand();
                if (!value.Equals("*")) insertSQL.CommandText = String.Format("DELETE FROM {0} WHERE {1} = '{2}'", @table, @column, @value);
                else insertSQL.CommandText = String.Format("DELETE FROM {0}", @table);

                MySqlConnection connection = CreateConnection();
                connection.Open();
                insertSQL.Connection = connection;
                insertSQL.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception(String.Format("Bład podczas usuwania {0} z kolumny {1} w tabeli {2}", value, column, table));
            }

        }
        public static bool ValidateUser(string userName, string password)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                connection.Open();
                string sql = String.Format("SELECT * FROM OPERATORZY WHERE OPERATOR='{0}'", userName);
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                MySqlDataReader rdr = cmd.ExecuteReader();
                {
                    rdr.Read();

                    if (BCrypt.Net.BCrypt.Verify(password, rdr["hash"].ToString()))
                    {
                        connection.Close();
                        return true;
                    }
                    connection.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception("Nie mogę zweryfikować użytkownika " + userName);
            }

        }
        public static ObservableCollection<string> ReadData(string table = "operatorzy", string field = "operator")
        {
            ObservableCollection<string> data = new ObservableCollection<string>();
            try
            {
                MySqlConnection connection = CreateConnection();
                connection.Open();
                string sql = String.Format("SELECT * FROM {0}", table);
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                MySqlDataReader rdr = cmd.ExecuteReader();
                {
                    while (rdr.Read())
                    {
                        data.Add(new string(rdr[field].ToString()));
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception(String.Format("Błąd podczas czytania danych. Tabela {0}, pole {1}.", table, field));
            }
            return data;
        }
        public static void ZdarzenieToSQL(Zdarzenie zdarzenie)
        {
            try
            {
                using (MySqlConnection connection = CreateConnection())
                {
                    using (MySqlCommand insertSQL = new MySqlCommand())
                    {
                        insertSQL.Connection = connection;
                        insertSQL.CommandText = @"INSERT INTO zdarzenia (rodzaj_zdarzenia, lokalizacja, przekazanie, operator, data_godzina_zdarzenia, kamera, zmiana, utworzone_data) VALUES (@rodzaj_zdarzenia, @lokalizacja, @przekazanie, @operator, @data_godzina_zdarzenia, @kamera, @zmiana, @utworzone_data)";
                        insertSQL.Parameters.AddWithValue("rodzaj_zdarzenia", zdarzenie.rodzaj_zdarzenia.ToString());
                        insertSQL.Parameters.AddWithValue("lokalizacja", zdarzenie.lokalizacja.ToString());
                        insertSQL.Parameters.AddWithValue("przekazanie", zdarzenie.przekazanie.ToString());
                        insertSQL.Parameters.AddWithValue("operator", zdarzenie.user.ToString());
                        insertSQL.Parameters.AddWithValue("data_godzina_zdarzenia", zdarzenie.data_godzina_zdarzenia.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertSQL.Parameters.AddWithValue("kamera", zdarzenie.kamera.ToString());
                        insertSQL.Parameters.AddWithValue("zmiana", zdarzenie.zmiana.ToString());
                        insertSQL.Parameters.AddWithValue("utworzone_data", zdarzenie.utworzone_data.ToString("yyyy-MM-dd HH:mm:ss"));


                        connection.Open();
                        insertSQL.Prepare();

                        insertSQL.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception("Błąd poczas dodawania zdarzenia do bazy!");
            }
        }
        public static ObservableCollection<Zdarzenie> ReadData_Zdarzenie(string table = "zdarzenia")
        {
            ObservableCollection<Zdarzenie> data = new ObservableCollection<Zdarzenie>();
            try
            {
                MySqlConnection connection = CreateConnection();
                connection.Open();
                string sql = String.Format("SELECT * FROM {0}", table);
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                MySqlDataReader rdr = cmd.ExecuteReader();
                {
                    while (rdr.Read())
                    {
                        data.Add(new Zdarzenie(DateTime.Parse(rdr["data_godzina_zdarzenia"].ToString()),
                            int.Parse(rdr["kamera"].ToString()),
                            int.Parse(rdr["zmiana"].ToString()),
                            rdr["rodzaj_zdarzenia"].ToString(),
                            rdr["przekazanie"].ToString(),
                            rdr["lokalizacja"].ToString(),
                            rdr["operator"].ToString(),
                            DateTime.Parse(rdr["utworzone_data"].ToString())));
                    }
                    connection.Close();
                }
            }
            catch
            {
                ToastCreator.CreateToast("Błąd poczas pobierania zdarzenia z bazy", "Błąd");
            }
            return data;
        }
    }

    static class DbCreds
    {
        public static void GetCreds()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            if (condition("server")) localSettings.Values["server"] = "127.0.0.1";
            if (condition("port")) localSettings.Values["port"] = "3306";
            if (condition("userid")) localSettings.Values["userid"] = "root";
            if (condition("password")) localSettings.Values["password"] = "asd";
            if (condition("dbName")) localSettings.Values["dbName"] = "monitoring";

            bool condition(string valueName)
            {
                if (localSettings.Values[valueName] == null || localSettings.Values[valueName].Equals("") || localSettings.Values[valueName].Equals(" ")) return true;
                return false;
            }

            var server_value = localSettings.Values["server"];
            var port_value = localSettings.Values["port"];
            var userid_value = localSettings.Values["userid"];
            var password_value = localSettings.Values["password"];
            var dbName_value = localSettings.Values["dbName"];

            server = server_value.ToString();
            port = port_value.ToString();
            userid = userid_value.ToString();
            password = password_value.ToString();
            dbName = dbName_value.ToString();
        }
        public static string server { get; set; }
        public static string port { get; set; }
        public static string userid { get; set; }
        public static string password { get; set; }
        public static string dbName { get; set; }
    }
}
