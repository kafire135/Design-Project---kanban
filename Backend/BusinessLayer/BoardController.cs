using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.Exceptions;
using IntroSE.Kanban.Backend.Utilities;

namespace IntroSE.Kanban.Backend.BusinessLayer
{

    /// <summary>
    ///This class controls the actions users' boards.<br/>
    ///<br/>
    ///<code>Supported operations:</code>
    ///<br/>
    /// <list type="bullet">AddBoard()</list>
    /// <list type="bullet">RemoveBoard()</list>
    /// <list type="bullet">GetAllTasksByState()</list>
    /// <list type="bullet">GetBoards()</list>
    /// <list type="bullet">GetBoardsId()</list>
    /// <list type="bullet">SearchBoard()</list>
    /// <list type="bullet">JoinBoard()</list>
    /// <list type="bullet">LeaveBoard()</list>
    /// <list type="bullet">ValidateUser()</list>
    /// <list type="bullet">ChangeOwner()</list>
    /// <list type="bullet">LimitColumn()</list>
    /// <list type="bullet">GetColumnLimit()</list>
    /// <list type="bullet">GetColumnName()</list>
    /// <list type="bullet">>GetColumn()</list>
    /// <list type="bullet">>ValidateColumnOrdinal()</list>
    /// <br/><br/>
    /// ===================
    /// <br/>
    /// <c>Ⓒ Kfir Nissim</c>
    /// <br/>
    /// ===================
    /// </summary>

    public class BoardController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\BusinessLayer\\BoardController.cs");
        BoardDataOperations boardData;
        BoardControllerDTO BCDTO;
        /// <summary>
        /// Initialize a new BoardController <br/><br/>
        /// </summary>
        /// <param name="boardData"></param>
        public BoardController(BoardDataOperations boardData)
        {
            this.boardData = boardData;
            BCDTO = DataAccessLayerFactory.GetInstance().BoardControllerDTO;
        }

        /// <summary>
        /// Add new <c>Board</c> to <c>UserData</c> userData <br/> <br/>
        /// <b>Throws</b> <c>ElementAlreadyExistsException</c> if a<c> Board</c> with that title already exists<br/>
        /// for the <c>User</c><br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in<br/>
        /// <b>Throws</b> <c>DataMisalignedException</c> if BoardIDCounter is out of sync<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if board name is empty<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <exception cref="ElementAlreadyExistsException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddBoard(CIString email, CIString name)
        {

            log.Debug("AddBoard() for: " + email + "Board's name" + name);

            try
            {
                ValidateUser(email);

                if (string.IsNullOrWhiteSpace(name))
                {
                    log.Error("AddBoard() failed: board name is empty");
                    throw new ArgumentException("board name is empty");
                }
                Board newBoard = boardData.AddNewBoard(email, name);

                //DAL CALLS
                BCDTO.AddBoard(newBoard.Id, newBoard.Title.Value, newBoard.Owner.Value);
                log.Debug("AddBoard() success");
            }
            catch (ElementAlreadyExistsException e)
            {
                log.Error("AddBoard() failed: "+e.Message);
                throw;
            }
            catch (DataMisalignedException)
            {
                log.Fatal("AddBoard() failed: BoardIDCounter is out of sync");
                throw;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("AddBoard() failed: " + e.Message);
                throw;
            }
            catch (UserNotLoggedInException e)
            {
                log.Error("AddBoard() failed: " + e.Message);
                throw;
            }

        }


        /// <summary>
        /// Remove <c>Board</c> from <c>UserData</c> userData <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the user user isn't the owner<br/>
        /// <b>Throws</b> <c>OperationCanceledException</c> if the user user isn't the owner<br/>
        /// in the system
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public void RemoveBoard(CIString email, CIString title)
        {


            log.Debug("RemoveBoard() for: " + email + "Board's name" + title);
            try
            {
                ValidateUser(email);
                Board board = SearchBoard(email, title);
                if (board.Owner != email)
                {
                    log.Error("RemoveBoard() failed: user has not permission to do RemoveBoard");
                    throw new AccessViolationException("user has not permission to do RemoveBoard");
                }
                boardData.NukeBoard(email, title);

                //DAL CALLS
                BCDTO.RemoveBoard(board.Id);
                log.Debug("RemoveBoard() success");
            }
            catch (NoSuchElementException)
            {
                log.Error("RemoveBoard() failed: '" + email + "' doesn't exist");
                throw;
            }
            catch (OperationCanceledException)
            {
                log.Error("RemoveBoard() failed: board '" + title + "' doesn't exist for " + email);
                throw;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("RemoveBoard() failed: " + e.Message);
                throw;
            }
            catch (UserNotLoggedInException e)
            {
                log.Error("RemoveBoard() failed: " + e.Message);
                throw;
            }
        }

