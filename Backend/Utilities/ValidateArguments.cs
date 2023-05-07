namespace IntroSE.Kanban.Backend.Utilities
{
    public class ValidateArguments
    {
        /// <summary>
        /// Checks whether or not the given arguments are null
        /// </summary>
        /// <param name="args"></param>
        /// <returns>true if none of the arguments are null, false if at least one is null</returns>
        public static bool ValidateNotNull(object[] args)
        {
            foreach (object arg in args)
            {
                if (arg == null) return false;
            }
            return true;
        }
    }
}
