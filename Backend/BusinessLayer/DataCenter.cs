using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.Utilities;
using IntroSE.Kanban.Backend.Exceptions;

namespace IntroSE.Kanban.Backend.BusinessLayer
{

    //===========================================================================
    //                                DataCenter
    //===========================================================================


    /// <summary>
    /// The class uses the facade design pattern.<br/>
    /// it manages a data structure of of <c>User</c>s and <c>Board</c>s. <br/><br/>
    /// The class provides an interface for accessing the underlying data structures and performs most of the<br/>
    /// basic operations needed.<br/><br/>
    /// 
    /// <code>Supported operations:</code>
    /// <b>-------------User Related--------------</b>
    /// <list type="bullet">
    /// <item>SearchUser(email)</item>
    /// <item>AddUser(email,password)</item>
    /// <item>RemoveUser(email)</item>
    /// <item>UserExists(email)</item>
    /// <item>UserLoggedInStatus(email)</item>
    /// <item>SetLoggedIn(email)</item>
    /// <item>SetLoggedOut(email)</item>
    /// </list>
    /// <b>-------------Boards Related--------------</b>
    /// <list type="bullet">
    /// <item>GetBoardsDataUnit(email)</item>
    /// <item>SearchBoardById(board_id)</item>
    /// <item>SearchBoardByEmailAndTitle(email,board_title)</item>
    /// <item>AddNewBoard(email,board_title)</item>
    /// <item>AddPointerToJoinedBoard(email,board_id)</item>
    /// <item>RemovePointerToJoinedBoard(email,board_id)</item>
    /// <item>NukeBoard(email,board_title)</item>
    /// <item>ChangeOwnerPointer(old_owner,board_title,new_owner)</item>
    /// <item>UserOwnsABoardWithThisTitle(email, board_title)</item>
    /// <item>UserIsJoinedToABoardWithThisTitle</item>
    /// </list>
    /// <b>-------------Management--------------</b>
    /// <list type="bullet">
    /// <item>LoadData()</item>
    /// <item>DeleteData()</item>
    /// </list>
    /// <br/>
    /// ===================
    /// <br/>
    /// <c>Ⓒ Yuval Roth</c>
    /// <br/>
    /// ===================
    /// </summary>
    public class DataCenter : UserDataOperations, BoardDataOperations, DataCenterManagement
    {

        // --------------------------------- Chapters ------------------------------------- 

        // 1) Fields and Constructors
        // 2) Public Methods
        // 3) Private Methods
        // 4) Implemented Interfaces

        // NOTICE: the documentation for all public methods is inherited from the Interfaces

        //----------------------------------------------------------------------------------




        //===========================================================================
        //                                Fields And Constructors
        //===========================================================================


        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\BusinessLayer\\DataCenter.cs");


        // -------------------- Structs ----------------------
        private struct DataUnit
        {
            public User User { get; init; }
            public BoardsDataUnit BoardsDataUnit { get; init; }
        }
        public struct BoardsDataUnit
        {
            public LinkedList<Board> MyBoards { get; init; }
            public LinkedList<Board> JoinedBoards { get; init; }
        }
        //------------------------------------------------------

        //------------------- Data Repository ------------------
        private AVLTree<CIString, DataUnit> UsersAndBoardsTree;
        private AVLTree<int, Board> OnlyBoardsTree;
        private HashSet<CIString> loggedIn;
        private int nextBoardID;
        private bool dataLoaded;
        //-----------------------------------------------------


        private DataAccessLayerFactory DALFactory;
        

        public DataCenter()
        {
            DALFactory = DataAccessLayerFactory.GetInstance();
            UsersAndBoardsTree = new();
            OnlyBoardsTree = new();
            loggedIn = new();
            dataLoaded = false;
        }


        //===========================================================================
        //                                Public Methods
        //===========================================================================

        public User SearchUser(CIString email)
        {
            try
            {
                log.Debug("SearchUser() for: " + email);
                User output = UsersAndBoardsTree.GetData(email).User;
                log.Debug("SearchUser() success");
                return output;
            }
            catch (KeyNotFoundException)
            {
                log.Error("SearchUser() failed: '" + email + "' doesn't exist in the system");
                throw new UserDoesNotExistException("A user with the email '" +
                    email + "' doesn't exist in the system");
            }
        }

