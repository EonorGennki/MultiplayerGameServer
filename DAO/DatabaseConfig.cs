using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace MultiplayerGameServer.DAO
{
    internal class DatabaseConfig
    {
        public static string ConnectionString { get; private set; } = Initialize();

        public static string Initialize()
        {
            try
            {
                var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

                var dbConfig = config.GetSection("Database").Get<DatabaseOptions>()!;

                var bulider = new MySqlConnectionStringBuilder
                {
                    Server = dbConfig.Server,
                    Database = dbConfig.Database,
                    UserID = dbConfig.UserId,
                    Password = dbConfig.Password
                };

                return bulider.ConnectionString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取连接字符串失败：{ex.Message}");
                return string.Empty;
            }
        }
    }
}
