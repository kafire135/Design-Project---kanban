
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class UserControllerDTO
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\DataAccessLayer\\UserControllerDTO.cs");

        private SQLExecuter executer;

        public UserControllerDTO(SQLExecuter executer)
        {
            this.executer = executer;
        }

        /// <summary>
        /// Adds a user to the database
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool AddUser(string email, string password)
        {
            log.Debug($"AddUser() for: {email}, {password}");
            return executer.ExecuteWrite("INSERT INTO Users (Email,Password) " +
                $"VALUES('{email}','{password}')");
        }

        /// <summary>
        /// Changes a user's password in the database
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ChangePassword(string email, string password)
        {
            log.Debug($"ChangePassword() for: {email}, {password}");
            return executer.ExecuteWrite("UPDATE Users " +
                $"SET Password = '{password}' " +
                $"WHERE Email like '{email}'");
        }

        /// <summary>
        /// Changes a user's email in the database
        /// </summary>
        /// <param name="oldEmail"></param>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        public bool ChangeEmail(string oldEmail, string newEmail)
        {
            log.Debug($"ChangeEmail() for: {oldEmail}, {newEmail}");
            return executer.ExecuteWrite("UPDATE Users " +
                $"SET Email = '{newEmail}' " +
                $"WHERE Email like '{oldEmail}'");
        }
    }
}
