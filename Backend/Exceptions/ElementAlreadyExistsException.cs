using System;

namespace IntroSE.Kanban.Backend.Exceptions
{
    public class ElementAlreadyExistsException : SystemException
    {
        public ElementAlreadyExistsException() : base() { }
        public ElementAlreadyExistsException(string message) : base(message) { }
        public ElementAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
