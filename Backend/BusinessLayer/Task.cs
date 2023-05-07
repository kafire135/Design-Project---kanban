using System;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.Utilities;
using IntroSE.Kanban.Backend.Exceptions;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.BusinessLayer
{


    /// <summary>
    ///This class controls the actions users' task.<br/>
    ///<br/>
    ///<code>Supported operations:</code>
    ///<br/>
    /// <list type="bullet">Task()</list>
    /// <list type="bullet">AdvanceTask()</list>
    /// <list type="bullet">UpdateTitle()</list>
    /// <list type="bullet">UpdateDescription()</list>
    /// <list type="bullet">UpdateDueDate()</list>
    /// <list type="bullet">AssignTask()</list>
    /// <br/><br/>
    /// ===================
    /// <br/>
    /// <c>Ⓒ Kfir Nissim</c>
    /// <br/>
    /// ===================
    /// </summary>


    public class Task
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\BusinessLayer\\Task.cs");

        private int boardId;
        private readonly int id;
        private readonly DateTime creationTime;
        private CIString title;
        private CIString description;
        private DateTime dueDate;
        private TaskStates state;
        private CIString assignee;
        private TaskControllerDTO taskDTO;
        private readonly int MAX_DESCRIPTION_CHAR_CAP = 300;
        private readonly int MAX_TITLE_CHAR_CAP = 50;
        private readonly int MIN_TITLE_CHAR_CAP = 1;


        /// <summary>
        /// Build <c>Task</c> <br/> <br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the title or description over their char cap, or due date is passed<br/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="boardId"></param>
        /// <param name="title"></param>
        /// <param name="dueDate"></param>
        /// <param name="description"></param>
        /// <exception cref="ArgumentException"></exception>
        public Task(int id, CIString title, DateTime dueDate, CIString description, int boardId)
        {
            log.Debug("Task() for id: " + id);
            if (title.Length < MIN_TITLE_CHAR_CAP)
            {
                log.Error("Task() failed: title is empty");
                throw new ArgumentException("title is empty");
            }
            if (title.Length > MAX_TITLE_CHAR_CAP)
            {
                log.Error("Task() failed: title is over the limit");
                throw new ArgumentException("title is over the limit");
            }
            if (description.Length > MAX_DESCRIPTION_CHAR_CAP)
            {
                log.Error("Task() failed: description is over the limit");
                throw new ArgumentException("description is over the limit");
            }
            if (dueDate.CompareTo(DateTime.Today) < 0)
            {
                log.Error("Task() failed: due date was passed");
                throw new ArgumentException("due date was passed");
            }
            this.id = id;
            this.title = title;
            this.dueDate = dueDate;
            this.description = description;
            assignee = new CIString("unAssigned");
            creationTime = DateTime.Today;
            state = TaskStates.backlog;
            log.Debug("Task() success");
            taskDTO = DataAccessLayerFactory.GetInstance().TaskControllerDTO;
            this.boardId = boardId;
        }


        /// <summary>
        /// Build <c>Task</c> <br/> <br/>
        /// </summary>
        /// <param name="taskDTO"></param>
        public Task(TaskDTO taskDTO)
        {
            id = taskDTO.Id;
            title = new CIString(taskDTO.Title);
            dueDate = taskDTO.DueDate;
            description = new CIString(taskDTO.Description);
            assignee = new CIString(taskDTO.Assignee);
            creationTime = taskDTO.CreationTime;
            state = (TaskStates)taskDTO.State;
        }

        //====================================
        //            getters/setters
        //====================================

        [JsonIgnore]
        public int BoardId
        { 
            get { return boardId; } 
            init { boardId = value;} 
        }

        public int Id
        {
            get { return id; }
            init { id = value; }
        }

        public DateTime CreationTime
        {
            get { return creationTime; }
            init { creationTime = value; }
        }

        [JsonIgnore]
        public CIString Assignee
        {
            get { return assignee; }
            set { assignee = value; }
        }

        [JsonIgnore]
        public TaskStates State => state;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public DateTime DueDate
        {
            get { return dueDate; }
            set { dueDate = value; }
        }


        //====================================
        //            Functionality
        //====================================


        /// <summary>
        /// Advance <c>Task</c>
        /// <b>Throws</b> <c>ArgumentException</c> if the task can't be advanced<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the user isn't task assignee<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void AdvanceTask(CIString email)
        {
            log.Debug("UpdateDescription() for taskId: " + id);
            if (state == TaskStates.done)
            {
                log.Error("AdvanceTask() failed: task numbered '" + id + "' is done and can't be advanced");
                throw new ArgumentException("task numbered '" + id + "' is done and can't be advanced");
            }
            if (assignee.Equals(email) == false)
            {
                log.Error("AdvanceTask() failed: User is not the task's assignee");
                throw new AccessViolationException("User is not the task's assignee");
            }
            state++;

            log.Debug("AdvanceTask() success");
        }


        /// <summary>
        /// Assign <c>Task assignee</c> to <c>Task</c> task <br/> <br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the task is already done <br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the the email isn't current assignee <br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailAssignee"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public bool AssignTask(CIString email, CIString emailAssignee)
        {
            log.Debug("AssignTask() for taskId: " + id + ", emailAssignee:" + emailAssignee);
            if (state == TaskStates.done)
            {
                log.Error("AssignTask() failed: " + id + "is done");
                throw new ArgumentException("the task '" +
                    id + "' is already done");
            }
            if (assignee == emailAssignee)
            {
                log.Info($"AssignTask() didn't do anything: user {emailAssignee} is already assigned to the task");
                return false;
            }
            if (assignee != email && assignee != "unAssigned")
            {
                log.Error("AssignTask() failed: task numbered '" + id + "' , email: '" + email + "' isn't the task's assignee");
                throw new AccessViolationException("email: '" + email + "' isn't the task's assignee");
            }
            assignee = emailAssignee;        
            log.Debug("AssignTask() success");
            return true;
        }

        /// <summary>
        /// Set <c>Task DueDate</c> to <c>Task</c> task <br/> <br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the due date has passed or task is already done <br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the user isn't the assignee <br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newDueDate"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void UpdateDueDate(CIString email, DateTime newDueDate)
        {
            log.Debug("UpdateDueDate() for taskId: " + id + ", email:" + email);
            if (assignee != email)
            {
                log.Error("UpdateDueDate() failed: User is not the task's assignee");
                throw new AccessViolationException("User is not the task's assignee");
            }
            if (state == TaskStates.done)
            {
                log.Error("UpdateDueDate() failed: " + id + "is done");
                throw new ArgumentException("the task '" +
                    id + "' is already done");
            }
            if (newDueDate.CompareTo(DateTime.Today) < 0)
            {
                log.Error("UpdateDueDate() failed: due date was passed");
                throw new ArgumentException("due date was passed");
            }
            dueDate = newDueDate;
            
            log.Debug("UpdateDueDate() success");
        }


        /// <summary>
        /// Set <c>Task Title</c> to <c>Task</c> task <br/> <br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the title over his char cap/empty or task is already done <br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the user isn't the assignee <br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void UpdateTitle(CIString email, CIString value)
        {
            log.Debug("UpdateTitle() for taskId: " + id + ", email:" + email);
            if (assignee.Equals(email) == false)
            {
                log.Error("UpdateTitle() failed: User is not the task's assignee");
                throw new AccessViolationException("User is not the task's assignee");
            }
            if (state == TaskStates.done)
            {
                log.Error("UpdateTitle() failed: " + id + "is done");
                throw new ArgumentException("the task '" +
                    id + "' is already done");
            }
            if (value.Length < MIN_TITLE_CHAR_CAP)
            {
                log.Error("UpdateTitle() failed: title is empty");
                throw new ArgumentException("title is empty");
            }
            if (value.Length > MAX_TITLE_CHAR_CAP)
            {
                log.Error("UpdateTitle() failed: title is over the limit");
                throw new ArgumentException("title is over the limit");
            }
            title = value;
            
            log.Debug("UpdateTitle() success");

        }


        /// <summary>
        /// Set <c>Task Description</c> to <c>Task</c> task <br/> <br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the title over his char cap or task is already done<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the user isn't the assignee<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newDescription"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void UpdateDescription(CIString email, CIString newDescription)
        {
            log.Debug("UpdateDescription() for taskId: " + id + ", email:" + email);
            if (assignee != email)
            {
                log.Error("UpdateDescription() failed: User is not the task's assignee");
                throw new AccessViolationException("User is not the task's assignee");
            }
            if (state == TaskStates.done)
            {
                log.Error("UpdateDescription() failed: " + id + "is done");
                throw new ArgumentException("the task '" +
                    id + "' is already done");
            }
            if (newDescription.Length > MAX_DESCRIPTION_CHAR_CAP)
            {
                log.Error("UpdateDescription() failed: title is over the limit");
                throw new ArgumentException("Description is over the limit");
            }
            description = newDescription;
            
            log.Debug("UpdateDescription() success");

        }

        public static implicit operator Task(TaskDTO other)
        {
            return new Task(other);     
        }
        //====================================================
        //                  Json related
        //====================================================

        public Task() { }

        //public static implicit operator Serializable.Task_Serializable(Task other)
        //{
        //    return new Serializable.Task_Serializable()
        //    {
        //        Id = other.Id,
        //        CreationTime = other.CreationTime,
        //        Title = other.Title,
        //        Description = other.Description,
        //        DueDate = other.DueDate,
        //    };
        // }

        //public Serializable.Task_Serializable GetSerializableInstance()
        //{
        //    return new Serializable.Task_Serializable()
        //    {
        //        Id = id,
        //        CreationTime = creationTime,
        //        Title = title,
        //        Description = description,
        //        DueDate = dueDate,
        //    };
        //}
    }
}
