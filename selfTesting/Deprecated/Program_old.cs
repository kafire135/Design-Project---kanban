//using System.Text.Json;
//using System.IO;
//using IntroSE.Kanban.Backend.BusinessLayer;
//using IntroSE.Kanban.Backend.DataAccessLayer;
//using IntroSE.Kanban.Backend.ServiceLayer;

////[assembly: log4net.Config.XmlConfigurator(ConfigFile ="log4net.config" ,Watch = true)]
//namespace IntroSE.Kanban.selfTesting
//{
//    public class Program_old
//    {

//        //============== READ ME ==================

//        // This is an executable project to test code during development 

//        //============== READ ME ================== 

//        private static log4net.ILog log = log4net.LogManager.GetLogger("selfTesting\\Program.cs");

//        public static void Main(string[] args)
//        {
//            //DebugAVLTree(new int[]{ 358, -327, -451, 46, 170, 274, -372, 151 });
//            //AVLTreeTesting();
//            //test01();
//            //BoardTreeTesting();
//            //datetesting();
//            //UserControllerTesting();
//            //JsonTesting();
//            //PasswordHashingTesting();
//            //logTesting();
//            //registerTest();
//            //validEmailTest();
//            //enumsTests();
//            //gradingTests();
//            //tests();
//            //enumeratorTests();
//            //counterTest();
//            //PathTest();
//            //login();
//            //testResponse();
//            //getInProgress();
//        }
//        public static void DebugAVLTree(int[] nums)
//        {
//                int count = nums.Length;
//                AVLTree<int, int> tree1 = new();
//                for (int i = 0; i < count;)
//                {
//                    try
//                    {
//                        tree1.Add(nums[i], 6);
//                        i++;
//                    }
//                    catch (ArgumentException) { }
//                }
//                Console.Write("{ ");
//                foreach (int o in nums)
//                {
//                    Console.Write(o + ", ");
//                }
//                Console.WriteLine("};");
//                Console.WriteLine("==========================================================");
//                for (int i = 0; i < 0; i++)
//                    tree1.Remove(nums[i]);

//                tree1.PrintTree();
//                Console.WriteLine("==========================================================");
//                for (int i = 0; i < count; i++)
//                {
//                    try
//                    {
//                        Console.WriteLine("Remove for: " + nums[i]);
//                        Console.WriteLine();
//                        tree1.PrintTree();
//                        Console.WriteLine();
//                        tree1.Remove(nums[i]);
//                    }
//                    catch (NoSuchElementException) {  }


//                    Console.WriteLine("==========================================================");
//                }
//                tree1.PrintTree();
//        }

//        public static void AVLTreeTesting()
//        {
//            int successCounter = 0;
//            while (true)
//            {
//                int count = 300;
//                AVLTree<int, int> tree1 = new();
//                Random random = new Random();
//                int[] nums = new int[count];
//                for (int i = 0; i < count; )
//                {
//                    int num = random.Next(-1000, 1000);
//                    try
//                    {
//                        tree1.Add(num, 6);
//                        nums[i] = num;
//                        i++;
//                    }
//                    catch (ArgumentException) { }
//                }
//                //Console.Write("{ ");
//                //foreach (int o in nums)
//                //{
//                //    Console.Write(o + ", ");
//                //}
//                //Console.WriteLine("};");
//                //Console.WriteLine("==========================================================");
//                //for (int i = 0; i < 0; i++)
//                //    tree1.Remove(nums[i]);

//                //tree1.PrintTree();
//                //Console.WriteLine("==========================================================");
//                try
//                {
//                    for (int i = 0; i < count; i++)
//                    {
//                        try
//                        {
//                            //Console.WriteLine("Remove for: " + nums[i]);
//                            //Console.WriteLine();
//                            //tree1.PrintTree();
//                            //Console.WriteLine();
//                            tree1.Remove(nums[i]);
//                        }
//                        catch (Exception) { throw; }


