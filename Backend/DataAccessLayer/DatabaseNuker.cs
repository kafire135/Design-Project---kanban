using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class DatabaseNuker
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\DataAccessLayer\\DatabaseNuker.cs");

        private SQLExecuter executer;

        public DatabaseNuker()
        {
            executer = DataAccessLayerFactory.GetInstance().SQLExecuter;
        }
        

        /// <summary>
        /// <b>WARNING: DELETES *ALL* PERSISTED DATA FROM THE DATABASE</b>
        /// </summary>
        public void Nuke()
        {
            log.Info("Database nuking started!");
            string[] tables = { "Users", "Boards","UserJoinedBoards","Tasks"};
            foreach (string table in tables)
            {
                executer.ExecuteWrite($"DELETE FROM {table}");
            }
            executer.ExecuteWrite($"UPDATE GlobalCounters SET BoardIDCounter = 0");
            log.Info("Database nuking finished successfully");
        }
    }
}
