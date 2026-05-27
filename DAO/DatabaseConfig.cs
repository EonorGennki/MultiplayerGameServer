using System.Text.Json;

namespace MultiplayerGameServer.DAO
{
    internal class DatabaseConfig
    {
        public static string ConnectionString { get; private set; } = Initialize();

        public static string Initialize()
        {
            try
            {
                string jsonContent = File.ReadAllText("appsettings.json");
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
                return ConnectionString =
                    doc.RootElement
                    .GetProperty("ConnectionStrings")
                    .GetProperty("DefaultStrings")
                    .GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取连接字符串失败：{ex.Message}");
                return string.Empty;
            }
        }
    }
}
