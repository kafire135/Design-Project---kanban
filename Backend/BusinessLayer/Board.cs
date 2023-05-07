using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.Exceptions;
using IntroSE.Kanban.Backend.Utilities;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{

    /// <summary>
    ///This class controls the actions users' board.<br/>
    ///<br/>
    ///<code>Supported operations:</code>
    ///<br/>
    /// <list type="bullet">AddTask()</list>
    /// <list type="bullet">RemoveTask()</list>
    /// <list type="bullet">AdvanceTask()</list>
    /// <list type="bullet">SearchTask()</list>
    /// <list type="bullet">GetColumnLimit()</list>
    /// <list type="bullet">GetColumnName()</list>
    /// <list type="bullet">GetColumn()</list>
    /// <list type="bullet">LimitColumn()</list>
    /// <list type="bullet">JoinBoard()</list>
    /// <list type="bullet">LeaveBoard()</list>
    /// <list type="bullet">ChangeOwner()</list>
    /// <list type="bullet">ValidateColumnOrdinal()</list>
    /// <br/><br/>
    /// ===================
    /// <br/>
    /// <c>Ⓒ Kfir Nissim</c>
    /// <br/>
    /// ===================
    /// </summary>
    public class Board
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\BusinessLayer\\Board.cs");

        private readonly int id;
        private readonly CIString title;
        private CIString owner;
        private readonly LinkedList<CIString> joined;
        private readonly LinkedList<Task>[] columns;
        private readonly int[] columnLimit;
        private readonly Dictionary<int, TaskStates> taskStateTracker;
        private int taskIDCounter;
        private BoardControllerDTO boardDTO;


        public Board(CIString title, int id, CIString owner)
        {
            this.id = id;
            this.title = title;
            this.owner = owner;
            joined = new();
            columnLimit = new int[3];
            columns = new LinkedList<Task>[3];
            columnLimit[(int)TaskStates.backlog] = -1;
            columnLimit[(int)TaskStates.inprogress] = -1;
            columnLimit[(int)TaskStates.done] = -1;
            columns[(int)TaskStates.backlog] = new();
            columns[(int)TaskStates.inprogress] = new();
            columns[(int)TaskStates.done] = new();
            taskStateTracker = new();
            boardDTO = DataAccessLayerFactory.GetInstance().BoardControllerDTO;
        }

        public Board(BoardDTO boardDTO)
        {
            id = boardDTO.Id;
            title = boardDTO.Title;
            owner = boardDTO.Owner;
            joined = new();
            columnLimit = new int[3];
            columns = new LinkedList<Task>[3];
            columns[(int)TaskStates.backlog] = new();
            columns[(int)TaskStates.inprogress] = new();
            columns[(int)TaskStates.done] = new();
            columnLimit[(int)TaskStates.backlog] = boardDTO.BackLogLimit;
            columnLimit[(int)TaskStates.inprogress] = boardDTO.InProgressLimit;
            columnLimit[(int)TaskStates.done] = boardDTO.DoneLimit;
            taskIDCounter = boardDTO.TaskIDCounter;
            taskStateTracker = new();

            foreach (string email in boardDTO.Joined)
            {
                joined.AddLast(email);
            }

            foreach (TaskDTO taskDTO in boardDTO.BackLog)
            {
                Task task = taskDTO;
                taskStateTracker.Add(task.Id, task.State);
                columns[(int)TaskStates.backlog].AddLast(task);
            }
            foreach (TaskDTO taskDTO in boardDTO.InProgress)
            {
                Task task = taskDTO;
                taskStateTracker.Add(task.Id, task.State);
                columns[(int)TaskStates.inprogress].AddLast(task);
            }
            foreach (TaskDTO taskDTO in boardDTO.Done)
            {
                Task task = taskDTO;
                taskStateTracker.Add(task.Id, task.State); ;
                columns[(int)TaskStates.done].AddLast(task);
            }

        }


        //====================================
        //         getters/initializers
        //====================================

        public int Id { get { return id; } init { id = value; } }
        public CIString Title { get { return title; } init { title = value; } }
        public CIString Owner { get { return owner; } init { owner = value; } }
        public LinkedList<CIString> Joined { get { return joined; } init { joined = value; } }
        public LinkedList<Task>[] Columns { get { return columns; } init { columns = value; } }
        public int[] ColumnLimit { /*get { return columnLimit; }*/ init { columnLimit = value; } }
        public Dictionary<int, TaskStates> TaskStateTracker { /*get { return taskStateTracker; }*/ init { taskStateTracker = value; } }
        public int TaskIDCounter { /*get { return taskIDCounter; }*/ init { taskIDCounter = value; } }

        //====================================
        //            Functionality
        //====================================


        /// <summary>
        /// Add new <c>Task</c> to <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the backlog column reached the limit
        /// </summary>
        /// <param name="title"></param>
        /// <param name="duedate"></param>
        /// <param name="description"></param>
        /// <exception cref="ArgumentException"></exception>
        public Task AddTask(CIString title, DateTime duedate, CIString description)
        {
            log.Debug("AddTask() for: " + title + ", " + description + ", " + duedate);
            if (columns[(int)TaskStates.backlog].Count != columnLimit[(int)TaskStates.backlog])
            {
                try
                {
                    Task task = new Task(taskIDCounter, title, duedate, description, Id);
                    columns[(int)TaskStates.backlog].AddLast(task);

                    taskStateTracker.Add(taskIDCounter, TaskStates.backlog);
                    taskIDCounter++;
                    boardDTO.UpdateTaskIdCounter(id, taskIDCounter);
                    log.Debug("AddTask() success");
                    return task;
                }
                catch (ArgumentException e)
                {
                    log.Error("AddTask() failed: '" + e.Message);
                    throw new ArgumentException(e.Message);
                }
                catch (NoSuchElementException e)
                {
                    log.Error("AddTask() failed: '" + e.Message);
                    throw new NoSuchElementException(e.Message);
                }
            }
            else
            {
                log.Error("AddTask() failed: Backlog in board '" + this.title + "' has reached its limit and can't contain more tasks");
                throw new ArgumentException("Backlog in board '" + this.title + "' has reached its limit and can't contain more tasks");
            }
        }

        /// <summary>
        /// Remove <c>Task</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the task doesn't exist
        /// </summary>
        /// <param name="taskId"></param>
        /// <exception cref="NoSuchElementException"></exception>
        public void RemoveTask(int taskId)
        {
            log.Debug("RemoveTask() taskId: " + taskId);
            if (taskStateTracker.ContainsKey(taskId))
            {
                LinkedList<Task> taskList = columns[(int)taskStateTracker[taskId]];
                foreach (Task task in taskList)
                {
                    if (task.Id == taskId)
                    {
                        taskList.Remove(task);
                        taskStateTracker.Remove(taskId);

                        log.Debug("RemoveTask() success");
                        break;
                    }
                }
            }
            else
            {
                log.Error("RemoveTask() failed: task numbered '" + taskId + "' doesn't exist");
                throw new NoSuchElementException("A Task with the taskId '" +
                    taskId + "' doesn't exist in the Board");
            }
        }


        /// <summary>
        /// Advance <c>Task</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the task doesn't exist at all or is not in the specified column<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the task can't be advanced<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number
        /// <b>Throws</b> <c>AccessViolationException</c> if the user isn't task assignee
        /// </summary>
        /// <param name="email"></param>
        /// <param name="taskId"></param>
        /// <param name="columnOrdinal"></param>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void AdvanceTask(CIString email, int columnOrdinal, int taskId)
        {
            log.Debug("AdvanceTask() for column and taskId: " + columnOrdinal + ", " + taskId);
            ValidateColumnOrdinal(columnOrdinal);
            Task task = SearchTask(taskId);
            if (task.Assignee != email)
            {
                log.Error("AdvanceTask() failed: User is not the task's assignee");
                throw new AccessViolationException("User is not the task's assignee");
            }

            if (taskStateTracker.ContainsKey(taskId))
            {
                TaskStates state = taskStateTracker[taskId];
                if ((int)state == columnOrdinal)
                {
                    if (state != TaskStates.done)
                    {
                        if (columns[(int)state + 1].Count < columnLimit[(int)state + 1] | columnLimit[(int)state + 1] == -1)
                        {
                            Task toAdvance = SearchTask(taskId);
                            DataAccessLayerFactory.GetInstance().TaskControllerDTO.ChangeTaskState(id, toAdvance.Id, (BoardColumnNames)toAdvance.State);
                            columns[(int)state].Remove(toAdvance);
                            columns[(int)state + 1].AddLast(toAdvance);
                            taskStateTracker[taskId] = state + 1;
                            log.Debug("AdvanceTask() success");
                        }
                        else
                        {
                            log.Error("AdvanceTask() failed: task numbered '" + taskId + "' can't be advanced because the next column is full");
                            throw new ArgumentException("task numbered '" + taskId + "' can't be advanced because the next column is full");
                        }

                    }
                    else
                    {
                        log.Error("AdvanceTask() failed: task numbered '" + taskId + "' is done and can't be advanced");
                        throw new ArgumentException("task numbered '" + taskId + "' is done and can't be advanced");
                    }
                }
                else
                {
                    log.Error("AdvanceTask() failed: task numbered '" + taskId + "' isn't in the column " + (TaskStates)columnOrdinal);
                    throw new NoSuchElementException("the task '" +
                        taskId + "' isn't in the column " + (TaskStates)columnOrdinal);
                }
            }
            else
            {
                log.Error("AdvanceTask() failed: task numbered '" + taskId + "' doesn't exist");
                throw new NoSuchElementException("task numbered '" + taskId + "' doesn't exist");
            }

        }


        /// <summary>
        /// Search <c>Task</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the task doesn't exist in the specified column <br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="columnOrdinal"></param>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Task SearchTask(int taskId, int columnOrdinal)
        {
            log.Debug("SearchTask() taskId, columnOrdinal: " + taskId + ", " + columnOrdinal);
            ValidateColumnOrdinal(columnOrdinal);

            LinkedList<Task> taskList = columns[columnOrdinal];
            foreach (Task task in taskList)
            {
                if (task.Id == taskId)
                {
                    log.Debug("SearchTask() success");
                    return task;
                }
            }
            log.Error("SearchTask() failed: A task numbered '" + taskId +
                "' doesn't exist in column '" + columnOrdinal + "'");
            throw new NoSuchElementException("A Task with the taskId '" +
                taskId + "' doesn't exist in column '" + columnOrdinal + "'");
        }

        /// <summary>
        /// Search <c>Task</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the task doesn't exist
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns>Task, unless an error occurs</returns>
        /// <exception cref="NoSuchElementException"></exception>
        public Task SearchTask(int taskId)
        {
            log.Debug("SearchTask() taskId: " + taskId);
            if (taskStateTracker.ContainsKey(taskId))
            {
                LinkedList<Task> taskList = columns[(int)taskStateTracker[taskId]];
                foreach (Task task in taskList)
                {
                    if (task.Id == taskId)
                    {
                        log.Debug("SearchTask() success");
                        return task;
                    }
                }
                //======================================================================================================
                // this part of the code should generally never run. if it does, there is a serious problem somewhere.
                //======================================================================================================
                log.Fatal("FATAL ERROR: task numbered" + taskId + "exists in the taskStateTracker and not in the column '" +
                    taskStateTracker[taskId] + "' where it's supposed to be");
                throw new OperationCanceledException("FATAL ERROR: task numbered" + taskId +
                    "exists in the taskStateTracker and not in the column '" +
                    taskStateTracker[taskId] + "' where it's supposed to be");
                //======================================================================================================
            }
            else
            {
                log.Error("SearchTask() failed: A task numbered '" + taskId + "' doesn't exist");
                throw new NoSuchElementException("A Task with the taskId '" +
                    taskId + "' doesn't exist in the Board");
            }
        }

        /// <summary>
        /// Get<c>column limit</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <returns>int column limit, unless an error occurs</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public int GetColumnLimit(int columnOrdinal)
        {
            log.Debug("GetColumnLimit() columnOrdinal: " + columnOrdinal);
            ValidateColumnOrdinal(columnOrdinal);

            log.Debug("GetColumnLimit() success");
            return columnLimit[columnOrdinal];
        }


        /// <summary>
        /// Get<c>column name</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <returns>column name, unless an error occurs</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public string GetColumnName(int columnOrdinal)
        {
            log.Debug("GetColumnName() columnOrdinal: " + columnOrdinal);
            ValidateColumnOrdinal(columnOrdinal);

            log.Debug("GetColumnName() success");
            switch (columnOrdinal)
            {
                case 1: return "in progress";

                default: return ((TaskStates)columnOrdinal).ToString();
            }

        }

        /// <summary>
        /// Get<c>column</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <returns>LinkedList of task, unless an error occurs</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public LinkedList<Task> GetColumn(int columnOrdinal)
        {
            log.Debug("GetColumn() columnOrdinal: " + columnOrdinal);
            ValidateColumnOrdinal(columnOrdinal);

            log.Debug("GetColumn() success");
            return columns[columnOrdinal];
        }

        /// <summary>
        /// Limit<c>column</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>ArgumentException</c> if column size is over the specified limit or the limit is invalid<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <param name="limit"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void LimitColumn(int columnOrdinal, int limit)
        {
            log.Debug("LimitColumn() for column and limit: " + columnOrdinal + ", " + limit);
            ValidateColumnOrdinal(columnOrdinal);

            if (limit >= -1)
            {
                if (limit != -1 & columns[columnOrdinal].Count > limit)
                {
                    log.Error("LimitColumn() failed: '" + (TaskStates)columnOrdinal + "' size is bigger than the limit " + limit);
                    throw new ArgumentException("A column '" +
                        (TaskStates)columnOrdinal + "' size is bigger than th limit " + limit);
                }

                columnLimit[columnOrdinal] = limit;
                log.Debug("LimitColumn() success");
            }
            else
            {
                log.Error("LimitColumn() failed: '" + limit + "' the limit is not valid");
                throw new ArgumentException("A limit '" +
                    limit + "' is not valid");
            }
        }


        /// <summary>
        /// Change <c>Board's Owner</c> to <c>Board</c> <br/> <br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the newOwnerEmail doesn't joined to the board<br/>
        /// </summary>
        /// <param name="currentOwnerEmail"></param>
        /// <param name="newOwnerEmail"></param>
        /// <param name="boardName"></param>
        /// <exception cref="ArgumentException"></exception>
        public void ChangeOwner(CIString currentOwnerEmail, CIString newOwnerEmail, CIString boardName)
        {
            log.Debug("ChangeOwner() for board: " + boardName + "from: " + currentOwnerEmail + "to: " + newOwnerEmail);
            owner = newOwnerEmail;
            joined.AddLast(currentOwnerEmail);
            joined.Remove(newOwnerEmail);
            log.Debug("ChangeOwner() success");
        }

        /// <summary>
        /// add <c>User</c> to <c>Board</c> joined boards <br/><br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the user is the board's owner or if the user alredy joined to the board<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardId"></param>
        /// <exception cref="ArgumentException"></exception>
        public void JoinBoard(CIString email, int boardId)
        {
            log.Debug("JoinBoard() for user: " + email + "for board " + boardId);

            joined.AddLast(email);
            log.Debug("JoinBoard() success");
        }
        /// <summary>
        /// remove <c>User</c> from <c>Board</c> joined boards and move all it's assigned task to unassigned <br/><br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the user isn't joined to the board<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardId"></param>
        /// <exception cref="ArgumentException"></exception>
        public void LeaveBoard(CIString email, int boardId)
        {
            log.Debug("LeaveBoard() for user: " + email + "for board " + boardId);
            if (!joined.Contains(email))
            {
                log.Error("LeaveBoard() failed: user with email '" + email + "' is not joined to the board");
                throw new ArgumentException("user with email '" + email + "' is not joined to the board");
            }
            joined.Remove(email);
            foreach (Task task in this.columns[(int)TaskStates.backlog])
            {
                if (task.Assignee == email)
                {
                    task.Assignee = "unAssigned";
                }
            }
            foreach (Task task in columns[(int)TaskStates.inprogress])
            {
                if (task.Assignee == email)
                {
                    task.Assignee = "unAssigned";
                }
            }
            log.Debug("LeaveBoard() success");
        }

        /// <summary>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private void ValidateColumnOrdinal(int columnOrdinal)
        {
            if (columnOrdinal < (int)TaskStates.backlog | columnOrdinal > (int)TaskStates.done)
            {
                log.Error("ValidateColumnOrdinal() failed: '" + columnOrdinal + "' is not a valid column number");
                throw new IndexOutOfRangeException("The column '" + columnOrdinal + "' is not a valid column number");
            }
        }

        public static implicit operator Board(BoardDTO other)
        {
            return new Board(other);
        }


        // Serialization



        public class BoardSerializable
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Owner { get; set; }
            public LinkedList<string> Joined { get; set; }
            public LinkedList<Task>[] Columns { get; set; }
        }
        public BoardSerializable GetSerializableInstance()
        {
            LinkedList<string> joinedList = new();
            foreach (CIString joinedUser in joined)
            {
                joinedList.AddLast(joinedUser);
            }
            return new BoardSerializable()
            {
                Id = id,
                Title = title,
                Owner = owner,
                Joined = joinedList,
                Columns = columns,
            }; 
        }

    }
}
