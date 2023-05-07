using System;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class TaskControllerDTO
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\DataAccessLayer\\TaskControllerDTO.cs");

        private SQLExecuter executer;

        public TaskControllerDTO(SQLExecuter executer)
        {
            this.executer = executer;
        }

        /// <summary>
        /// Adds a task to the database
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="taskId"></param>
        /// <param name="title"></param>
        /// <param name="assignee"></param>
        /// <param name="description"></param>
        /// <param name="CreationTime"></param>
        /// <param name="duedate"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool AddTask(int boardId, int taskId, string title, string assignee, string description, DateTime CreationTime, DateTime duedate, BoardColumnNames state)
        {
            log.Debug($"AddTask() for: {boardId}, {taskId}, {title}, {assignee}, {description}, {CreationTime}, {duedate}, {state}");
            string command = "INSERT INTO Tasks(BoardId, TaskId, TaskTitle, Assignee, Description, CreationTime, DueDate, State) "+
                             $"VALUES({boardId},{taskId},'{title}','{assignee}','{description}','{CreationTime}'," +
                             $"'{duedate}',{(int)state})";

            return executer.ExecuteWrite(command);
        }

        /// <summary>
        /// Removes a task from the database
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        public bool RemoveTask(int boardId, int TaskId)
        {
            log.Debug($"RemoveTask() for: {boardId}, {TaskId}");
            return executer.ExecuteWrite("DELETE FROM Tasks " +
                                        $"WHERE BoardId = {boardId} and TaskId = {TaskId}");
        }

        /// <summary>
        /// Changes a task's state in the database
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="taskId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool ChangeTaskState(int boardId, int taskId, BoardColumnNames state)
        {
            log.Debug($"ChangeTaskState() for: {boardId}, {taskId}, {state}");
            string command = "UPDATE Tasks " +
                            $"SET State = {(int)state} " +
                            $"WHERE BoardId = {boardId} and TaskId = {taskId}";

            return executer.ExecuteWrite(command);
        }

        /// <summary>
        /// Changes a task's title in the database
        /// </summary>
        /// <param name="title"></param>
        /// <param name="boardId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool ChangeTitle(string title, int boardId, int taskId)
        {
            log.Debug($"ChangeTitle() for: {title}, {boardId}, {taskId}");
            string command = "UPDATE Tasks " +
                            $"SET TaskTitle = '{title}' " +
                            $"WHERE BoardId = {boardId} and TaskId = {taskId}";

            return executer.ExecuteWrite(command);
        }

        /// <summary>
        /// Changes a task's description in the database
        /// </summary>
        /// <param name="description"></param>
        /// <param name="boardId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool ChangeDescription(string description, int boardId, int taskId)
        {
            log.Debug($"ChangeDescription() for: {description}, {boardId}, {taskId}");
            string command = "UPDATE Tasks " +
                            $"SET Description = '{description}' " +
                            $"WHERE BoardId = {boardId} and TaskId = {taskId}";

            return executer.ExecuteWrite(command);
        }

        /// <summary>
        /// Changes a task's assingee in the database
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool ChangeAssignee(string email, int boardId, int taskId)
        {
            log.Debug($"ChangeAssignee() for: {email}, {boardId}, {taskId}");
            string command = "UPDATE Tasks " +
                            $"SET Assignee = '{email}' " +
                            $"WHERE BoardId = {boardId} and TaskId = {taskId}";

            return executer.ExecuteWrite(command);
        }

        /// <summary>
        /// Changes a task's due date in the database
        /// </summary>
        /// <param name="dueDate"></param>
        /// <param name="boardId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool ChangeDueDate(DateTime dueDate, int boardId, int taskId)
        {
            log.Debug($"ChangeDueDate() for: {dueDate}, {boardId}, {taskId}");
            string command = "UPDATE Tasks " +
                            $"SET DueDate = '{dueDate}' " +
                            $"WHERE BoardId = {boardId} and TaskId = {taskId}";

            return executer.ExecuteWrite(command);
        }
    }
}
