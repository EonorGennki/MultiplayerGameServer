using MultiplayerGameServer.DAO;
using MultiplayerGameServer.Logic.Interface;
using System.Security.Cryptography;
using System.Text;

namespace MultiplayerGameServer.Logic.Service
{
    internal class UserService : BaseService, IUserService
    {
        private Database database;

        public UserService(Database database) : base()
        {
            this.database = database;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ServiceResult Register(string username, string password)
        {
            string salt = GenerateSalt();
            string passwordHash = HashPassword(password, salt);

            if (database.GetUserByUsername(username) is not null)
            {
                return ServiceResult.Failure(ServiceErrorCode.AlreadyExists);
            }

            try
            {
                database.InsertUser(username, passwordHash, salt);
                return ServiceResult.Success();
            }
            catch
            {
                return ServiceResult.Failure(ServiceErrorCode.DatabaseError);
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ServiceResult Login(string username, string password)
        {
            UserEntity? user = database.GetUserByUsername(username);
            if (user == null)
            {
                return ServiceResult.Failure(ServiceErrorCode.DatabaseError);
            }

            string passwordHash = HashPassword(password, user.Salt);
            if (passwordHash != user.PasswordHash)
            {
                return ServiceResult.Failure(ServiceErrorCode.InvalidPassword);
            }

            database.SetActive(user.UserId, true);
            ServiceResult result = ServiceResult.Success();
            result.Data = user.UserId;
            return result;
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
        /// 获取用户名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PlayerInfo GetPlayerInfo(int userId)
        {
            UserEntity? user = database.GetUserByUserId(userId);
            if (user is null)
            {
                throw new ArgumentException($"用户 {userId} 不存在");
            }

            PlayerInfo playerInfo = new PlayerInfo(user.UserName);
            return playerInfo;
        }
    }
}
