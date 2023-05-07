using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class BoardDTO
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public string Owner { set; get; }
        public LinkedList<string> Joined { set; get; }
        public LinkedList<TaskDTO> BackLog { set; get; }
        public LinkedList<TaskDTO> InProgress { set; get; }
        public LinkedList<TaskDTO> Done { set; get; }
        public int BackLogLimit { set; get; }
        public int InProgressLimit { set; get; }
        public int DoneLimit { set; get; }
        public int TaskIDCounter { set; get; }
    }
}
