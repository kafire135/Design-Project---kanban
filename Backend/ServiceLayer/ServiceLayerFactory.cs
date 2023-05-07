
namespace IntroSE.Kanban.Backend.ServiceLayer
{


    /// <summary>
    /// A factory for the service layer classes<br/>
    /// This factory implements the singleton pattern<br/>
    /// use GetInstance() to get an instance of this factory.<br/><br/>
    /// <b>Note:</b> this factory instantiates each class exactly once and does not produce duplicates
    /// <code>Inventory:</code>
    /// <list type="bullet">
    /// <item>BoardControllerService</item>
    /// <item>BoardService</item>
    /// <item>TaskService</item>
    /// <item>UserService</item>
    /// </list>
    /// </summary>
    public class ServiceLayerFactory
    {
        private static ServiceLayerFactory instance = null;

        private BoardControllerService boardControllerService;
        private BoardService boardService;
        private TaskService taskService;
        private UserService userService;
        private BackendInitializer initializer;

        private ServiceLayerFactory() 
        {
            initializer = new();
            BusinessLayer.BusinessLayerFactory BLFactory = BusinessLayer.BusinessLayerFactory.GetInstance();
            boardControllerService = new(BLFactory.BoardController);
            boardService = new(BLFactory.BoardController);
            taskService = new(BLFactory.TaskController);
            userService = new(BLFactory.UserController);
        }

        /// <summary>
        /// Get the BoardControllerService instance
        /// </summary>
        public BoardControllerService BoardControllerService => boardControllerService;

        /// <summary>
        /// Get the BoardService instance
        /// </summary>
        public BoardService BoardService => boardService;

        /// <summary>
        /// Get the TaskService instance
        /// </summary>
        public TaskService TaskService => taskService;

        /// <summary>
        /// Get the TaskService instance
        /// </summary>
        public UserService UserService => userService;

        /// <summary>
        /// Get the BackendInitializer instance
        /// </summary>
        public BackendInitializer BackendInitializer => initializer;


        /// <summary>
        /// Get the factory instance
        /// </summary>
        public static ServiceLayerFactory GetInstance()
        {
            if (instance == null) instance = new();
            return instance;
        }

        /// <summary>
        /// Resets the factory instance to null <br/><br/>
        /// <b>WARNING: USED FOR UNIT TESTING PURPOSES ONLY</b>
        /// 
        /// </summary>
        public static void DeleteEverything()
        {
            instance = null;
            BusinessLayer.BusinessLayerFactory.DeleteEverything();
        }


    }
}
