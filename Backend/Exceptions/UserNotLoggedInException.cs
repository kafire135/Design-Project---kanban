using System;

namespace IntroSE.Kanban.Backend.Exceptions
{
    public class UserNotLoggedInException : SystemException
    {
        public UserNotLoggedInException() : base() { }
        public UserNotLoggedInException(string message) : base(message) { }
        public UserNotLoggedInException(string message, Exception innerException) : base(message, innerException) { }
    }
}