        /// <summary>
        /// Returns <c>tasks' list</c> from <c>UserData</c> userData <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnOrdinal"></param>
        /// <returns>A list of tasks by specific state, unless an error occurs</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public LinkedList<Task> GetInProgressTasks(CIString email)
            {
            log.Debug("GetInProgressTasks() for: " + "Board's name");
            try
            {
                ValidateUser(email);

                LinkedList<Task> tasks = new LinkedList<Task>();
                LinkedList<Board> boards = GetBoards(email);
                foreach (Board board in boards)
                {
                    foreach (Task task in board.GetColumn((int)TaskStates.inprogress))
                    {
                        if(task.Assignee == email)
                            tasks.AddLast(task);
                    }
                }
                log.Debug("GetInProgressTasks() success");
                return tasks;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("GetInProgressTasks() failed: " + e.Message);
                throw;
            }
            catch (UserNotLoggedInException e)
            {
                log.Error("GetAllTasksByState() failed: " + e.Message);
                throw;
            }
            catch (IndexOutOfRangeException e)
            {
                log.Error("GetAllTasksByState() failed: " + e.Message);
                throw;
            }
            
        }


        /// <summary>
        /// Returns <c>boards' list</c> from <c>UserData</c> userData <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A list of Boards, unless an error occurs</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        public LinkedList<Board> GetBoards (CIString email) {

