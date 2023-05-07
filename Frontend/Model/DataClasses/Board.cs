using IntroSE.Kanban.Frontend.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Frontend.Model.DataClasses
{
    public class Board
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Owner { get; set; }
        public LinkedList<string>? Joined { get; set; }
        public LinkedList<Task>[]? Columns { get; set; }
    }
}
