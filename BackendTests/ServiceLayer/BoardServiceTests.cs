using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntroSE.Kanban.Backend.ServiceLayer.Tests
{
    [TestClass()]
    public class BoardServiceTests
    {
        UserService userservice;
        BoardControllerService boardcontrollerservice;
        BoardService boardservice;
        TaskService taskservice;

        public BoardServiceTests()
        {
            BusinessLayerFactory.GetInstance().DataCenterManagement.DeleteData();
            ServiceLayerFactory.DeleteEverything();
            ServiceLayerFactory factory = ServiceLayerFactory.GetInstance();
            userservice = factory.UserService;
            boardcontrollerservice = factory.BoardControllerService;
            boardservice = factory.BoardService;
            taskservice = factory.TaskService;
        }

        [TestMethod()]
        public void JoinBoardTestSuccess()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true, ""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            result = userservice.LogIn("Printz@post.bgu.ac.il", "Ha12345");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void JoinBoardTest_user_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'Printz@post.bgu.ac.il' doesn't exist in the system"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void JoinBoardTest_user_not_logged_in()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'Printz@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            userservice.LogOut("Printz@post.bgu.ac.il");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void JoinBoardTest_board_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "Board number '1' doesn't exist"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            result = userservice.LogIn("Printz@post.bgu.ac.il", "Ha12345");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 1);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void JoinBoardTest_user_already_joined()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "the user Printz@post.bgu.ac.il is already joined to the board"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            result = userservice.LogIn("Printz@post.bgu.ac.il", "Ha12345");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void JoinBoardTest_user_is_the_owner()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "the user 'kfirniss@post.bgu.ac.il' is the board's owner"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.JoinBoard("kfirniss@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void LeaveBoardTestSuccess()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true, ""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            result = userservice.LogIn("Printz@post.bgu.ac.il", "Ha12345");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            result = boardservice.LeaveBoard("Printz@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void LeaveBoardTest_user_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'Printz@post.bgu.ac.il' doesn't exist in the system"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            result = boardservice.LeaveBoard("Printz@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void LeaveBoardTest_user_not_logged_in()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'Printz@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            result = userservice.LogOut("Printz@post.bgu.ac.il");
            result = boardservice.LeaveBoard("Printz@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void LeaveBoardTest_Leave_board_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "Board number '1' doesn't exist"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            result = userservice.LogIn("Printz@post.bgu.ac.il", "Ha12345");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            result = boardservice.LeaveBoard("Printz@post.bgu.ac.il", 1);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void LeaveBoardTest_leave_board_user_didnt_join_the_board()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user with email 'Printz@post.bgu.ac.il' is not joined to the board"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            result = userservice.LogIn("Printz@post.bgu.ac.il", "Ha12345");
            result = boardservice.LeaveBoard("Printz@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void LeaveBoardTest_user_is_the_owner()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' is the board's owner"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.JoinBoard("kfirniss@post.bgu.ac.il", 0);
            result = boardservice.LeaveBoard("kfirniss@post.bgu.ac.il", 0);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ChangeOwnerTestSuccess()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true, ""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            result = userservice.LogIn("Printz@post.bgu.ac.il", "Ha12345");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            result = boardservice.ChangeOwner("kfirniss@post.bgu.ac.il", "Printz@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ChangeOwnerTest_user_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = boardservice.ChangeOwner("kfirniss@post.bgu.ac.il", "Printz@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ChangeOwnerTest_user_not_logged_in()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = boardservice.ChangeOwner("kfirniss@post.bgu.ac.il", "Printz@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ChangeOwnerTest_board_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'another board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.ChangeOwner("kfirniss@post.bgu.ac.il", "Printz@post.bgu.ac.il", "another board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ChangeOwnerTest_user_dont_joined_to_the_board()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user Printz@post.bgu.ac.il isn't joined to the board and can't be made owner"));
            userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            string result = boardservice.ChangeOwner("kfirniss@post.bgu.ac.il", "Printz@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ChangeOwnerTest_user_is_already_the_owner()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true, ""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.ChangeOwner("kfirniss@post.bgu.ac.il", "kfirniss@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        //successful
        [TestMethod()]
        public void AddTaskTestSuccess()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true,""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            Assert.AreEqual(expected, result);
            

        }

        //user doesn't exist
        [TestMethod()]
        public void AddTaskTestUserDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            Assert.AreEqual(expected, result);
        }

        //user doesn't login
        [TestMethod()]
        public void AddTaskTestUserIsntLoggedIn()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            Assert.AreEqual(expected, result);
        }

        //board isn't exist
        [TestMethod()]
        public void AddTaskTestBoardDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'new board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            Assert.AreEqual(expected, result);
        }

        //column can't over the limit
        [TestMethod()]
        public void AddTaskTestColumnCantOverLimit()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "Backlog in board 'new board' has reached its limit and can't contain more tasks"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 0, 1);
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "ni ni ni", new DateTime(2023, 05, 20));
            Assert.AreEqual(expected, result);
        }

        //successful
        [TestMethod()]
        public void RemoveTaskTestSuccess()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true,""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            result = taskservice.RemoveTask("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //user doesn't exist
        [TestMethod()]
        public void RemoveTaskTestUserDoenstExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = taskservice.RemoveTask("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //user doesn't login
        [TestMethod()]
        public void RemoveTaskTestUserIsntLoggedIn()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = taskservice.RemoveTask("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //board isn't exist
        [TestMethod()]
        public void RemoveTaskTestBoarsIsntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'new board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = taskservice.RemoveTask("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //task isn't exist
        [TestMethod()]
        public void RemoveTaskTestTaskIsntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A Task with the taskId '0' doesn't exist in the Board"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.RemoveTask("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //successful
        [TestMethod()]
        public void AdvanceTaskTestSuccess()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true, ""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            taskservice.AssignTask("kfirniss@post.bgu.ac.il", "new board", 0, 0, "kfirniss@post.bgu.ac.il");
            result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 0, 0);
            Assert.AreEqual(expected, result);
        }

        //user doesn't exist
        [TestMethod()]
        public void AdvanceTaskTestUsetDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 0, 0);
            Assert.AreEqual(expected, result);
        }

        //user doesn't login
        [TestMethod()]
        public void AdvanceTaskTestUserIsntLoggedIn()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 0, 0);
            Assert.AreEqual(expected, result);
        }

        //board isn't exist
        [TestMethod()]
        public void AdvanceTaskTestBoardDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'new board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 0, 0);
            Assert.AreEqual(expected, result);
        }

        //task isn't exist in the column
        [TestMethod()]
        public void AdvanceTaskTestTaskDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A Task with the taskId '0' doesn't exist in the Board"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 0, 0);
            Assert.AreEqual(expected, result);
        }

        //task is done
        [TestMethod()]
        public void AdvanceTaskTestTaskIsDone()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "task numbered '0' is done and can't be advanced"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            taskservice.AssignTask("kfirniss@post.bgu.ac.il", "new board", 0, 0, "kfirniss@post.bgu.ac.il");
            result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 0, 0);
            result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 1, 0);
            result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 2, 0);
            Assert.AreEqual(expected, result);
        }

        //column can't over the limit
        [TestMethod()]
        public void AdvanceTaskTestColumnCantOverTheLimit()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "task numbered '1' can't be advanced because the next column is full"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 2", "ni ni ni", new DateTime(2023, 05, 20));
            taskservice.AssignTask("kfirniss@post.bgu.ac.il", "new board", 0, 0, "kfirniss@post.bgu.ac.il");
            taskservice.AssignTask("kfirniss@post.bgu.ac.il", "new board", 0, 1, "kfirniss@post.bgu.ac.il");
            result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 1, 1);
            result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 0, 0);
            result = taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 0, 1);
            Assert.AreEqual(expected, result);
        }

        //successful
        [TestMethod()]
        public void LimitColumnTestSucces()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true,""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 0, 1);
            Assert.AreEqual(expected, result);
        }

        //user isn't exist
        [TestMethod()]
        public void LimitColumnTestUserDoentExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 0, 1);
            Assert.AreEqual(expected, result);
        }

        //user doesn't login
        [TestMethod()]
        public void LimitColumnTestUserIsntLoggedIn()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 0, 1);
            Assert.AreEqual(expected, result);
        }

        //borard isn't exist
        [TestMethod()]
        public void LimitColumnTestBoardDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'new board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 0, 1);
            Assert.AreEqual(expected, result);
        }

        //column isn't exist
        [TestMethod()]
        public void LimitColumnTestColumnDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "The column '5' is not a valid column number"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 5, 1);
            Assert.AreEqual(expected, result);
        }

        //column has more tasks than the limit
        [TestMethod()]
        public void LimitColumnTestColumnHasMoreTasksThanLimit()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A column 'backlog' size is bigger than th limit 1"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 2", "ni ni ni", new DateTime(2023, 05, 20));
            result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 0, 1);
            Assert.AreEqual(expected, result);
        }

        //successful
        [TestMethod()]
        public void GetColumnLimitTestSuccess()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<object>(true,1));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 0, 1);
            result = boardservice.GetColumnLimit("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //user isn't exist
        [TestMethod()]
        public void GetColumnLimitTestUserDoentExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = boardservice.GetColumnLimit("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //user doesn't login
        [TestMethod()]
        public void GetColumnLimitTestUserIsntLoggedIn()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = boardservice.GetColumnLimit("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //board isn't exist
        [TestMethod()]
        public void GetColumnLimitTestBoardDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'new board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardservice.GetColumnLimit("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //column isn't exist
        [TestMethod()]
        public void GetColumnLimitTestColumnDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "The column '7' is not a valid column number"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.LimitColumn("kfirniss@post.bgu.ac.il", "new board", 0, 1);
            result = boardservice.GetColumnLimit("kfirniss@post.bgu.ac.il", "new board", 7);
            Assert.AreEqual(expected, result);
        }

        //column is unlimited
        [TestMethod()]
        public void GetColumnLimitTestColumnIsUnlimited()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<int>(true, -1));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.GetColumnLimit("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //successful
        [TestMethod()]
        public void GetColumnNameTestSuccess()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true,"backlog"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.GetColumnName("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //user isn't exist
        [TestMethod()]
        public void GetColumnNameTestUserDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = boardservice.GetColumnName("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //user doesn't login
        [TestMethod()]
        public void GetColumNameTestUserIsntLoggedIn()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = boardservice.GetColumnName("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //board isn't exist
        [TestMethod()]
        public void GetColumnNameTestBoardDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'new board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardservice.GetColumnName("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //column isn't exist
        [TestMethod()]
        public void GetColumnNameTestColumnDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "The column '7' is not a valid column number"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardservice.GetColumnName("kfirniss@post.bgu.ac.il", "new board", 7);
            Assert.AreEqual(expected, result);
        }

        //successful
        [TestMethod()]
        public void GetColumnTestSuccess()
        {
            LinkedList<BusinessLayer.Task> tasks = new LinkedList<BusinessLayer.Task>();
            tasks.AddLast(new BusinessLayer.Task(0, new Utilities.CIString("task 1") , new DateTime(2023, 05, 20), new Utilities.CIString("bla bla bla"), 0));

            string expected = JsonEncoder.ConvertToJson(new Response<LinkedList<BusinessLayer.Task>>(true,tasks));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            result = boardservice.GetColumn("kfirniss@post.bgu.ac.il", "new board", 0);
            Response<LinkedList<BusinessLayer.Task>> exp = JsonEncoder.BuildFromJson<Response<LinkedList<BusinessLayer.Task>>>(expected);
            Response<LinkedList<BusinessLayer.Task>> act = JsonEncoder.BuildFromJson<Response<LinkedList<BusinessLayer.Task>>>(result);
            BusinessLayer.Task tExp = exp.returnValue.ElementAt(0);
            BusinessLayer.Task tAct = act.returnValue.ElementAt(0);
            Assert.IsFalse(tExp.Id != tAct.Id || tExp.Title != tAct.Title || tExp.Description != tAct.Description || tExp.DueDate != tAct.DueDate);
        }

        //user isn't exist
        [TestMethod()]
        public void GetColumnTestUserDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = boardservice.GetColumn("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //user doesn't login
        [TestMethod()]
        public void GetColumnTestUserIsntLoggedIn()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = boardservice.GetColumn("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //board isn't exist
        [TestMethod()]
        public void GetColumnTestBoardDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'new board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardservice.GetColumn("kfirniss@post.bgu.ac.il", "new board", 0);
            Assert.AreEqual(expected, result);
        }

        //column isn't exist
        [TestMethod()]
        public void GetColumnTestColumnDoesntExist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "The column '5' is not a valid column number"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            result = boardservice.GetColumn("kfirniss@post.bgu.ac.il", "new board", 5);
            Assert.AreEqual(expected, result);
        }
    }
}