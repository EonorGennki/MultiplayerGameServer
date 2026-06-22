using MySql.Data.MySqlClient;

namespace MultiplayerGameServer.DAO
{
    internal class Database
    {
        public readonly DatabaseConnectionFactory factory;

        public Database(DatabaseConnectionFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwordHash"></param>
        /// <param name="salt"></param>
        public void InsertUser(string username, string passwordHash, string salt)
        {
            using (MySqlConnection? connection = factory.ConnectMysql())
            {
                string sql = "INSERT INTO users (user_name, user_password_hash, user_salt) VALUES(@username, @passwordHash, @salt)";
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertPlayer(long playerId, int userId)
        {
            try
            {
                using (MySqlConnection? connection = factory.ConnectMysql())
                {
                    string sql = "INSERT INTO players (player_id, user_id) VALUES(@playerId, @userId)";
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@playerId", playerId);
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
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
                string sql = "SELECT * FROM users WHERE user_name = @username";
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
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
        /// 根据用户id查询用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserEntity? GetUserByUserId(int userId)
        {
            try
            {
                using (MySqlConnection? connection = factory.ConnectMysql())
                {
                    string sql = "SELECT * FROM users WHERE user_id = @userId";
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
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
            catch (MySqlException)
            {
                return new UserEntity { UserId = -1 };
            }
            catch (Exception)
            {
                return new UserEntity { UserId = -2 };
            }
        }

        /// <summary>
        /// 查询用户拥有的角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void GetPlayers(UserEntity user)
        {
            if (user is null)
            {
                return;
            }

            try
            {
                using (MySqlConnection? connection = factory.ConnectMysql())
                {
                    string sql = "SELECT * FROM players WHERE user_id = @userId";
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", user.UserId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PlayerEntity playerEntity = new PlayerEntity
                                {
                                    PlayerId = Convert.ToInt64(reader["player_id"]),
                                    isActive = Convert.ToBoolean(reader["player_is_active"])
                                };

                                user.Players.Add(playerEntity);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
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
                PasswordHash = reader["user_password_hash"].ToString()!,
                Salt = reader["user_salt"].ToString()!,
                IsActive = Convert.ToBoolean(reader["user_is_active"]),
                LastLoginDate = reader["user_last_login_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["user_last_login_time"]),
                SignUpDate = Convert.ToDateTime(reader["user_signup_date"]),
                UpdateTime = Convert.ToDateTime(reader["user_update_date"])
            };
        }

        /// <summary>
        /// 设置用户活跃状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isActive"></param>
        public void SetUserActive(int userId, bool isActive)
        {
            using (MySqlConnection? connection = factory.ConnectMysql())
            {
                string sql = "UPDATE users SET user_is_active = @isActive WHERE user_id = @userId";
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@isActive", isActive);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 设置玩家角色激活状态
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="isActive"></param>
        public void SetPlayerActive(long playerId, bool isActive)
        {
            using (MySqlConnection? connection = factory.ConnectMysql())
            {
                string sql = "UPDATE players SET player_is_active = @isActive WHERE player_id = @playerId";
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@isActive", isActive);
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
