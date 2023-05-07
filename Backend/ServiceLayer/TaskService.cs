using System;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.Utilities;
using IntroSE.Kanban.Backend.Exceptions;

namespace IntroSE.Kanban.Backend.ServiceLayer
{

    /// <summary>
	///This class implements TaskService 
	///<br/>
	///<code>Supported operations:</code>
	///<br/>
    /// <list type="bullet">AddTask()</list>
    /// <list type="bullet">RemoveTask()</list>
    /// <list type="bullet">AdvanceTask()</list>
    /// <list type="bullet">UpdateTaskDueDate()</list>
	/// <list type="bullet">AssignTask()</list>
	/// <list type="bullet">UpdateTaskTitle()</list>
    /// <list type="bullet">UpdateTaskDescription()</list>
	/// <br/><br/>
	/// ===================
	/// <br/>
	/// <c>Ⓒ Kfir Nissim</c>
	/// <br/>
	/// ===================
	/// </summary>
    /// 

    public class TaskService
    {
        private readonly TaskController taskController;

        public TaskService(TaskController TC)
        {
            taskController = TC;
        }

        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
		/// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: // (operationState == true) => empty string
		/// }			// (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardName, title, description, dueDate }) == false)
            {
                Response<string> res = new(false, "AddTask() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                taskController.AddTask(email, boardName, title, description, dueDate);
                Response<string> res = new(true, "");
                return JsonEncoder.ConvertToJson(res);
            }
            catch (NoSuchElementException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ArgumentException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }

        /// <summary>
        /// This method removes a task to the specific user and board.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardTitle">The name of the board</param>
        /// <param name="taskId">id of the task</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: // (operationState == true) => empty string
		/// }			// (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string RemoveTask(string email, string boardTitle, int taskId)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardTitle, taskId }) == false)
            {
                Response<string> res = new(false, "RemoveTask() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                taskController.RemoveTask(email, boardTitle, taskId);
                Response<string> res = new(true, "");
                return JsonEncoder.ConvertToJson(res);
            }
            catch (NoSuchElementException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ArgumentException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }


        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>
        /// Json formatted as so:
        /// <code>
        ///	{
        ///		operationState: bool 
        ///		returnValue: // (operationState == true) => empty string
        /// }			// (operationState == false) => error message		
        /// </code>
        /// </returns>
        public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardName, columnOrdinal, taskId }) == false)
            {
                Response<string> res = new(false, "AdvanceTask() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                taskController.AdvanceTask(email, boardName, columnOrdinal, taskId);
                Response<string> res = new(true, "");
                return JsonEncoder.ConvertToJson(res);
            }
            catch (NoSuchElementException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ArgumentException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (IndexOutOfRangeException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }

        /// <summary>
        /// This method updates the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: // (operationState == true) => empty string
		/// }			// (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardName, columnOrdinal, taskId, dueDate }) == false)
            {
                Response<string> res = new(false, "UpdateTaskDueDate() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                taskController.UpdateTaskDueDate(email, boardName, columnOrdinal, taskId, dueDate);
                Response<string> res = new(true, "");
                return JsonEncoder.ConvertToJson(res);
            }
            catch (NoSuchElementException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ArgumentException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (IndexOutOfRangeException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }

        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: // (operationState == true) => empty string
		/// }			// (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardName, columnOrdinal, taskId, title}) == false)
            {
                Response<string> res = new(false, "UpdateTaskTitle() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                taskController.UpdateTaskTitle(email,boardName,columnOrdinal,taskId,title);
                Response<string> res = new(true, "");
                return JsonEncoder.ConvertToJson(res);
            }
            catch (NoSuchElementException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ArgumentException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (IndexOutOfRangeException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }

        /// <summary>
        /// This method updates the description of a task.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: // (operationState == true) => empty string
		/// }			// (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardName, columnOrdinal, taskId, description }) == false)
            {
                Response<string> res = new(false, "UpdateTaskDescription() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                taskController.UpdateTaskDescription(email,boardName,columnOrdinal,taskId,description);
                Response<string> res = new(true, "");
                return JsonEncoder.ConvertToJson(res);
            }
            catch (NoSuchElementException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ArgumentException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (IndexOutOfRangeException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }


        /// <summary>
        /// This method assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column number. The first column is 0, the number increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified a task ID</param>        
        /// <param name="emailAssignee">Email of the asignee user</param>
        /// <returns>
        /// Json formatted as so:
        /// <code>
        ///	{
        ///		operationState: bool 
        ///		returnValue: // (operationState == true) => empty string
        /// }			// (operationState == false) => error message		
        /// </code>
        /// </returns>
        public string AssignTask(string email, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardName, columnOrdinal, taskId, emailAssignee }) == false)
            {
                Response<string> res = new(false, "UpdateTaskDescription() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                taskController.AssignTask(email,boardName,columnOrdinal,taskId,emailAssignee);
                Response<string> res = new(true, "");
                return JsonEncoder.ConvertToJson(res);
            }
            catch (NoSuchElementException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (IndexOutOfRangeException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ElementAlreadyExistsException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }

    }
    
}