//                        //Console.WriteLine("==========================================================");
//                    }
//                    Console.WriteLine("Instace " + successCounter + ": success.");
//                    successCounter++;
//                }
//                catch (Exception)
//                {
//                    Console.WriteLine("==========================================================");
//                    Console.WriteLine("Instace " + successCounter + ": FAIL.");
//                    Console.WriteLine();
//                    Console.Write("input: {");
//                    for (int i = 0 ;i < nums.Length; i++)
//                    {
//                        if(i !=  nums.Length-1)
//                            Console.Write(nums[i] + ", ");
//                        else Console.Write(nums[i]);
//                    }
//                    Console.WriteLine("}");
//                    Console.WriteLine();
//                    Console.WriteLine("Click enter to show the failed instance");
//                    Console.ReadLine();
//                    DebugAVLTree(nums);    
//                    break;
//                }  
//            }
//        }
//        public static void test01()
//        {
//            AVLTree<int, int> tree1 = new();
//            for (int i = 0; i < 8000000; i++)
//            {
//                tree1.Add(i, 0);
//            }
//            tree1.PrintTree();
//            Console.WriteLine("==========================================================");
//        }
//        public static void BoardTreeTesting()
//        {
//            Backend.BusinessLayer.User user1 = new Backend.BusinessLayer.User("yuval", "12345");
//            Backend.BusinessLayer.User user2 = new Backend.BusinessLayer.User("yuval2", "12345");
//            Backend.BusinessLayer.BoardTree tree = new Backend.BusinessLayer.BoardTree();

//            Backend.BusinessLayer.Board board = tree.AddBoard(user1, "test");
//            tree.RemoveBoard(user1, "test");
//            tree.GetAllBoards(user1);
//            tree.RemoveBoard(user1, "test");
//            tree.GetAllBoards(user1);
//        }
//        public static void DateTesting()
//        {
//            Backend.BusinessLayer.Date date = new Backend.BusinessLayer.Date("12.6.1998");
//            Console.WriteLine(date.day);
//        }

//        public static void UserControllerTesting()
//        {
//            //String pass = "afgHH123456";
//            //String pass2 = "asHddggdgdgd33!@";
//            //bool ans = Backend.BusinessLayer.UserController.IsLegalPassword(pass2);
//            //Console.WriteLine(ans);
//        }
//        public static void JsonTesting()
//        {
//            Board board = new("TestBoard", 0,"cc");
//            board.AddTask("TestTask0", new DateTime(2023, 1, 1), "Hello0");
//            board.AddTask("TestTask1", new DateTime(2023, 1, 1), "Hello1");
//            board.AddTask("TestTask2", new DateTime(2023, 1, 1), "Hello2");
//            board.AdvanceTask(0, 0);
//            board.AdvanceTask(1, 0);
//            board.AdvanceTask(0, 1);
//            //JsonSerializerOptions options = new() { WriteIndented = true };
//            //Console.WriteLine(JsonSerializer.Serialize(board, options));
//            Console.WriteLine(JsonController.ConvertToJson(board));
//        }
//        public static void logTesting()
//        {
//            log.Debug("Hello m8");
//        }
//        public static void registerTest()
//        {
//            Backend.ServiceLayer.GradingService gs = new();
//            gs.Register("test", "sismaSababa23");
//            Console.WriteLine(gs.Login("test", "sismaSababa23"));
//            Console.WriteLine(gs.Logout("test"));
//        }

//    public static void validEmailTest()
//        {
//            GradingService gs = new();
//            gs.Register("test", "sismaSababa23");
//            Console.WriteLine(gs.Login("test", "sismaSababa23"));
//            Console.WriteLine(gs.Logout("test"));
//            string email = "prinrz@post.bgu.ac.il";
//            string email1 = "Prein@pdij";
//            string email2 = "12344.@gmail.com"; //false/ true?
//            string email3 = "hadaspr100gmail.com";
//            string email4 = "hadas@gmailcom";
//            string email5 = null;
//            string email6 = "fdsa";
//            string email7 = "fdsa@";
//            string email8 = "fdsa@fdsa";
//            string email9 = "fdsa@fdsa.";
//            string email10 = "someone@somewhere.com";
//            string email11 = "someone@somewhere.co.uk";
//            string email12 = "someone+tag@somewhere.net"; // false/true?
//            string email13 = "futureTLD@somewhere.fooo";
//            bool ans = UserController.IsEmailValid(email13);
//            Console.WriteLine(ans);
//        }
//        public enum State
//        {
//            backlog,
//            inprogress,
//            done,
//        }
//        public static void enumsTests()
//        {
//            Console.WriteLine("hello: "+(int)State.backlog);
//        }

