using MultiplayerGameServer.DAO;
using MultiplayerGameServer.Tool;
using System.Security.Cryptography;
using System.Text;

namespace MultiplayerGameServer.Service
{
    internal class UserService : BaseService
    {
        public UserService(Database database) : base(database)
        {
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Register(string username, string password)
        {
            string salt = GenerateSalt();
            string passwordHash = HashPassword(password, salt);

            if (database.GetUserByUsername(username) is not null)
            {
                return false;
            }
            else
            {
                database.InsertUser(username, passwordHash, salt);
            }

            return false;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Login(string username, string password)
        {
            UserEntity? user = database.GetUserByUsername(username);

            if (user is not null)
            {
                string passwordHash = HashPassword(password, user.Salt);
                if (passwordHash == user.PasswordHash)
                {
                    return true;
                }
            }

            return false;
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
    }
}
