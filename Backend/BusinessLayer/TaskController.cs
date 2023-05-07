using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.Utilities;
using IntroSE.Kanban.Backend.Exceptions;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{


    /// <summary>
    ///This class controls the actions users' boards.<br/>
    ///<br/>
    ///<code>Supported operations:</code>
    ///<br/>
    /// <list type="bullet">AddTask()</list>
    /// <list type="bullet">RemoveTask()</list>
    /// <list type="bullet">AdvanceTask()</list>
    /// <list type="bullet">UpdateTaskDescription()</list>
    /// <list type="bullet">UpdateTaskDueDate()</list>
    /// <list type="bullet">UpdateTaskTitle()</list>
    /// <list type="bullet">AssignTask()</list>
    /// <br/><br/>
    /// ===================
    /// <br/>
    /// <c>Ⓒ Kfir Nissim</c>
    /// <br/>
    /// ===================
    /// </summary>


    public class TaskController
    {
        private readonly BoardController boardController;
        private readonly TaskControllerDTO TCDTO;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\BusinessLayer\\TaskController.cs");
        public TaskController(BoardController BC)
        {
            boardController = BC;
            TCDTO = DataAccessLayerFactory.GetInstance().TaskControllerDTO;
        }


        /// <summary>
        /// Add new <c>Task</c> to <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the the user isn't the board's owner or joined the board<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the backlog column reached the limit or one of the task's arguments is illegal<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="title"></param>
        /// <param name="dueDate"></param>
        /// <param name="description"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void AddTask(CIString email, CIString boardName, CIString title, CIString description, DateTime dueDate)
        {
            log.Debug("AddTask() for: " + title + ", " + description + ", " + dueDate);
            try
            {
                Board board = boardController.SearchBoard(email, boardName);
                Task newTask = board.AddTask(title, dueDate, description);

                //DALL CALLS
                DataAccessLayerFactory.GetInstance().TaskControllerDTO.AddTask(board.Id, newTask.Id,
                    newTask.Title, newTask.Assignee, newTask.Description, newTask.CreationTime,
                    newTask.DueDate, (BoardColumnNames)newTask.State);

                log.Debug("AddTask() success");
            }
            catch (NoSuchElementException ex)
            {
                log.Error("AddTask() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("AddTask() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("AddTask() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("AddTask() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("AddTask() failed: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Remove <c>Task</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board or the task doesn't exist<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the the user isn't the board's owner or joined the board<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the backlog column reached the limit or one of the task's arguments is illegal<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardTitle"></param>
        /// <param name="taskId"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void RemoveTask(CIString email, CIString boardTitle, int taskId)
        {
            log.Debug("RemoveTask() taskId: " + taskId);
            try
            {
                Board board = boardController.SearchBoard(email, boardTitle);
                board.RemoveTask(taskId);

                //DAL CALLS
                TCDTO.RemoveTask(board.Id, taskId);

                log.Debug("RemoveTask() success");
            }
            catch (NoSuchElementException ex)
            {
                log.Error("RemoveTask() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("RemoveTask() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("RemoveTask() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("RemoveTask() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("RemoveTask() failed: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Advance <c>Task</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist or task doesn't exist at all or is not in the specified column<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the task can't be advanced<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the user isn't task assignee<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="taskId"></param>
        /// <param name="columnOrdinal"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void AdvanceTask(CIString email, CIString boardName, int columnOrdinal, int taskId)
        {
            log.Debug("AdvanceTask() taskId: " + taskId);
            try
            {
                Board board = boardController.SearchBoard(email, boardName);
                Task task = board.SearchTask(taskId);
                board.AdvanceTask(email, columnOrdinal, taskId);
                task.AdvanceTask(email);

                //DAL CALLS
                TCDTO.ChangeTaskState(task.BoardId, task.Id, (BoardColumnNames)task.State);

                log.Debug("AdvanceTask() success");
            }
            catch (NoSuchElementException ex)
            {
                log.Error("AdvanceTask() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("AdvanceTask() failed: " + ex.Message);
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("AdvanceTask() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("AdvanceTask() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("AdvanceTask() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("AdvanceTask() failed: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Set <c>Task Description</c> to <c>Task</c> task <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist or task doesn't exist at all or is not in the specified column<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the user isn't task assignee<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the description over his char cap or task is already done<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="taskId"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="description"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void UpdateTaskDescription(CIString email, CIString boardName, int columnOrdinal, int taskId, CIString description)
        {
            log.Debug("UpdateDescription() for taskId: " + taskId + ", email:" + email);
            try
            {
                Board board = boardController.SearchBoard(email, boardName);
                Task task = board.SearchTask(taskId, columnOrdinal);
                task.UpdateDescription(email, description);

                //DAL CALLS
                TCDTO.ChangeDescription(task.Description, task.BoardId, task.Id);

                log.Debug("UpdateDescription() success");
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("UpdateDescription() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("SearchBoard() failed: " + ex.Message);
                throw;
            }

            catch (NoSuchElementException ex)
            {
                log.Error("UpdateDescription() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("UpdateDescription() failed: " + ex.Message);
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("UpdateDescription() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("UpdateDescription() failed: " + ex.Message);
                throw;
            }

        }


        /// <summary>
        /// Set <c>Task DueDate</c> to <c>Task</c> task <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist or task doesn't exist at all or is not in the specified column<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the user isn't task assignee<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the due date has passed or task is already done<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="taskId"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="dueDate"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void UpdateTaskDueDate(CIString email, CIString boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            log.Debug("UpdateDueDate() for taskId: " + taskId + ", email:" + email);
            try
            {
                Board board = boardController.SearchBoard(email, boardName);
                Task task = board.SearchTask(taskId, columnOrdinal);
                task.UpdateDueDate(email, dueDate);

                //DAL CALLS
                TCDTO.ChangeDueDate(task.DueDate, task.BoardId, task.Id);

                log.Debug("UpdateTaskDueDate() success");
            }
            catch (NoSuchElementException ex)
            {
                log.Error("UpdateDueDate() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("UpdateDueDate() failed: " + ex.Message);
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("UpdateDueDate() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("UpdateDueDate() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("UpdateDueDate() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("UpdateDueDate() failed: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Set <c>Task Title</c> to <c>Task</c> task <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist or task doesn't exist at all or is not in the specified column<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the user isn't task assignee<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the title over his char cap/empty or task is already done<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="taskId"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="title"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void UpdateTaskTitle(CIString email, CIString boardName, int columnOrdinal, int taskId, CIString title)
        {
            log.Debug("UpdateTaskTitle() for taskId: " + taskId + ", email:" + email);
            try
            {
                Board board = boardController.SearchBoard(email, boardName);
                Task task = board.SearchTask(taskId, columnOrdinal);
                task.UpdateTitle(email, title);

                //DAL CALLS
                TCDTO.ChangeTitle(task.Title, task.BoardId, task.Id);

                log.Debug("UpdateTaskTitle() success");
            }
            catch (NoSuchElementException ex)
            {
                log.Error("UpdateTaskTitle() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("UpdateTaskTitle() failed: " + ex.Message);
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("UpdateTaskTitle() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("UpdateTaskTitle() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("UpdateTaskTitle() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("UpdateTaskTitle() failed: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Assign <c>Task assignee</c> to <c>Task</c> task <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist or task doesn't exist at all or is not in the specified column<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the the email isn't current assignee<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the task is already done <br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="taskId"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="email"></param>>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void AssignTask(CIString email, CIString boardName, int columnOrdinal, int taskId, CIString emailAssignee)
        {
            log.Debug("AssignTask() for taskId: " + taskId + ", email:" + email);
            try
            {
                if (BusinessLayerFactory.GetInstance().BoardDataOperations.UserExists(emailAssignee) == false)
                    throw new UserDoesNotExistException($"user {emailAssignee} does not exist in the system");



                Board board = boardController.SearchBoard(email, boardName);

                if (board.Owner != emailAssignee & board.Joined.Contains(emailAssignee) == false)
                {
                    throw new AccessViolationException($"{emailAssignee} is not joined to the board and cannot be assigned to the task");
                }
                Task task = board.SearchTask(taskId, columnOrdinal);
                if (task.AssignTask(email, emailAssignee))
                {
                    //DAL CALLS
                    TCDTO.ChangeAssignee(task.Assignee, task.BoardId, task.Id);
                }
                log.Debug("AssignTask() success");
            }
            catch (NoSuchElementException ex)
            {
                log.Error("AssignTask() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("AssignTask() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("AssignTask() failed: " + ex.Message);
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("AssignTask() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("AssignTask() failed: " + ex.Message);
                throw;
            }
        }
    }

}

