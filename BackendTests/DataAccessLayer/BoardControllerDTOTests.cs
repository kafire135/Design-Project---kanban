using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.Utilities;

namespace IntroSE.Kanban.Backend.DataAccessLayer.Tests
{
    [TestClass()]
    public class BoardControllerDTOTests
    {
        BoardControllerService service;
        UserService userService;
        BoardService boardService;
        SQLExecuter executer;
        BusinessLayer.BoardDataOperations dataOperations;


        public BoardControllerDTOTests()
        {
            Backend.BusinessLayer.BusinessLayerFactory.GetInstance().DataCenterManagement.DeleteData();
            ServiceLayerFactory.DeleteEverything();
            ServiceLayerFactory ServiceFactory = ServiceLayerFactory.GetInstance();
            DataAccessLayerFactory DataFactory = DataAccessLayerFactory.GetInstance();
            service = ServiceFactory.BoardControllerService;
            userService = ServiceFactory.UserService;
            boardService = ServiceFactory.BoardService;
            executer = DataFactory.SQLExecuter;
            dataOperations = BusinessLayer.BusinessLayerFactory.GetInstance().BoardDataOperations;
        }

        [TestMethod()]
        public void AddBoardTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            string result = service.AddBoard(email, boardName);
            string query = $"SELECT * FROM Boards WHERE Owner='{email}'";
            if (GetOperationState(result) == false) Assert.Fail("operationState is false");
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }

        //remove board of owner
        [TestMethod()]
        public void RemoveBoardTestOwner()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(service.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            string result = service.RemoveBoard(email, boardName);
            string query = $"SELECT * FROM Boards WHERE Owner='{email}' AND BoardTitle='{boardName}'";
            if (GetOperationState(result) == false) Assert.Fail("operationState is false");
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count != 0) Assert.Fail("No rows were fetched");
        }

        //remove board of joined
        [TestMethod()]
        public void RemoveBoardTestJoined()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            string email2 = "hadaspr100@gmail.com";
            string password2 = "Printz1234";
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(service.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            if (GetOperationState(userService.Register(email2, password2)) == false)
                Assert.Fail("Register failed");
            int boardId = dataOperations.SearchBoardByEmailAndTitle(new CIString(email), new CIString(boardName)).Id;
            if (GetOperationState(boardService.JoinBoard(email2, boardId)) == false)
                Assert.Fail("JoinBoard failed");
            string result = service.RemoveBoard(email, boardName);
            string query = $"SELECT * FROM UserJoinedBoards WHERE BoardId={boardId};";
            if (GetOperationState(result) == false) Assert.Fail("operationState is false");
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count != 0) Assert.Fail("rows were fetched");
        }

        [TestMethod()]
        public void JoinBoardTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string email2 = "hadaspr100@gmail.com";
            string password2 = "Printz1234";
            string boardName = "board1";
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(service.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            int boardId = dataOperations.SearchBoardByEmailAndTitle(new CIString(email), new CIString(boardName)).Id;
            if (GetOperationState(userService.Register(email2, password2)) == false)
                Assert.Fail("Register failed");
            string result = boardService.JoinBoard(email2, boardId);
            string query = $"SELECT * FROM UserJoinedBoards WHERE BoardId='{boardId}';";
            if (GetOperationState(result) == false) Assert.Fail("operationState is false");
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }

        [TestMethod()]
        public void LeaveBoardTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string email2 = "hadaspr100@gmail.com";
            string password2 = "Printz1234";
            string boardName = "board1";
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(service.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            int boardId = dataOperations.SearchBoardByEmailAndTitle(new CIString(email), new CIString(boardName)).Id;
            if (GetOperationState(userService.Register(email2, password2)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(ServiceLayerFactory.GetInstance().BoardService.JoinBoard(email2, boardId)) == false)
                Assert.Fail("JoinBoard failed");
            string result = boardService.LeaveBoard(email2, boardId);
            string query = $"SELECT * FROM UserJoinedBoards WHERE BoardId='{boardId}';";
            if (GetOperationState(result) == false) Assert.Fail("operationState is false");
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count != 0) Assert.Fail("No rows were fetched");
        }

        [TestMethod()]
        public void ChangeOwnerTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string email2 = "hadaspr100@gmail.com";
            string password2 = "Printz1234";
            string boardName = "board1";
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(service.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            int boardId = dataOperations.SearchBoardByEmailAndTitle(new CIString(email), new CIString(boardName)).Id;
            if (GetOperationState(userService.Register(email2, password2)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardService.JoinBoard(email2, boardId)) == false)
                Assert.Fail("JoinBoard failed");
            string result = boardService.ChangeOwner(email, email2 , boardName);
            string query = $"SELECT * FROM Boards WHERE Owner='{email2}'";
            if (GetOperationState(result) == false) Assert.Fail("operationState is false");
            LinkedList<object[]> list = executer.ExecuteRead(query);     
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }

        [TestMethod()]
        public void LimitColumnTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(service.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            string result = boardService.LimitColumn(email, boardName, 0, 20);
            string query = $"SELECT * FROM Boards WHERE BacklogLimit=20";
            if (GetOperationState(result) == false) Assert.Fail("operationState is false");
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }


        private static bool GetOperationState(string json)
        {
            Response<object> res = JsonEncoder.BuildFromJson<Response<object>>(json);
            if (res.operationState == true)
            {
                return true;
            }
            else return false;
        }


    }
}
