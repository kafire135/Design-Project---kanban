using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.Utilities;
using IntroSE.Kanban.Backend.Exceptions;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
	///This class implements BoardControllerService 
	///<br/>
	///<code>Supported operations:</code>
	///<br/>
	/// <list type="bullet">AddBoard()</list>
	/// <list type="bullet">RemoveBoard()</list>
	/// <list type="bullet">GetAllTasksByState()</list>
    /// <list type="bullet">GetUserBoards</list>
	/// <br/><br/>
	/// ===================
	/// <br/>
	/// <c>Ⓒ Kfir Nissim</c>
	/// <br/>
	/// ===================
	/// </summary>
    public class BoardControllerService
    {

        private readonly BoardController boardController;

        public BoardControllerService(BoardController BC)
        {
            boardController = BC;
        }

        /// <summary>
        /// This method adds a board to the specific user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="name">The name of the new board</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue:  // (operationState == true) => empty string
		/// }		       // (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string AddBoard(string email, string name)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, name }) == false)
            {
                Response<string> res = new(false, "AddBoard() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                boardController.AddBoard(email, name);
                Response<string> res = new(true, "");
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ElementAlreadyExistsException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
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
            catch (System.Data.SQLite.SQLiteException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }

        public string SearchBoard(string email, int boardId)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, boardId }) == false)
            {
                Response<string> res = new(false, "SearchBoard() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                Board.BoardSerializable board = boardController.SearchBoard(email, boardId).GetSerializableInstance();
                Response<Board.BoardSerializable> res = new(true, board);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (ElementAlreadyExistsException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (AccessViolationException ex)
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
            catch (System.Data.SQLite.SQLiteException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }

        /// <summary>
        /// This method removes a board to the specific user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="name">The name of the board</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: // (operationState == true) => empty string
		/// }			// (operationState == false) => error message		
		/// </code>
		/// </returns>
        public string RemoveBoard(string email, string name)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, name }) == false)
            {
                Response<string> res = new(false, "RemoveBoard() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                boardController.RemoveBoard(email, name);
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
            //catch (OperationCanceledException ex)
            //{
            //    Response<string> res = new(false, ex.Message);
            //    return JsonController.ConvertToJson(res);
            //}
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
        /// This method returns all the in-progress tasks that user is assigned to.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">column id . Must be between zero and numbers of columns</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: //(operationState == true) => LinkedList&lt;Task&gt;
		/// }		      //(operationState == false) => string with error message		
		/// </code>
		/// </returns>
        public string GetInProgressTasks(string email)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email }) == false)
            {
                Response<string> res = new(false, "GetInProgressTasks() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                LinkedList<Task> tasks = boardController.GetInProgressTasks(email);
                Response<LinkedList<Task>> res = new(true, tasks);
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
        /// This method returns all the board's Id of the user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>
		/// Json formatted as so:
		/// <code>
		///	{
		///		operationState: bool 
		///		returnValue: //(operationState == true) => LinkedList&lt;int&gt;
		/// }		      //(operationState == false) => string with error message		
		/// </code>
		/// </returns>
        public string GetUserBoards(string email)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email }) == false)
            {
                Response<string> res = new(false, "GetUserBoards() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                Response<LinkedList<int>> res = new(true, boardController.GetBoardsId(email));
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
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
        }

        public string GetBoardById(string email, int id)
        {
            if (ValidateArguments.ValidateNotNull(new object[] { email, id }) == false)
            {
                Response<string> res = new(false, "getBoardById() failed: ArgumentNullException");
                return JsonEncoder.ConvertToJson(res);
            }
            try
            {
                Response<Board.BoardSerializable> res = new(true, boardController.SearchBoard(email, id).GetSerializableInstance());
                return JsonEncoder.ConvertToJson(res);
            }
            catch (NoSuchElementException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserNotLoggedInException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }
            catch (UserDoesNotExistException ex)
            {
                Response<string> res = new(false, ex.Message);
                return JsonEncoder.ConvertToJson(res);
            }

        }

    }


}
