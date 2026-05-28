using MultiplayerGameServer.Tool;
using MySql.Data.MySqlClient;
using SocketGameProtocal;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MultiplayerGameServer.DAO
{
    internal class UserDatabase
    {
        public readonly DatabaseConnectionFactory factory;

        public UserDatabase(DatabaseConnectionFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public bool SignUp(MainPack pack)
        {
            string username = pack.LoginPack.Username;
            string password = pack.LoginPack.Password;

            string salt = GenerateSalt();
            string passwordHash = HashPassword(password, salt);

            if (GetUserByUsername(username) is not null)
            {
                Console.WriteLine("用户名已存在！");
                return false;
            }
            else
            {
                using (MySqlConnection? connection = factory.ConnectMysql())
                {
                    string _sql = "INSERT INTO users (user_name, user_password_hash, user_salt) VALUES(@_username, @_passwordHash, @_salt)";
                    using (MySqlCommand cmd = new MySqlCommand(_sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@_username", username);
                        cmd.Parameters.AddWithValue("@_passwordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@_salt", salt);
                        cmd.ExecuteNonQuery();
                        return true;
                    }

                }
            }
        }

        /// <summary>
        /// 密码哈希生成
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
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
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// 根据用户名查询用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public UserEntity? GetUserByUsername(string username)
        {
            using (MySqlConnection? connection = factory.ConnectMysql())
            {
                string _sql = "SELECT * FROM users WHERE user_name = @username";
                using (MySqlCommand cmd = new MySqlCommand(_sql, connection))
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

        /// <summary>
        /// 获取user对象
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private UserEntity MapToUserEntity(MySqlDataReader reader)
        {
            return new UserEntity
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                UserName = reader["user_name"].ToString()!,
                IsActive = Convert.ToBoolean(reader["user_is_active"]),
                LastLoginDate = reader["user_last_login_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["user_last_login_time"]),
                SignUpDate = Convert.ToDateTime(reader["user_signup_date"]),
                UpdateTime = Convert.ToDateTime(reader["user_update_date"])
            };
        }
    }
}
