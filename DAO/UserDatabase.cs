using MySql.Data.MySqlClient;
using SocketGameProtocal;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MultiplayerGameServer.DAO
{
    internal class UserDatabase : IDisposable
    {
        private MySqlConnection? connection;
        public readonly string connectionStr = "Server=localhost;Datbase=user;UserID=root;Password=Shiqikuangsan520;";

        public UserDatabase()
        {

        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        private MySqlConnection? ConnectMysql()
        {
            try
            {
                connection = new MySqlConnection(connectionStr);
                connection.Open();
                Console.WriteLine("数据库连接成功！");
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"数据库连接失败:{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="_pack"></param>
        /// <returns></returns>
        public bool SignUp(MainPack _pack)
        {
            string _username = _pack.LoginPack.Username;
            string _password = _pack.LoginPack.Password;

            string _salt = GenerateSalt();
            string _passwordHash = HashPassword(_password, _salt);

            if (GetUserByUsername(_username) is not null)
            {
                Console.Write("用户名已存在！");
                return false;
            }
            else
            {
                using (MySqlConnection? _connection = ConnectMysql())
                {
                    string _sql = "INSERT INTO users (user_name, user_password_hash, user_salt) VALUES(@_username, @_passwordHash, @user_salt)";
                    using (MySqlCommand cmd = new MySqlCommand(_sql, _connection))
                    {
                        cmd.Parameters.AddWithValue("@_username", _username);
                        cmd.Parameters.AddWithValue("@passwordHash", _passwordHash);
                        cmd.Parameters.AddWithValue("@passwordSalt", _salt);
                        return true;
                    }

                }
            }
        }

        /// <summary>
        /// 密码哈希生成
        /// </summary>
        /// <param name="_password"></param>
        /// <param name="_salt"></param>
        /// <returns></returns>
        private string HashPassword(string _password, string _salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string saltedPassword = _password + _salt;
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// 随机盐值生成
        /// </summary>
        /// <returns></returns>
        private string GenerateSalt()
        {
            byte[] _saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(_saltBytes);
            }
            return Convert.ToBase64String(_saltBytes);
        }

        /// <summary>
        /// 根据用户名查询用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public UserEntity? GetUserByUsername(string username)
        {
            using (MySqlConnection? _connection = ConnectMysql())
            {
                string _sql = "SELECT * FROM users WHERE user_name = @username";
                using (MySqlCommand cmd = new MySqlCommand(_sql, _connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapToUserEntity(reader);
                        }
                    }
                }
            }

            return null;
        }

        private UserEntity MapToUserEntity(MySqlDataReader reader)
        {
            return new UserEntity
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                UserName = reader["user_name"].ToString()!,
                IsActive = Convert.ToBoolean(reader["user_is_active"]),
                LastLoginDate = reader["user_last_login_time"] == DBNull.Value ? null : Convert.ToDateTime(reader["LastLoginTime"]),
                SignUpDate = Convert.ToDateTime(reader["user_signup_time"]),
                UpdateTime = Convert.ToDateTime(reader["user_update_time"])
            };
        }

        public void Dispose()
        {
            if (connection != null)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                connection.Dispose();
            }
        }
    }
}
