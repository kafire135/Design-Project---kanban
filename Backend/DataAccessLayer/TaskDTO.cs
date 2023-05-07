using System;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class TaskDTO
    {
        public int BoardId { get; set; }
        public int Id { set; get; }
        public string Title { set; get; }
        public string Description { set; get; }
        public DateTime CreationTime { set; get; }
        public DateTime DueDate { set; get; }
        public BoardColumnNames State { set; get; }
        public string Assignee { set; get; }
    }
}