//        public static void gradingTests()
//        {
//            Backend.ServiceLayer.GradingService gs = new();
//            Console.WriteLine(gs.Register("blahblah@gmail.com", "SismaTil123"));
//            Console.WriteLine(gs.Login("blahblah@gmail.com", "SismaTil123"));
//            Console.WriteLine(gs.AddBoard("blahblah@gmail.com", "test"));
//            Console.WriteLine(gs.AddTask("blahblah@gmail.com", "test", "toDo_ZERO", "stam", new DateTime(2022, 5, 20)));
//            Console.WriteLine(gs.AddTask("blahblah@gmail.com", "test", "toDo_ONE", "stam", new DateTime(2022, 5, 20)));
//            Console.WriteLine(gs.AdvanceTask("blahblah@gmail.com", "test", 0, 0));
//            Console.WriteLine(gs.AdvanceTask("blahblah@gmail.com", "test", 0, 1));
//            Backend.ServiceLayer.GradingService.GradingResponse<LinkedList<Backend.BusinessLayer.Task>> res =
//                Backend.ServiceLayer.JsonController.BuildFromJson<Backend.ServiceLayer.
//                GradingService.GradingResponse<LinkedList<Backend.BusinessLayer.Task>>>(gs.InProgressTasks("blahblah@gmail.com"));
//            Console.WriteLine(gs.InProgressTasks("blahblah@gmail.com"));
//            LinkedList<Backend.BusinessLayer.Task> taskList = res.ReturnValue;
//            foreach (Backend.BusinessLayer.Task task in taskList)
//            {
//                Console.WriteLine(task.Id);
//            }
//        }

//        public static void tests()
//        {
//            GradingService gradingService = new ();
//            string email = "rrr@gmial.com";
//            string board = "one";
//            gradingService.Register(email, "Aka123k123");
//            gradingService.Login(email, "Aka123k123");
//            string invalid = "jgiosejiooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooojjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjoooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
//            gradingService.AddBoard(email, "one");
//            gradingService.AddTask(email, "one", "bRAND", "HELLOW WORLD", DateTime.Now);
//            gradingService.AddTask(email, "one", "new", "HELLOW WORLD", DateTime.Now);
//            gradingService.AdvanceTask(email, "one", 0, 0);
//            gradingService.AdvanceTask(email, "one", 0, 1);
//            gradingService.AdvanceTask(email, "one", 1, 0);
//            gradingService.AdvanceTask(email, "one", 1, 1);
//            gradingService.AddTask(email, "one", "new", "HELLOW WORLD", DateTime.Now);
//            gradingService.AdvanceTask(email, "one", 2, 0);
//            gradingService.AdvanceTask(email, "one", 0, 0); // no such task in column 0
//            gradingService.AdvanceTask(email, "one", 0, 2);
//            gradingService.InProgressTasks(email);
//            gradingService.LimitColumn(email, board, 1, 5);
//            gradingService.LimitColumn(email, board, 1, 4);
//            gradingService.LimitColumn(email, board, 1, 10);
//            gradingService.GetColumnLimit(email, board, 1);
//            gradingService.GetColumnName(email, board, 5); // INVALID NUMBER
//            gradingService.AddTask(email, "three", "new", "HELLOW WORLD", DateTime.Now); // no such board three
//            gradingService.UpdateTaskDueDate(email, "one", 1, 0, DateTime.Now);// not good , changes to task that not in true coloumn number
//            gradingService.UpdateTaskDueDate(email, "one", 9, 2, DateTime.Now);// not good , changes to invalid coloumn number
//            gradingService.RemoveBoard(email, "one");
//            gradingService.AddTask(email, "one", "new", "HELLOW WORLD", DateTime.Now);
//            gradingService.AddBoard(email, "two");
//            gradingService.AddTask(email, "two", "new", "HELLOW WORLD", DateTime.Now);
//            gradingService.UpdateTaskDueDate(email, "two", 0, 0, DateTime.Now);//
//            gradingService.UpdateTaskTitle(email, "two", 0, 0, "new title");//
//            gradingService.UpdateTaskTitle(email, "two", 0, 1, "new title");//no such task
//            gradingService.AdvanceTask(email, "two", 0, 0);
//            gradingService.UpdateTaskTitle(email, "two", 1, 0, "new title");//
//            gradingService.UpdateTaskTitle(email, "two", 1, 0, invalid);//invalid title
//            gradingService.UpdateTaskDescription(email, "two", 1, 0, "new descp");
//            gradingService.UpdateTaskDescription(email, "two", 1, 0, invalid);//
//            gradingService.LimitColumn(email, "two", 1, 1);
//            gradingService.AddTask(email, "two", "new task", "HELLOW WORLD", DateTime.Now);
//            Console.WriteLine(gradingService.AdvanceTask(email, "two", 0, 1)); // the column in full
//        }
//        public static void enumeratorTests()
//        {
//            AVLTree<int,User> tree = new();
//            tree.Add(5, new User("test5",""));
//            tree.Add(16, new User("test16", ""));
//            tree.Add(3, new User("test3", ""));
//            tree.Add(6, new User("test6", ""));
//            tree.Add(0, new User("test0", ""));