        public User AddUser(CIString email, string password)
        {
            try
            {
                log.Debug("AddUser() for: " + email);
                DataUnit data = UsersAndBoardsTree.Add(email, new DataUnit()
                {
                    User = new User(email, password),
                    BoardsDataUnit = new()
                    {
                        MyBoards = new LinkedList<Board>(),
                        JoinedBoards = new LinkedList<Board>()
                    }
                });

                log.Debug("AddUser() success");
                return data.User;
            }
            catch (DuplicateKeysNotSupported)
            {
                log.Error("AddUser() failed: '" + email + "' already exists");
                throw new ElementAlreadyExistsException("A user with the email '" +
                    email + "' already exists in the system");
            }
        }

        public void RemoveUser(CIString email)
        {
            throw new NotImplementedException("DEPRECATED METHOD: Not updated to support current requirements");

#pragma warning disable CS0162 // Unreachable code detected
            try
            {
                log.Debug("RemoveUser() for: " + email);
                UsersAndBoardsTree.Remove(email);
                /*
                    TO DO:
                    Take care of boards of deleted users, including joined
                 */
                log.Debug("RemoveUser() success");
            }
            catch (KeyNotFoundException)
            {
                log.Error("RemoveUser() failed: '" + email + "' doesn't exist");
                throw new UserDoesNotExistException("A user with the email '" +
                    email + "' doesn't exist in the system");
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        public bool UserLoggedInStatus(CIString email)
        {
            log.Debug("UserLoggedInStatus() for: " + email);
            return loggedIn.Contains(email);
        }

        public void SetLoggedIn(CIString email)
        {
            try
            {
                log.Debug("SetLoggedIn() for: " + email);
                ValidateUser(email);
                if (UserLoggedInStatus(email) == false)
                {
                    log.Info(email + " is now logged in");
                    loggedIn.Add(email);
                }
                else
                {
                    log.Error("SetLoggedIn() failed: '" + email + "' is already logged in");
                    throw new ArgumentException("The user with the email '" + email + "' is already logged in");
                }
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("SetLoggedIn() failed: "+e.Message);
                throw;
            }
            
        }

        public void SetLoggedOut(CIString email)
        {
            try
            {
                log.Debug("SetLoggedOut() for: " + email);
                ValidateUser(email);
                if (UserLoggedInStatus(email) == true)
                {
                    log.Info(email + " is now logged out");
                    loggedIn.Remove(email);
                }
                else
                {
                    log.Error("SetLoggedOut() failed: '" + email + "' is not logged in");
                    throw new ArgumentException("The user with the email '" + email + "' is not logged in");
                }
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("SetLoggedout() failed: " + e.Message);
                throw;
            }
        }

        public bool UserExists(CIString email)
        {
            log.Debug("UserExists() for: " + email);
            return UsersAndBoardsTree.Contains(email);
        }

        public BoardsDataUnit GetBoardsDataUnit(CIString email)
        {
            
            try
            {
                log.Debug("GetBoardsDataUnit() for: " + email);
                ValidateUser(email);


                DataUnit data = UsersAndBoardsTree.GetData(email);
                log.Debug("GetBoardsDataUnit() success");
                return data.BoardsDataUnit;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("GetBoardsDataUnit() failed: "+e.Message);
                throw;
            }
        }

        public Board SearchBoardById(int id)
        {
            try
            {
                log.Debug("SearchBoardById() for: " + id);
                Board output = OnlyBoardsTree.GetData(id);
                log.Debug("SearchBoardById() success");
                return output;
            }
            catch (KeyNotFoundException)
            {
                log.Error("SearchBoardById() failed: board number '" + id + "' doesn't exist");
                throw new NoSuchElementException("Board number '" + id + "' doesn't exist");
            }
        }

        public Board SearchBoardByEmailAndTitle(CIString email, CIString title)
        {
            try
            {
                ValidateUser(email);

                LinkedList<Board> myBoardsList = UsersAndBoardsTree.GetData(email).BoardsDataUnit.MyBoards;
                Board output = FindBoardInList(myBoardsList, title);
                if (output == null)
                {
                    log.Error("SearchBoardByEmailAndTitle() failed: board titled '" + title + "' doesn't exist for " + email);
                    throw new NoSuchElementException("Board title '" + title + "' doesn't exist for " + email);
                }
                return output;
            }
            catch(UserDoesNotExistException e)
            {
                log.Error("SearchBoardByEmailAndTitle() failed: "+e.Message);
                throw;
            }
        }

        public Board AddNewBoard(CIString email, CIString title)
        {
            try
            {
                log.Debug("AddNewBoard() for: " + email + ", " + title);

                ValidateUser(email);

                // Check if there's a board with that title already
                if (UserOwnsABoardWithThisTitle(email, title) | UserIsJoinedToABoardWithThisTitle(email,title))
                {
                    log.Error("AddNewBoard() failed: board '" + title + "' already exists for " + email);
                    throw new ElementAlreadyExistsException("A board titled " +
                            title + " already exists for the user with the email " + email);
                }

                // Fetch the user's boards
                LinkedList<Board> myBoardList = UsersAndBoardsTree.GetData(email).BoardsDataUnit.MyBoards;

                // Add a new board and return it
                Board newBoard = new(title, GetNextBoardID,email);
                OnlyBoardsTree.Add(newBoard.Id, newBoard);
                myBoardList.AddLast(newBoard);
                IncrementBoardID();
                log.Debug("AddNewBoard() success");
                return newBoard;
            }
            catch (DuplicateKeysNotSupported)
            {
                log.Fatal("AddNewBoard() failed: BoardIDCounter is out of sync");
                throw new DataMisalignedException("BoardIDCounter is out of sync");
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("AddNewBoard() failed: "+e.Message);
                throw;
            }
        }

        public Board AddPointerToJoinedBoard(CIString email, int id)
        {
            try
            {
                log.Debug("AddPointerToJoinedBoard() for: " + email + ", " + id);

                ValidateUser(email);

                Board boardToJoin = SearchBoardById(id);

                // Check if the user is joined on the board already
                if (UserJoinedToBoardCheck(email, boardToJoin.Title))
                {
                    log.Error("AddPointerToJoinedBoard() failed: " + email + " is already joined on board nubmer " + id);
                    throw new ElementAlreadyExistsException(email + " is already joined on board nubmer " + id);
                }

                LinkedList<Board> joinedBoardList = UsersAndBoardsTree.GetData(email).BoardsDataUnit.JoinedBoards;
                joinedBoardList.AddLast(boardToJoin);

                log.Debug("AddPointerToJoinedBoard() success");
                return boardToJoin;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("AddPointerToJoinedBoard() failed: "+e.Message);
                throw;
            }
            catch (NoSuchElementException)
            {
                log.Error("AddPointerToJoinedBoard() failed: board number " + id + "doesn't exist");
                throw;
            }        
        }

        public Board RemovePointerToJoinedBoard(CIString email, int id) 
        {
            try
            {
                log.Debug("RemovePointerToJoinedBoard() for: " + email + ", " + id);

                ValidateUser(email);

                // Fetch the user's boards
                LinkedList<Board> joinedBoardList = UsersAndBoardsTree.GetData(email).BoardsDataUnit.JoinedBoards;
            
                // Search for the specific board
                for (LinkedListNode<Board> node = joinedBoardList.First; node != null; node = node.Next)
                {
                    if (node.Value.Id == id)
                    {
                        Board output = node.Value;
                        joinedBoardList.Remove(node);

                        log.Debug("RemovePointerToJoinedBoard() success");
                        return output;
                    }
                }

                // didn't find a board by that id
                log.Error("RemovePointerToJoinedBoard() failed: " + email + " is not joined to board nubmer " + id);
                throw new NoSuchElementException(email + " is not joined to board nubmer " + id);
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("RemovePointerToJoinedBoard() failed: "+e.Message);
                throw;
            }
        }

        //public void NukeBoard(int id)
        //{
        //    try
        //    {
        //        log.Debug("NukeBoard() for: " + id);
        //        //validate that the board can be cleanly removed
        //        Board toRemove = SearchBoardById(id);
        //        if (UserOwnsABoardWithThisTitle(toRemove.Owner, toRemove.Title) == false)
        //        {
        //            log.Fatal("Board numbered " + id + " says that '" + toRemove.Owner +
        //                "' owns it but the board doesn't exist for that user");
        //            throw new OperationCanceledException("NukeBoard() failed: Board numbered " + id + " says that '" + toRemove.Owner +
        //                "' owns it but the board doesn't exist for that user");
        //        }
        //        foreach (CIString email in toRemove.Joined)
        //        {
        //            if (UserJoinedToBoardCheck(email, toRemove.Title) == false)
        //            {
        //                log.Fatal("Board numbered " + id + " says that '" + email +
        //                "' is joined to it but the user isn't joined to that board");
        //                throw new OperationCanceledException("NukeBoard() failed: Board numbered " + id + " says that '" + email +
        //                "' is joined to it but the user isn't joined to that board");
        //            }
        //        }

        //        // do the removal from everywhere
        //        OnlyBoardsTree.Remove(toRemove.Id);
        //        RemoveBoardFromOwner(toRemove.Owner, toRemove.Title);
        //        foreach (CIString joinedEmail in toRemove.Joined)
        //        {
        //            RemovePointerToJoinedBoard(joinedEmail, toRemove.Id);
        //        }

        //        //DALFactory.BoardControllerDTO.RemoveBoard(id);
        //        log.Debug("NukeBoard() success");
        //    }
        //    catch (NoSuchElementException)
        //    {
        //        log.Error("NukeBoard() failed: board numbered " + id + " does not exist in the system");
        //        throw;
        //    }        
        //}
        
        public void NukeBoard(CIString email, CIString title)
        {
            try
            {
                log.Debug("NukeBoard() for " + email + ", " + title);

                ValidateUser(email);

                //validate that the board can be cleanly removed
                Board toRemove = SearchBoardByEmailAndTitle(email,title);
                if (email.Equals(toRemove.Owner)==false)
                {
                    log.Fatal("Board numbered " + toRemove.Id + " says that '" + toRemove.Owner +
                        "' owns it but the user '" + email + "' owns it as well");
                    throw new OperationCanceledException("NukeBoard() failed: Board numbered " + toRemove.Id + " says that '" + toRemove.Owner +
                        "' owns it but the user '" + email + "' owns it as well");
                }
                foreach (CIString joinedEmail in toRemove.Joined)
                {
                    if (UserJoinedToBoardCheck(joinedEmail, title) == false)
                    {
                        log.Fatal("Board numbered " + toRemove.Id + " says that '" + joinedEmail +
                        "' is joined to it but the user isn't joined to that board");
                        throw new OperationCanceledException("NukeBoard() failed: Board numbered " + toRemove.Id + " says that '" + joinedEmail +
                        "' is joined to it but the user isn't joined to that board");
                    }
                }


                //remove it from everywhere
                RemoveBoardFromOwner(email, title);
                foreach (CIString joinedEmail in toRemove.Joined)
                {
                    RemovePointerToJoinedBoard(joinedEmail, toRemove.Id);
                }
                RemoveBoardById(toRemove.Id);
                
                log.Debug("NukeBoard() success");
            }
            catch (NoSuchElementException)
            {
                log.Error("NukeBoard() failed: board titled " + title + " does not exist for user "+email);
                throw;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("NukeBoard() failed: user "+e.Message);
                throw;
            }
        }

        public void ChangeOwnerPointer(CIString oldOwner, CIString title, CIString newOwner)
        {
            
            try
            {
                log.Debug("ChangeOwnerPointers() for: " + oldOwner + ", " + title + ", " + newOwner);

                ValidateUser(oldOwner);
                ValidateUser(newOwner);

                if (UserOwnsABoardWithThisTitle(newOwner, title))
                {
                    log.Error("ChangeOwnerPointers() failed: board '" + title + "' already exists for " + newOwner);
                    throw new ElementAlreadyExistsException("A board titled " +
                            title + " already exists for the user with the email " + newOwner);
                }
                Board board = RemoveBoardFromOwner(oldOwner, title);
                AddExistingBoard(newOwner, board);

                log.Debug("ChangeOwnerPointers() success");
            }
            catch (NoSuchElementException e)
            {
                log.Error("ChangeOwnerPointers() failed: " + e.Message);
                throw;
            }
            catch (UserDoesNotExistException e) {
                log.Error("ChangeOwnerPointers() failed: " + e.Message);
                throw;
            }
        }

        public bool UserOwnsABoardWithThisTitle(CIString email, CIString title)
        {
            try
            {
                log.Debug("UserOwnsABoardWithThisTitle() for: " + email + ", " + title);

                ValidateUser(email);

                LinkedList<Board> myBoardList = UsersAndBoardsTree.GetData(email).BoardsDataUnit.MyBoards;

                bool answer = FindBoardInList(myBoardList, title) != null;
                log.Debug("UserOwnsABoardWithThisTitle() success");
                return answer;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("UserOwnsABoardWithThisTitle() failed: "+e.Message);
                throw;
            }     
        }

        public bool UserIsJoinedToABoardWithThisTitle(CIString email, CIString title)
        {
            try
            {
                log.Debug("UserIsJoinedABoardWithThisTitle() for: " + email + ", " + title);

                ValidateUser(email);

                LinkedList<Board> JoinedBoardList = UsersAndBoardsTree.GetData(email).BoardsDataUnit.JoinedBoards;

                bool answer = FindBoardInList(JoinedBoardList, title) != null;
                log.Debug("UserIsJoinedABoardWithThisTitle() success");
                return answer;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("UserIsJoinedABoardWithThisTitle() failed: " + e.Message);
                throw;
            }
        }

        public void LoadData()
        {
            if (dataLoaded) return;

            log.Debug("LoadData() initialized");

            DataLoader dataLoader = DALFactory.DataLoader;
            dataLoader.LoadData();


            nextBoardID = dataLoader.BoardIdCounter;
            LinkedList<BoardDTO> boardDTOs = dataLoader.BoardsList;
            LinkedList<UserDTO> userDTOs = dataLoader.UsersList;

            foreach (BoardDTO boardDTO in boardDTOs)
            {
                OnlyBoardsTree.Add(boardDTO.Id, boardDTO);
            }

            foreach (UserDTO userDTO in userDTOs)
            {
                User user = userDTO;

                LinkedList<Board> myBoards = new();
                foreach (int boardId in userDTO.MyBoards)
                {
                    myBoards.AddLast(OnlyBoardsTree.GetData(boardId));
                }

                LinkedList<Board> joinedBoards = new();
                foreach (int boardId in userDTO.JoinedBoards)
                {
                    joinedBoards.AddLast(OnlyBoardsTree.GetData(boardId));
                }

                UsersAndBoardsTree.Add(user.Email, new DataUnit()
                {
                    User = user,
                    BoardsDataUnit = new()
                    {
                        MyBoards = myBoards,
                        JoinedBoards = joinedBoards
                    }
                });
            }
            dataLoaded = true;
            log.Debug("LoadData() success");

        }

        public void DeleteData()
        {
            new DatabaseNuker().Nuke();
            UsersAndBoardsTree = new();
            OnlyBoardsTree = new();
            loggedIn = new();
            nextBoardID = 0;
            dataLoaded = true;
        }


        //===========================================================================
        //                                Private Methods
        //===========================================================================


        /// <summary>
        /// <b>Throws</b> <c>ElementAlreadyExistsException</c> if a board with that title already exists for the user<br/><br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if a user with that email doesn't exist
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newBoard"></param>
        /// <exception cref="ElementAlreadyExistsException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        private void AddExistingBoard(CIString email, Board newBoard)
        {
            try
            {
                log.Debug("AddExistingBoard() for: " + email + ", " + newBoard.Title);

                ValidateUser(email);

                // Fetch the user's boards
                LinkedList<Board> myBoardList = UsersAndBoardsTree.GetData(email).BoardsDataUnit.MyBoards;

                // Check if there's a board with that title already
                if (UserOwnsABoardWithThisTitle(email, newBoard.Title))
                {
                    log.Error("AddExistingBoard() failed: board '" + newBoard.Title + "' already exists for " + email);
                    throw new ElementAlreadyExistsException("A board titled " +
                            newBoard.Title + " already exists for the user with the email " + email);
                }

                // Add the board
                myBoardList.AddLast(newBoard);
                log.Debug("AddExistingBoard() success");
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("AddExistingBoard() failed: "+e.Message);
                throw;
            }
        }

        /// <summary>
        /// Removes a board from BoardTree
        /// <br/><br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if a <c>Board</c> with that id <br/>
        /// doesn't exist in the system<br/><br/>
        /// </summary>
        /// <exception cref="NoSuchElementException"></exception>
        /// <returns>The removed Board</returns>
        private Board RemoveBoardById(int id)
        {
            try
            {
                log.Debug("RemoveBoardById() for: "+ id);
                Board output = OnlyBoardsTree.Remove(id);
                log.Debug("RemoveBoardById() success");
                return output;
            }
            catch (KeyNotFoundException)
            {
                log.Error("RemoveBoard() failed: board numbered '" + id + "' doesn't exist in the system");
                throw new UserDoesNotExistException("board numbered '" + id + "' doesn't exist in the system");
            }
        }

        /// <summary>
        /// Removes a board from the owner inside UsersAndBoardsTree
        /// <br/><br/>
        /// <b>Throws</b> <c>NoSuchElementException</c> if a <c>Board</c> with that title <br/>
        /// doesn't exist for the user<br/><br/>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the <c>User</c> doesn't exist <br/><br/>
        /// in the system
        /// </summary>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <returns>The removed Board</returns>
        private Board RemoveBoardFromOwner(CIString email, CIString title)
        {
            try
            {
                log.Debug("RemoveBoardByEmailAndTitle() for: " + email + ", " + title);

                ValidateUser(email);

                // Fetch the user's boards
                LinkedList<Board> myBoardList = UsersAndBoardsTree.GetData(email).BoardsDataUnit.MyBoards;

                // Search for the specific board
                for (LinkedListNode<Board> node = myBoardList.First; node != null; node = node.Next)
                {
                    if (node.Value.Title.Equals(title))
                    {
                        Board output = node.Value;
                        myBoardList.Remove(node);
                        log.Debug("RemoveBoardByEmailAndTitle() success");
                        return output;
                    }
                }

                // didn't find a board by that name
 
                log.Error("RemoveBoardByEmailAndTitle() failed: board '" + title + "' doesn't exist for " + email);
                throw new NoSuchElementException("A board titled '" +
                                title + "' doesn't exists for the user with the email " + email);
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("RemoveBoardByEmailAndTitle() failed: "+e.Message);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        private bool UserJoinedToBoardCheck(CIString email, CIString title)
        {
            try
            {
                log.Debug("UserJoinedToBoardCheck() for: " + email + ", " + title);

                ValidateUser(email);

                LinkedList<Board> myBoardList = UsersAndBoardsTree.GetData(email).BoardsDataUnit.JoinedBoards;

                bool answer = FindBoardInList(myBoardList, title) != null;
                log.Debug("UserJoinedToBoardCheck() success");
                return answer;
            }
            catch (UserDoesNotExistException e)
            {
                log.Error("UserJoinedToBoardCheck() failed: "+e.Message);
                throw;
            }
        }

        /// <returns>Board if found or null if not found</returns>
        private Board FindBoardInList(LinkedList<Board> boardList, CIString title)
        {
            Board output = null;
            foreach (Board board in boardList)
            {
                if (board.Title.Equals(title)) 
                { 
                    output = board;
                    break;
                }
            }
            return output;
        }

        /// <returns>Board if found or null if not found</returns>
        private Board FindBoardInList(LinkedList<Board> boardList, int id)
        {
            Board output = null;
            foreach (Board board in boardList)
            {
                if (board.Id == id)
                {
                    output = board;
                    break;
                }
            }
            return output;
        }

        /// <summary>
        /// <b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        private void ValidateUser(CIString email)
        {
            log.Debug("ValidateUser() for: " + email);
            if (UserExists(email) == false)
            {
                //log.Error("ValidateUser() failed: a user with the email '" +
                //    email + "' doesn't exist in the system");
                throw new UserDoesNotExistException("A user with the email '" +
                    email + "' doesn't exist in the system");
            }
            log.Debug("ValidateUser() success");
        }

        private int GetNextBoardID => nextBoardID;
        private void IncrementBoardID()
        {
            nextBoardID++;
            DALFactory.BoardControllerDTO.UpdateBoardIdCounter(nextBoardID);
        }



    }

    //===========================================================================
    //                           Implemented Interfaces
    //===========================================================================

    public interface UserDataOperations
    {
        /// <summary>
        /// Searches for a user with the specified email<br/><br/>
        /// <b>Throws </b> <c>UserDoesNotExistException</c> if the user doesn't exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns>User</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        public User SearchUser(CIString email);

        /// <summary>
        /// Adds a user to the system
        /// <br/><br/>
        /// <b>Throws </b> <c>ElementAlreadyExists</c> if a user with this email<br/>
        /// already exists in the system
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="ElementAlreadyExistsException"></exception>
        /// <returns>The added <c>User</c></returns>
        public User AddUser(CIString email, string password);

        /// <summary>
        /// Removes the user with the specified email from the system
        /// <br/><br/>
        /// <b>Throws </b> <c>UserDoesNotExistException</c> if the user doesn't exist in the system
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        public void RemoveUser(CIString email);

        /// <summary>
        /// Check if a user exists in the system
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true or false</returns>
        public bool UserExists(CIString email);

        /// <summary>
        /// Gets the user's logged in status
        /// </summary>
        /// <returns><c>true</c> if the user is logged in, <c>false</c>  otherwise</returns>
        /// <param name="email"></param>
        public bool UserLoggedInStatus(CIString email);

        /// <summary>
        /// Sets a user's logged in status to true
        /// <br/><br/>
        /// <b>Throws </b> <c>ArgumentException</c> if the user's logged in status is already true
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetLoggedIn(CIString email);

        /// <summary>
        /// Sets a user's logged in status to false
        /// <br/><br/>
        /// <b>Throws </b> <c>ArgumentException</c> if the user's logged in status is already false
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetLoggedOut(CIString email);
    }
    public interface BoardDataOperations 
    {
        /// <summary>
        /// Check if a user exists in the system
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true or false</returns>
        public bool UserExists(CIString email);

        /// <summary>
        /// Gets the user's logged in status
        /// </summary>
        /// <returns><c>true</c> if the user is logged in, <c>false</c>  otherwise</returns>
        /// <param name="email"></param>
        public bool UserLoggedInStatus(CIString email);

        /// <summary>
        /// Gets all the <c>User</c>'s boards data
        /// <br/><br/>
        /// <b>Throws </b> <c>UserDoesNotExistException</c> if the <c>User</c> does not exist<br/>
        /// in the system
        /// </summary>
        /// <returns><see cref="BoardsDataUnit"/></returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        public DataCenter.BoardsDataUnit GetBoardsDataUnit(CIString email);

        /// <summary>
        /// Gets all the <c>User</c>'s <c>Board</c>s
        /// <br/><br/>
        /// <b>Throws </b> <c>NoSuchElementException</c> if the <c>User</c> does not exist<br/>
        /// in the system
        /// </summary>
        /// <returns><see cref="Board"/></returns>
        /// <exception cref="NoSuchElementException"></exception>
        public Board SearchBoardById(int board_id);

        /// <summary>
        /// Searches for a board by email and title<br/><br/>
        /// <b>Throws </b> <c>NoSuchElementException</c> if the board doesn't exist for the user<br/><br/>
        /// <b>Throws </b> <c>UserDoesNotExistException</c> if the user doesn't exist in the system
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        public Board SearchBoardByEmailAndTitle(CIString email, CIString board_title);

        /// <summary>
        /// Adds a <c>Board</c> to the <c>User</c>.
        ///<br/><br/>
        /// <b>Throws </b> <c>ElementAlreadyExistsException</c> if a <c>Board</c> with that title already exists <br/>
        /// for the <c>User</c><br/><br/>
        /// <b>Throws </b> <c>UserDoesNotExistException</c> if the <c>User</c> doesn't exist <br/>
        /// in the system
        /// </summary>
        /// <returns>The <c>Board</c> that was added</returns>
        /// <exception cref="ElementAlreadyExistsException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        public Board AddNewBoard(CIString email, CIString board_title);

        /// <summary>
        /// Adds a pointer of an existing board to the user's JoinedBoards.<br/><br/>
        /// <b>Throws </b> <c>ElementAlreadyExistsException</c> if the user is already joined on the board<br/>
        /// <b>Throws </b> <c>UserDoesNotExistException</c> if the user doesn't exist in the system<br/>
        /// <b>Throws </b> <c>NoSuchElementException</c> if a board with that id doesn't exist<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <exception cref="ElementAlreadyExistsException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="NoSuchElementException"></exception>
        public Board AddPointerToJoinedBoard(CIString email,int board_id);

        /// <summary>
        /// Removes the pointer of the joined board from the user's JoinedBoards<br/><br/>
        /// <b>Throws </b> <c>UserDoesNotExistException</c> if the user doesn't exist in the system<br/>
        /// <b>Throws </b> <c>ArgumentException</c> if the user is not joined on a board with that id<br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns>The unjoined board</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Board RemovePointerToJoinedBoard(CIString email,int board_id);

        /// <summary>
        /// Removes a <c>Board</c> from the <b>entire system</b><br/>
        /// <br/><br/>
        /// <b>Throws </b> <c>NoSuchElementException</c> if,  for some reason, a board with that id <br/>
        /// doesn't exist in the system in general or specifically for its owner<br/><br/>
        /// <b>Throws </b> <c>ArgumentException</c> if, for some reason, a board with that id doesn't<br/>
        /// exist for any of the joined users<br/><br/>
        /// <b>Throws </b> <c>UserDoesNotExistException</c> if, for some reason, one of the board's joined users doesn't exist <br/>
        /// in the system
        /// </summary>
        /// <exception cref="NoSuchElementException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UserDoesNotExistException"></exception>
        public void NukeBoard(CIString email, CIString board_title);

        ///// <summary>
        /////  Completly removes a <c>Board</c> from the <b>entire system</b><br/>
        ///// <br/><br/>
        ///// <b>Throws </b> <c>NoSuchElementException</c> if,  for some reason, a board with that id <br/>
        ///// doesn't exist in the system in general or specifically for its owner<br/><br/>
        ///// <b>Throws </b> <c>ArgumentException</c> if, for some reason, a board with that id doesn't<br/>
        ///// exist for any of the joined users<br/><br/>
        ///// <b>Throws </b> <c>UserDoesNotExistException</c> if, for some reason, one of the board's joined users doesn't exist <br/>
        ///// in the system
        ///// </summary>
        ///// <exception cref="NoSuchElementException"></exception>
        ///// <exception cref="ArgumentException"></exception>
        ///// <exception cref="UserDoesNotExistException"></exception>
        //public void NukeBoard(int board_id);

        /// <summary>
        /// Remove pointer from old owner and add pointer to new owner<br/><br/>
        /// <b>Throws </b> <c>ElementAlreadyExistsException</c> if a board with that title already exists for the newOwner<br/><br/>
        /// <b>Throws </b> <c>UserDoesNotExistException</c> if a user with that email doesn't exist<br/><br/>
        /// <b>Throws </b> <c>NoSuchElementException</c> if a <c>Board</c> with that title <br/>
        /// doesn't exist for the oldOwner<br/><br/>
        /// </summary>
        /// <param name="oldOwner"></param>
        /// <param name="boardName"></param>
        /// <param name="newOwner"></param>
        public void ChangeOwnerPointer(CIString old_owner, CIString board_title, CIString new_owner);

        /// <summary>
        /// Checks whether or not a user owns a board with the specified title
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <returns>true is yes, false if no</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        public bool UserOwnsABoardWithThisTitle(CIString email, CIString title);

        /// <summary>
        /// Checks whether or not a user is joined to a board with the specified title
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <returns>true is yes, false if no</returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        public bool UserIsJoinedToABoardWithThisTitle(CIString email, CIString title);
    }
    public interface DataCenterManagement
    {
        /// <summary>
        /// This method loads all persisted data into memory
        /// </summary>
        public void LoadData();

        /// <summary>
        /// <b>WARNING: this method deletes all persisted and loaded data</b>
        /// </summary>
        public void DeleteData();
    }
}