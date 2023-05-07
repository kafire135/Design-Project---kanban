using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using IntroSE.Kanban.Backend.Utilities;

namespace IntroSE.Kanban.Backend.ServiceLayer.Tests
{
    [TestClass()]
    public class BoardControllerServiceTests
    {
        UserService userservice;
        BoardControllerService boardcontrollerservice;
        BoardService boardservice;
        TaskService taskservice;

        public BoardControllerServiceTests()
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
        public void AddBoardTest_sucessful()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true,""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void AddBoardTest_user_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void AddBoardTest_user_not_logged_in()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result =boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void AddBoardTest_board_already_exists()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled new board already exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void RemoveBoardTest_successful()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(true,""));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardcontrollerservice.RemoveBoard("kfirniss@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void RemoveBoardTest1_user_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = boardcontrollerservice.RemoveBoard("kfirniss@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void RemoveBoardTest_user_not_logged_in()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardcontrollerservice.RemoveBoard("kfirniss@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void RemoveBoardTest_user_has_no_boards_to_delete()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'new board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.RemoveBoard("kfirniss@post.bgu.ac.il", "new board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void RemoveBoardTest_board_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A board titled 'other board' doesn't exists for the user with the email kfirniss@post.bgu.ac.il"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardcontrollerservice.RemoveBoard("kfirniss@post.bgu.ac.il", "other board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void RemoveBoardTest_user_isnt_owner()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user has not permission to do RemoveBoard"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = userservice.Register("Printz@post.bgu.ac.il", "Ha12345");
            result = boardservice.JoinBoard("Printz@post.bgu.ac.il", 0);
            result = boardcontrollerservice.RemoveBoard("Printz@post.bgu.ac.il", "new Board");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetAllTasksByStateTest_successful()
        {
            Task task1 = new(0, new CIString("task 1"), new DateTime(2023, 05, 20), new CIString("bla bla bla"),0);
            Task task2 = new(0, new CIString("task 2"), new DateTime(2023, 05, 20), new CIString("ninini"),1);
            userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "another board");
            taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2023, 05, 20));
            taskservice.AddTask("kfirniss@post.bgu.ac.il", "another board", "task 2", "ninini", new DateTime(2023, 05, 20));
            taskservice.AssignTask("kfirniss@post.bgu.ac.il", "new board", 0, 0, "kfirniss@post.bgu.ac.il");
            taskservice.AssignTask("kfirniss@post.bgu.ac.il", "another board", 0, 0, "kfirniss@post.bgu.ac.il");
            taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "new board", 0, 0);
            taskservice.AdvanceTask("kfirniss@post.bgu.ac.il", "another board", 0, 0);
            
            string result = boardcontrollerservice.GetInProgressTasks("kfirniss@post.bgu.ac.il");
            Response<LinkedList<Task>> act = JsonEncoder.BuildFromJson<Response<LinkedList<Task>>>(result);
            Task tAct1 = act.returnValue.ElementAt(0);
            Task tAct2 = act.returnValue.ElementAt(1);
            Assert.IsFalse(task1.Id != tAct1.Id || task1.Title.Equals(tAct1.Title)==false || task1.Description.Equals(tAct1.Description)==false || task1.DueDate.Equals(tAct1.DueDate)==false);
            Assert.IsFalse(task2.Id != tAct2.Id || task2.Title.Equals(tAct2.Title)==false || task2.Description.Equals(tAct2.Description)==false || task2.DueDate.Equals(tAct2.DueDate)==false);
        }

        [TestMethod()]
        public void GetAllTasksByStateTest_user_doesnt_exist()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "A user with the email 'kfirniss@post.bgu.ac.il' doesn't exist in the system"));
            string result = boardcontrollerservice.GetInProgressTasks("kfirniss@post.bgu.ac.il");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetAllTasksByStateTest_user_not_logged_in()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<string>(false, "user 'kfirniss@post.bgu.ac.il' isn't logged in"));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = userservice.LogOut("kfirniss@post.bgu.ac.il");
            result = boardcontrollerservice.GetInProgressTasks("kfirniss@post.bgu.ac.il");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetAllTasksByStateTest_user_has_no_boards()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<LinkedList<BusinessLayer.Task>> (true,new LinkedList<BusinessLayer.Task>()));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.GetInProgressTasks("kfirniss@post.bgu.ac.il");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetAllTasksByStateTest_user_has_no_task_in_the_state()
        {
            string expected = JsonEncoder.ConvertToJson(new Response<LinkedList<BusinessLayer.Task>>(true, new LinkedList<BusinessLayer.Task>()));
            string result = userservice.Register("kfirniss@post.bgu.ac.il", "Ha12345");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "new board");
            result = boardcontrollerservice.AddBoard("kfirniss@post.bgu.ac.il", "another board");
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "new board", "task 1", "bla bla bla", new DateTime(2022,05,20));
            result = taskservice.AddTask("kfirniss@post.bgu.ac.il", "another board", "task 2", "ninini", new DateTime(2022, 05, 20));
            result = boardcontrollerservice.GetInProgressTasks("kfirniss@post.bgu.ac.il");
            Assert.AreEqual(expected, result);
        }
    }
}