//            foreach (User i in tree)
//            {
//                Console.WriteLine(i.GetEmail());
//            }
//        }
//        public static void PathTest()
//        {
//            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
//            Console.WriteLine(path);
//        }
//        public static void login()
//        {
//            BusinessLayerFactory factory = BusinessLayerFactory.GetInstance();
//            UserService us = new(factory.UserController);
//            us.Register("TestEmail@post.bgu.ac.il","Coolpass1234");
//            us.LogOut("TestEmail@post.bgu.ac.il");
//            string json = us.LogIn("TestEmail@post.bgu.ac.il","Coolpass1234");
//            GradingResponse<string> response = new(json);
//            Console.WriteLine(JsonController.ConvertToJson(response));

//        }
//        public static void testResponse()
//        {
//            GradingService.GradingResponse2 res = new();
//            Console.WriteLine(JsonController.ConvertToJson(res));
//        }

//        public static void getInProgress()
//        {
//            GradingService gs = new();
//            gs.Register("TestEmail@post.bgu.ac.il", "Coolpass1234");
//            gs.AddBoard("TestEmail@post.bgu.ac.il", "test");
//            gs.AddTask("TestEmail@post.bgu.ac.il", "test", "1", "bla", new DateTime(2200, 10, 20));
//            gs.AddTask("TestEmail@post.bgu.ac.il", "test", "2", "blabla", new DateTime(2200,10,20));
//            gs.AdvanceTask("TestEmail@post.bgu.ac.il", "test", 0, 0);
//            gs.AdvanceTask("TestEmail@post.bgu.ac.il", "test", 0, 1);
//            Console.WriteLine(gs.GetColumn("TestEmail@post.bgu.ac.il", "test", 1));
//            string json = gs.GetColumn("TestEmail@post.bgu.ac.il", "test", 1);
//            GradingResponse<LinkedList<Backend.BusinessLayer.Task>> res =
//                JsonController.BuildFromJson<GradingResponse<LinkedList<Backend.BusinessLayer.Task>>>(json);
//            LinkedList<Backend.BusinessLayer.Task> list = res.ReturnValue;
//            foreach (Backend.BusinessLayer.Task task in list)
//            {
//                Console.WriteLine(task.CreationTime);
//            }
//        }


//    }

//}



