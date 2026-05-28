using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.DAO
{
    internal class DatabaseConnectionFactory
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        public MySqlConnection? ConnectMysql()
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"数据库连接失败:{ex.Message}");
                return null;
            }
        }
    }
}
