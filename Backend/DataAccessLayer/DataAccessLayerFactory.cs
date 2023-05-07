
namespace IntroSE.Kanban.Backend.DataAccessLayer
{


    /// <summary>
    /// A factory for the data access layer classes<br/>
    /// This factory implements the singleton pattern<br/>
    /// use GetInstance() to get an instance of this factory.<br/><br/>
    /// <b>Note:</b> this factory instantiates each class excactly once and does not produce duplicates
    /// <code>Inventory:</code>
    /// <list type="bullet">
    /// <item>BoardControllerDTO</item>
    /// <item>TaskControllerDTO</item>
    /// <item>UserControllerDTO</item>
    /// <item>Executer</item>
    /// </list>
    /// </summary>
    public class DataAccessLayerFactory
    {
        private static DataAccessLayerFactory instance = null;
        private BoardControllerDTO boardControllerDTO;
        private TaskControllerDTO taskControllerDTO;
        private UserControllerDTO userControllerDTO;
        private SQLExecuter executer;

        private DataAccessLayerFactory()
        {
            executer = new();
            boardControllerDTO = new(executer);
            taskControllerDTO = new(executer);
            userControllerDTO = new(executer);      
        }

        /// <summary>
        /// Retrieves the BoardControllerDTO instance
        /// </summary>
        public BoardControllerDTO BoardControllerDTO => boardControllerDTO;

        /// <summary>
        /// Retrieves the TaskControllerDTO instance
        /// </summary>
        public TaskControllerDTO TaskControllerDTO => taskControllerDTO;

        /// <summary>
        /// Retrieves the UserControllerDTO instance
        /// </summary>
        public UserControllerDTO UserControllerDTO => userControllerDTO;

        /// <summary>
        /// Retrieves the SQLExecuter instance
        /// </summary>
        public SQLExecuter SQLExecuter => executer;

        /// <summary>
        /// Retrieves a DataLoader instance.<br/><br/>
        /// instantiates a new instance every time this is called
        /// </summary>
        public DataLoader DataLoader => new DataLoader(executer);

        public static DataAccessLayerFactory GetInstance()
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
        }

    }
}
