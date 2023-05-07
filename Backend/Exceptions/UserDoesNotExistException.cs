using System;

namespace IntroSE.Kanban.Backend.Exceptions
{
    public class UserDoesNotExistException : SystemException
    {
        public UserDoesNotExistException() : base() { }
        public UserDoesNotExistException(string message) : base(message) { }
        public UserDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }
}
