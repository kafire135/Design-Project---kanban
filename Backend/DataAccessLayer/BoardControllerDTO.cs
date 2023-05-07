
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class BoardControllerDTO
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\DataAccessLayer\\BoardControllerDTO.cs");

        private SQLExecuter executer;

        public BoardControllerDTO(SQLExecuter executer)
        {
            this.executer = executer;
        }

        /// <summary>
        /// Adds a board to the database
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Title"></param>
        /// <param name="Owner"></param>
        /// <returns></returns>
        public bool AddBoard(int Id,string Title ,string Owner)
        {
            log.Debug($"AddBoard() for {Id}, {Title}, {Owner}");
            return executer.ExecuteWrite ("INSERT into Boards (BoardId, BoardTitle, Owner, BacklogLimit, InprogressLimit, DoneLimit,TaskIDCounter) " +
                                         $"VALUES({Id},'{Title}','{Owner}',-1,-1,-1,0)");
        }

        /// <summary>
        /// Removes a board from the database
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool RemoveBoard(int Id)
        {
            log.Debug($"RemoveBoard() for {Id}");
            return executer.ExecuteWrite($"DELETE FROM Boards " +
                                         $"WHERE BoardId = {Id}; " +
                                         $"DELETE FROM UserJoinedBoards " +
                                         $"WHERE BoardId = {Id}; " +
                                         $"DELETE FROM Tasks " +
                                         $"WHERE BoardId = {Id}") ;
        }

        /// <summary>
        /// Joins a user to a board in the database
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool JoinBoard(string email, int id)
        {
            log.Debug($"JoinBoard() for {email}, {id}");
            return executer.ExecuteWrite("INSERT into UserJoinedBoards (BoardId, Email) " +
                                        $"VALUES({id},'{email}')");
        }

        /// <summary>
        /// Removes a user from a board in the database
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool LeaveBoard(string email, int id)
        {
            log.Debug($"LeaveBoard() for {email}, {id}");
            return executer.ExecuteWrite("DELETE FROM UserJoinedBoards " +
                                        $"WHERE BoardId = {id} and Email like '{email}'; " +
                                        $"UPDATE Tasks " +
                                        $"SET Assignee = 'unAssigned' " +
                                        $"WHERE BoardId = {id} and Assignee like '{email}'");
        }

        /// <summary>
        /// Changes a board owner in the database
        /// </summary>
        /// <param name="oldOwner"></param>
        /// <param name="newOwner"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ChangeOwner(string oldOwner,string newOwner, int id)
        {
            log.Debug($"ChangeOwner() for {oldOwner}, {newOwner}, {id}");
            return executer.ExecuteWrite("UPDATE Boards "+
                                        $"SET Owner = '{newOwner}' "+
                                        $"WHERE BoardId = {id}; " +
                                        $"DELETE FROM UserJoinedBoards " +
                                        $"WHERE BoardId = {id} and Email like '{newOwner}'; " +
                                        $"INSERT INTO UserJoinedBoards(BoardId, Email) " +
                                        $"VALUES({id},'{oldOwner}')");
        }

        /// <summary>
        /// Sets the limit of a column of a board in the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="column"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public bool LimitColumn(int id, BoardColumnNames column, int limit)
        {
            log.Debug($"LimitColumn() for {id}, {column}, {limit}");
            return executer.ExecuteWrite("UPDATE Boards " +
                                        $"SET {column}Limit = {limit} " +
                                        $"WHERE BoardId = {id}");
        }

        /// <summary>
        /// Updates the board ids counter in the database
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool UpdateBoardIdCounter(int newValue)
        {
            log.Debug($"UpdateBoardIdCounter() for {newValue}");
            return executer.ExecuteWrite("UPDATE GlobalCounters " +
                                        $"SET BoardIDCounter = {newValue}");

        }

        /// <summary>
        /// Updates a board's task id counter in the database
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool UpdateTaskIdCounter(int boardId, int newValue)
        {
            log.Debug($"AddBoard() for {boardId}, {newValue}");
            return executer.ExecuteWrite("UPDATE Boards " +
                                        $"SET TaskIDCounter = {newValue} " +
                                        $"WHERE BoardId = {boardId}");
        }

    }
}
