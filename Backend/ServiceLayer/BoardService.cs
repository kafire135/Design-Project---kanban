using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.Utilities;
using IntroSE.Kanban.Backend.Exceptions;

namespace IntroSE.Kanban.Backend.ServiceLayer
{

    /// <summary>
	///This class implements BoardService 
	///<br/>
	///<code>Supported operations:</code>
	///<br/>
	/// <list type="bullet">LimitColumn()</list>
    /// <list type="bullet">GetColumnLimit()</list>
	/// <list type="bullet">GetColumnName()</list>
	/// <list type="bullet">GetColumn()</list>
    /// <list type="bullet">ChangeOwner()</list>
    /// <list type="bullet">JoinBoard()</list>
    /// <list type="bullet">LeaveBoard()</list>
	/// <br/><br/>
	/// ===================
	/// <br/>
	/// <c>Ⓒ Kfir Nissim</c>
	/// <br/>
	/// ===================
	/// </summary>
    /// 
    public class BoardService
    {
        private readonly BoardController boardController;

        public BoardService(BoardController BC)
        {
            boardController = BC;
        }

        /// <summary>
        /// This method add user to board's joined boards
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardId">the Id of the board</param>
        /// <returns>
        /// Json formatted as so:
        /// <code>
        ///	{
        ///		operationState: bool 
        ///		returnValue: // (operationState == true) => empty string
        /// }			// (operationState == false) => error message		
        /// </code>
        /// </returns>
        public string JoinBoard(string email, int boardId)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardId }) == false)
            {
                Response<string> res = new(false, "JoinBoard() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                boardController.JoinBoard(email, boardId);
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
            catch (ElementAlreadyExistsException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ArgumentException ex)
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
        /// This method remove user from the board's joined boards
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardId">the Id of the board</param>
        /// <returns>
        /// Json formatted as so:
        /// <code>
        ///	{
        ///		operationState: bool 
        ///		returnValue: // (operationState == true) => empty string
        /// }			// (operationState == false) => error message		
        /// </code>
        /// </returns>

        public string LeaveBoard(string email, int boardId)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardId }) == false)
            {
                Response<string> res = new(false, "LeaveBoard() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                boardController.LeaveBoard(email, boardId);
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
            catch (ArgumentException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ElementAlreadyExistsException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }


        /// <summary>
        /// This method transfers a board ownership.
        /// </summary>
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>
        /// <param name="newOwnerEmail">Email of the new owner</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: string // (operationState == true) => empty string;
		/// }				// (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string ChangeOwner(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { currentOwnerEmail, newOwnerEmail, boardName }) == false)
            {
                Response<string> res = new(false, "ChangeOwner() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                boardController.ChangeOwner(currentOwnerEmail, newOwnerEmail, boardName);
                Response<string> res = new(true, "");
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ElementAlreadyExistsException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (NoSuchElementException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ArgumentException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }

        


        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>
        /// Json formatted as so:
        /// <code>
        ///	{
        ///		operationState: bool 
        ///		returnValue: // (operationState == true) => empty string
        /// }			// (operationState == false) => error message		
        /// </code>
        /// </returns>
        public string LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardName, columnOrdinal, limit }) == false)
            {
                Response<string> res = new(false, "LimitColumn() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                boardController.LimitColumn(email,boardName,columnOrdinal,limit);
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
        /// This method gets the limit of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: // (operationState == true) => column limit (int)
		/// }			// (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardName, columnOrdinal }) == false)
            {
                Response<string> res = new(false, "GetColumnLimit() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                int columnlimit = boardController.GetColumnLimit(email, boardName, columnOrdinal);
                Response<int> res = new(true, columnlimit);
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
        /// This method gets the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: // (operationState == true) => column name (string)
		/// }	             // (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardName, columnOrdinal }) == false)
            {
                Response<string> res = new(false, "GetColumnName() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                string columnname = boardController.GetColumnName(email,boardName,columnOrdinal);
                Response<string> res = new(true, columnname);
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
        /// This method returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: string // (operationState == true) => LinkedList&lt;Task&gt;
		/// }				// (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email ,boardName, columnOrdinal }) == false)
            {
                Response<string> res = new(false, "GetColumn() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                LinkedList<Task> column = boardController.GetColumn(email,boardName,columnOrdinal);
                Response<LinkedList<Task>> res = new(true, column);
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


    }
    
}
