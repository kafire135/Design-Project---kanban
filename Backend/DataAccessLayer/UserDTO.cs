using System.Collections.Generic;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public LinkedList<int> MyBoards { get; set; }
        public LinkedList<int> JoinedBoards { get; set; }
    }
}
