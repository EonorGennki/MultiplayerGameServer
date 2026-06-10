using MultiplayerGameServer.DAO;

namespace MultiplayerGameServer.Logic.Service
{
    internal class BaseService
    {
        protected Database database;

        public BaseService(Database database)
        {
            this.database = database;
        }
    }
}
