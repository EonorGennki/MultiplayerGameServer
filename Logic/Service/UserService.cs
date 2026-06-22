using MultiplayerGameServer.DAO;
using MultiplayerGameServer.Logic.Interface;
using System.Security.Cryptography;
using System.Text;
using Yitter.IdGenerator;

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
                return ServiceResult.Failure(ServiceErrorCode.UserAlreadyExists);
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
            if (user is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.UserNotFound);
            }

            if (user.UserId == -1)
            {
                return ServiceResult.Failure(ServiceErrorCode.DatabaseError);
            }

            if (user.UserId == -2)
            {
                return ServiceResult.Failure(ServiceErrorCode.UnknownError);
            }

            string passwordHash = HashPassword(password, user.Salt);
            if (passwordHash != user.PasswordHash)
            {
                return ServiceResult.Failure(ServiceErrorCode.InvalidPassword);
            }

            database.SetUserActive(user.UserId, true);
            ServiceResult result = ServiceResult.Success();
            database.GetPlayers(user);
            long playerId = 0;
            if (user.Players.Count == 0)
            {
                playerId = YitIdHelper.NextId();
                database.InsertPlayer(user.UserId, playerId);
                database.SetPlayerActive(playerId, true);
            }
            else
            {
                foreach (var player in user.Players)
                {
                    if (player.isActive)
                    {
                        playerId = player.PlayerId;
                        break;
                    }
                }
            }

            result.Data = new UserData
            {
                UserId = user.UserId,
                PlayerId = playerId
            };

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

            PlayerInfo playerInfo = new PlayerInfo(userId, user.UserName);
            return playerInfo;
        }
    }
}
