using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.Tests
{
    [TestClass()]
    public class TaskControllerDTOTests
    {
        UserService userService;
        BoardControllerService boardControllerService;
        BoardService boardService;
        TaskService taskService;
        SQLExecuter executer;

        public TaskControllerDTOTests()
        {
            BusinessLayer.BusinessLayerFactory.GetInstance().DataCenterManagement.DeleteData();
            ServiceLayerFactory.DeleteEverything();
            ServiceLayerFactory ServiceFactory = ServiceLayerFactory.GetInstance();
            DataAccessLayerFactory DataFactory = DataAccessLayerFactory.GetInstance();
            userService = ServiceFactory.UserService;
            boardService = ServiceFactory.BoardService;
            boardControllerService = ServiceFactory.BoardControllerService;
            taskService = ServiceFactory.TaskService;
            executer = DataFactory.SQLExecuter;
        }


        [TestMethod()]
        public void AddTaskTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            string taskTitle = "task1";
            string description = "desc1";
            DateTime dueDate = new DateTime(2024, 05, 20);
            if (GetOperationState(userService.Register(email, password))==false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardControllerService.AddBoard(email, boardName))==false)
                Assert.Fail("AddBoard failed");
            string result = taskService.AddTask(email, boardName, taskTitle, description, dueDate);
            string query = "SELECT * FROM Tasks WHERE TaskId=0";
            if (GetOperationState(result) == false) Assert.Fail(result);
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }

        [TestMethod()]
        public void RemoveTaskTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            string taskTitle = "task1";
            string description = "desc1";
            DateTime dueDate = new DateTime(2023,05,23);
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardControllerService.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            if (GetOperationState(taskService.AddTask(email, boardName, taskTitle, description, dueDate))==false)
                Assert.Fail("AddTask failed"); ;
            string result = taskService.RemoveTask(email, boardName, 0);
            string query = "SELECT * FROM Tasks WHERE BoardId=0 AND TaskId=0";
            if (GetOperationState(result) == false) Assert.Fail(result);
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count != 0) Assert.Fail("rows were fetched");
        }

        [TestMethod()]
        public void ChangeTaskStateTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            string taskTitle = "task1";
            string description = "desc1";
            DateTime dueDate = new DateTime(2025,05,20);
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardControllerService.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            if (GetOperationState(taskService.AddTask(email, boardName, taskTitle, description, dueDate)) == false)
                Assert.Fail("AddTask failed");
            if (GetOperationState(taskService.AssignTask(email, boardName, 0, 0, email)) == false)
                Assert.Fail("AssignTask failed");
            string result = taskService.AdvanceTask(email, boardName, 0, 0);
            string query = "SELECT * FROM Tasks WHERE State = 1";
            if (GetOperationState(result) == false) Assert.Fail(result);
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }

        [TestMethod()]
        public void ChangeTitleTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            string taskTitle = "task1";
            string description = "desc1";
            string newTitle = "newTitle";
            DateTime dueDate = new DateTime(2023,5,20);
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardControllerService.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            if (GetOperationState(taskService.AddTask(email, boardName, taskTitle, description, dueDate)) == false)
                Assert.Fail("AddTask failed");
            if (GetOperationState(taskService.AssignTask(email, boardName, 0, 0, email)) == false)
                Assert.Fail("AssignTask failed");
            string result = taskService.UpdateTaskTitle(email, boardName, 0, 0, newTitle);
            string query = $"SELECT * FROM Tasks WHERE TaskTitle='{newTitle}'";
            if (GetOperationState(result) == false) Assert.Fail(result);
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }

        [TestMethod()]
        public void ChangeDescriptionTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            string taskTitle = "task1";
            string description = "desc1";
            string newDescription = "desc2";
            DateTime dueDate = new DateTime(2023,05,20);
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardControllerService.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            if (GetOperationState(taskService.AddTask(email, boardName, taskTitle, description, dueDate)) == false)
                Assert.Fail("AddTask failed");
            if (GetOperationState(taskService.AssignTask(email, boardName, 0, 0, email)) == false)
                Assert.Fail("AssignTask failed");
            string result = taskService.UpdateTaskDescription(email, boardName, 0, 0, newDescription);
            string query = $"SELECT * FROM Tasks WHERE Description='{newDescription}'";
            if (GetOperationState(result) == false) Assert.Fail(result);
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }

        [TestMethod()]
        public void ChangeAssigneeTestOwner()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            string taskTitle = "task1";
            string description = "desc1";;
            DateTime dueDate = new DateTime(2023,05,20);
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardControllerService.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            if (GetOperationState(taskService.AddTask(email, boardName, taskTitle, description, dueDate)) == false)
                Assert.Fail("AddTask failed");
            if (GetOperationState(taskService.AddTask(email, boardName, taskTitle, description, dueDate)) == false)
                Assert.Fail("AddTask failed");
            string result = taskService.AssignTask(email, boardName, 0, 0, email);
            string query = $"SELECT * FROM Tasks WHERE Assignee='{email}'";
            if (GetOperationState(result) == false) Assert.Fail(result);
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }

        [TestMethod()]
        public void ChangeAssigneeTestNewAssignee()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string email2 = "Hadaspr100@gmail.com";
            string password2 = "Printz1234";
            string boardName = "board1";
            string taskTitle = "task1";
            string description = "desc1";
            DateTime dueDate = new DateTime(2024,05,20);
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardControllerService.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            if (GetOperationState(taskService.AddTask(email, boardName, taskTitle, description, dueDate)) == false)
                Assert.Fail("AddTask failed");
            if (GetOperationState(taskService.AssignTask(email, boardName, 0, 0, email))==false)
                Assert.Fail("AssignTask failed");
            if(GetOperationState(userService.Register(email2, password2))==false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardService.JoinBoard(email2, 0)) == false)
                Assert.Fail("JoinBoard failed");
            string result = taskService.AssignTask(email, boardName, 0, 0, email2); 
            string query = $"SELECT * FROM Tasks WHERE Assignee = '{email2}'";
            if (GetOperationState(result) == false) Assert.Fail(result);
            LinkedList<object[]> list = executer.ExecuteRead(query);
            if (list.Count == 0) Assert.Fail("No rows were fetched");
        }

        [TestMethod()]
        public void ChangeDueDateTest()
        {
            string email = "printz@post.bgu.ac.il";
            string password = "Hadas1234";
            string boardName = "board1";
            string taskTitle = "task1";
            string description = "desc1";
            DateTime dueDate = new DateTime(2025, 05, 20);
            DateTime dueDate2 = new DateTime(2024, 05, 20);
            if (GetOperationState(userService.Register(email, password)) == false)
                Assert.Fail("Register failed");
            if (GetOperationState(boardControllerService.AddBoard(email, boardName)) == false)
                Assert.Fail("AddBoard failed");
            if (GetOperationState(taskService.AddTask(email, boardName, taskTitle, description, dueDate)) == false)
                Assert.Fail("AddTask failed");
            if (GetOperationState(taskService.AssignTask(email, boardName, 0, 0, email)) == false)
                Assert.Fail("AssignTask failed");
            string result = taskService.UpdateTaskDueDate(email, boardName, 0, 0, dueDate2);
            string query = $"SELECT * FROM Tasks WHERE DueDate='{dueDate2}'";
            if (GetOperationState(result) == false) Assert.Fail(result);
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