            log.Debug("GetBoards() for: " + email);
            try
            {
                ValidateUser(email);
                LinkedList<Board> myBoards = boardData.GetBoardsDataUnit(email).MyBoards;
                LinkedList<Board> joinedBoards = boardData.GetBoardsDataUnit(email).JoinedBoards;
                LinkedList<Board> output = new();
                foreach(Board board in myBoards)
                {
                    output.AddLast(board);
                }
                foreach (Board board in joinedBoards)
                {
                    output.AddLast(board);
                }
                log.Debug("GetBoards() success");
                return output;
            }
            catch (NoSuchElementException)
            {
                log.Error("GetBoards() failed: '" + email + "' doesn't exist");
                throw;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("GetBoards() failed: " + e.Message);
                throw;
            }
            catch (UserNotLoggedInException e)
            {
                log.Error("GetBoards() failed: " + e.Message);
                throw;
            }
        }

        /// <summary>
        /// Returns <c>List</c> of boardId of user <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A list of int of board Id, unless an error occurs</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        public LinkedList<int> GetBoardsId(CIString email)
        {

            log.Debug("GetBoardsId() for: " + email);
            try
            {
                ValidateUser(email);
                LinkedList<Board> myBoards = GetBoards(email);
                LinkedList<int> output = new();
                foreach (Board board in myBoards)
                {
                    output.AddLast(board.Id);
                }
                log.Debug("GetBoardsId() success");
                return output;
            }
            catch (NoSuchElementException)
            {
                log.Error("GetBoardsId() failed: '" + email + "' doesn't exist");
                throw;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("GetBoardsId() failed: " + e.Message);
                throw;
            }
            catch (UserNotLoggedInException e)
            {
                log.Error("GetBoardsId() failed: " + e.Message);
                throw;
            }
        }


        /// <summary>
        /// Returns <c>board</c> from <c>UserData</c> userData <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in<br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exists for the user<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Board, unless an error occurs</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        public Board SearchBoard(CIString email , CIString name)
        {
            log.Debug("SearchBoard() for: " + email + " Board's name " + name);

            try
            {
                ValidateUser(email);

                LinkedList<Board> boardList = boardData.GetBoardsDataUnit(email).MyBoards;
                foreach (Board board in boardList)
                {
                    if (board.Title == name)
                    {
                        log.Debug("SearchBoard() success");
                        return board;
                    }
                }
                LinkedList<Board> boardList1 = boardData.GetBoardsDataUnit(email).JoinedBoards;
                foreach (Board board in boardList1)
                {
                    if (board.Title == name)
                    {
                        log.Debug("SearchBoard() success");
                        return board;
                    }
                }
                log.Error("SearchBoard() failed: '" + name + "' doesn't exist");
                throw new NoSuchElementException("A board titled '" +
                                name + "' doesn't exists for the user with the email " + email);
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("SearchBoard() failed: " + e.Message);
                throw;
            }
            catch (UserNotLoggedInException e)
            {
                log.Error("SearchBoard() failed: " + e.Message);
                throw;
            }
            
        }

        /// <summary>
        /// Returns <c>board</c> from <c>UserData</c> userData <br/> <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/> 
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Board, unless an error occurs</returns>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        public Board SearchBoard(CIString email, int boardId)
        {
            log.Debug("SearchBoard() for: Board's Id " + boardId);
            try
            {
                ValidateUser(email);
                Board board = boardData.SearchBoardById(boardId);
                log.Debug("SearchBoard() success");
                return board;
            }
            catch (NoSuchElementException e)
            {
                log.Error("SearchBoard() failed: " + e.Message);
                throw;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("SearchBoard() failed: " + e.Message);
                throw;
            }
            catch (UserNotLoggedInException e)
            {
                log.Error("SearchBoard() failed: " + e.Message);
                throw;
            }
        }
        /// <summary>
        /// add <c>User</c> to <c>Board</c> joined boards <br/><br/>
        /// <b>Throws</b> <c>ElementAlreadyExistsException</c> if The user already has a board with that name<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if The user is already joined to the board<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if The user is the board's owner<br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/> 
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardId"></param>
        /// <exception cref="ElementAlreadyExistsException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        public void JoinBoard(CIString email, int boardId)
        {
            log.Debug("JoinBoard() for: user " + email + " Board's Id " + boardId);
            try
            {
                ValidateUser(email);
                Board board = SearchBoard(email, boardId);
                if (board.Owner == email)
                {
                    log.Error("JoinBoard() failed: user with email '" + email + "' is the board's owner");
                    throw new AccessViolationException("the user '" + email + "' is the board's owner");
                }
                if (board.Joined.Contains(email))
                {
                    log.Error("JoinBoard() failed: user with email '" + email + "' is already joined to the board");
                    throw new ArgumentException("the user " + email + " is already joined to the board");
                }
                foreach (Board boardToTest in GetBoards(email))
                {
                    if (boardToTest.Title == board.Title)
                    {
                        throw new ElementAlreadyExistsException($"The user '{email}' already has a board with that name");
                    }
                }

                board.JoinBoard(email, boardId);
                boardData.AddPointerToJoinedBoard(email, boardId);

                //DAL CALLS
                BCDTO.JoinBoard(email, boardId);

                log.Debug("JoinBoard() success");
            }
            catch (ElementAlreadyExistsException e)
            {
                log.Error("JoinBoard() failed: "+e.Message);
                throw;
            }
            catch (NoSuchElementException e)
            {
                log.Error($"JoinBoard() failed: "+ e.Message);
                throw; ;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("JoinBoard() failed: " + e.Message);
                throw;
            }
            catch (UserNotLoggedInException e)
            {
                log.Error("JoinBoard() failed: " + e.Message);
                throw;
            }
            catch (AccessViolationException e)
            {
                log.Error("JoinBoard() failed: " + e.Message);
                throw;
            }
            catch (ArgumentException e)
            {
                log.Error("JoinBoard() failed: " + e.Message);
                throw;
            }

        }
        /// <summary>
        /// remove <c>User</c> from <c>Board</c> joined boards <br/><br/>
        /// <b>Throws</b> <c>ElementAlreadyExistsException</c> if The user already has a board with that name<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if The user is already joined to the board<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if The user is the board's owner<br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/> 
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardId"></param>
        /// <exception cref="ElementAlreadyExistsException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        public void LeaveBoard(CIString email, int boardId)
        {
            log.Debug("LeaveBoard() for user: " + email + "for board " + boardId);
            try
            {
                ValidateUser(email);
                Board board = SearchBoard(email, boardId);
                if (board.Owner == email)
                {
                    log.Error($"LeaveBoard() failed: user {email} is the owner of the board {boardId}");
                    throw new AccessViolationException($"user '{email}' is the board's owner");
                }
                board.LeaveBoard(email, boardId);
                boardData.RemovePointerToJoinedBoard(email, boardId);

                //DAL CALLS
                BCDTO.LeaveBoard(email.Value, boardId);

                log.Debug("LeaveBoard() success");
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("LeaveBoard() failed: "+e.Message);
                throw;
            }
            catch (UserNotLoggedInException e)
            {
                log.Error("JoinBoard() failed: " + e.Message);
                throw;
            }
            catch(NoSuchElementException e)
            {
                log.Error($"LeaveBoard() failed: "+e.Message);
                throw;
            }
            catch (AccessViolationException e)
            {
                log.Error("LeaveBoard() failed: "+e.Message);
                throw;
            }
            catch (ArgumentException e)
            {
                log.Error("JoinBoard() failed: " + e.Message);
                throw;
            }
        }


        /// <summary>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        private void ValidateUser(CIString email)
        {
            if (!boardData.UserExists(email))
            {
                log.Error("ValidateUser() failed: a user with the email '" +
                    email + "' doesn't exist in the system");
                throw new UserDoesNotExistException("A user with the email '" +
                    email + "' doesn't exist in the system");
            }
            if (!boardData.UserLoggedInStatus(email))
            {
                log.Error("ValidateUser() failed: user '" + email + "' isn't logged in");
                throw new UserNotLoggedInException("user '" + email + "' isn't logged in");
            }
        }


        /// <summary>
        /// Change <c>Board's Owner</c> to <c>BoardData</c> boardData <br/> <br/>
        /// <b>Throws</b> <c>ElementAlreadyExistsException</c> if a<c> Board</c> with that title already exists<br/>
        /// for the <c>User</c><br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if Board doesn't exist for the user<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if user isn't the owner<br/>
        /// </summary>
        /// <param name="currentOwnerEmail"></param>
        /// <param name="newOwnerEmail"></param>
        /// <param name="boardName"></param>
        /// <exception cref="ElementAlreadyExistsException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void ChangeOwner(CIString currentOwnerEmail, CIString newOwnerEmail, CIString boardName)
        {
            log.Debug("ChangeOwner() for board: " + boardName + "from: " + currentOwnerEmail + "to: " + newOwnerEmail);
            try
            {

                Board board = SearchBoard(currentOwnerEmail, boardName);
                if (board.Owner != currentOwnerEmail)
                {
                    log.Error($"ChangeOwner() failed: user {currentOwnerEmail} isn't the board's owner");
                    throw new AccessViolationException($"user {currentOwnerEmail} isn't the board's owner");
                }
                if (board.Owner == newOwnerEmail)
                {
                    log.Info($"ChangeOwner() didn't do anything: user {newOwnerEmail} is already the owner of the board {boardName}");
                    return;
                }
                if (board.Joined.Contains(newOwnerEmail) == false)
                {
                    log.Error($"ChangeOwner() failed: user {newOwnerEmail} isn't joined to the board and can't be made owner");
                    throw new AccessViolationException($"user {newOwnerEmail} isn't joined to the board and can't be made owner");
                }

                board.ChangeOwner(currentOwnerEmail, newOwnerEmail, boardName);
                boardData.ChangeOwnerPointer(currentOwnerEmail, boardName, newOwnerEmail);


                //DAL CALLS
                BCDTO.ChangeOwner(currentOwnerEmail,newOwnerEmail, board.Id);

                log.Debug("ChangeOwner() success");
            }
            catch (ElementAlreadyExistsException e)
            {
                log.Error("ChangeOwner() failed: "+e.Message);
                throw;
            }
            catch (NoSuchElementException e)
            {
                log.Error("ChangeOwner() failed: " + e.Message);
                throw;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("ChangeOwner() failed: " + e.Message);
                throw;
            }
            catch (ArgumentException e)
            {
                log.Error("ChangeOwner() failed: " + e.Message);
                throw;
            }
        }


        /// <summary>
        /// Limit<c>column</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number<br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the limit is illegal<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the the user isn't the board's owner or joined the board<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="limit"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public void LimitColumn(CIString email, CIString boardName, int columnOrdinal, int limit)
        {
            log.Debug("LimitColumn() for column and limit: " + columnOrdinal + ", " + limit);
            try
            {
                Board board = SearchBoard(email, boardName);
                board.LimitColumn(columnOrdinal, limit);

                //DAL CALLS
                BCDTO.LimitColumn(board.Id, (BoardColumnNames)columnOrdinal, limit);

                log.Debug("LimitColumn() success");
            }
            catch (NoSuchElementException ex)
            {
                log.Error("LimitColumn() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("LimitColumn() failed: " + ex.Message);
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("LimitColumn() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("LimitColumn() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("LimitColumn() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("LimitColumn() failed: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get<c>column limit</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the the user isn't the board's owner or joined the board<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="columnOrdinal"></param>
        /// <returns>int column limit, unless an error occurs</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public int GetColumnLimit(CIString email, CIString boardName, int columnOrdinal)
        {
            log.Debug("GetColumnLimit() columnOrdinal: " + columnOrdinal);
            try
            {
                Board board = SearchBoard(email, boardName);
                int columnlimit = board.GetColumnLimit(columnOrdinal);
                log.Debug("GetColumnLimit() success");
                return columnlimit;
            }
            catch (NoSuchElementException ex)
            {
                log.Error("GetColumnLimit() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("GetColumnLimit() failed: " + ex.Message);
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("GetColumnLimit() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("GetColumnLimit() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("GetColumnLimit() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("GetColumnLimit() failed: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Get<c>column name</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the the user isn't the board's owner or joined the board<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="columnOrdinal"></param>
        /// <returns>column name, unless an error occurs</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public string GetColumnName(CIString email, CIString boardName, int columnOrdinal)
        {
            log.Debug("GetColumnName() columnOrdinal: " + columnOrdinal);
            try
            {
                Board board = SearchBoard(email, boardName);
                string columnname = board.GetColumnName(columnOrdinal);
                log.Debug("GetColumnName() success");
                return columnname;
            }
            catch (NoSuchElementException ex)
            {
                log.Error("GetColumnName() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("GetColumnName() failed: " + ex.Message);
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("GetColumnName() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("GetColumnName() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("GetColumnName() failed: " + ex.Message);
                throw;
            }
            catch (UserNotLoggedInException ex)
            {
                log.Error("GetColumnName() failed: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Get<c>column</c> from <c>Board</c> board <br/> <br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// <b>Throws</b> <c>UserNotLoggedInException</c> if the user isn't logged in <br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if the board doesn't exist<br/>
        /// <b>Throws</b> <c>IndexOutOfRangeException</c> if the column is not a valid column number<br/>
        /// <b>Throws</b> <c>AccessViolationException</c> if the the user isn't the board's owner or joined the board<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <param name="columnOrdinal"></param>
        /// <returns>LinkedList of task, unless an error occurs</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="UserNotLoggedInException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        public LinkedList<Task> GetColumn(CIString email, CIString boardName, int columnOrdinal)
        {
            log.Debug("GetColumn() columnOrdinal: " + columnOrdinal);
            try
            {
                Board board = SearchBoard(email, boardName);
                LinkedList<Task> column = board.GetColumn(columnOrdinal);
                log.Debug("GetColumn() success");
                return column;
            }
            catch (NoSuchElementException ex)
            {
                log.Error("GetColumn() failed: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                log.Error("GetColumn() failed: " + ex.Message);
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("GetColumn() failed: " + ex.Message);
                throw;
            }
            catch (AccessViolationException ex)
            {
                log.Error("GetColumn() failed: " + ex.Message);
                throw;
            }
            catch (UserDoesNotExistException ex)
            {
                log.Error("GetColumn() failed: " + ex.Message);
                throw;
            }
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

    }
}
