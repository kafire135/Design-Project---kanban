using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Frontend.ViewModel.UIElements
{
    public class Label : UIElement
    {
        public Label(int x, int y, string text, bool visibility) : base(x, y, text, visibility) { }
        public Label(int x, int y, string text) : base(x, y, text, true) { }
        public Label(int x, int y, bool visibility) : base(x, y, "", visibility) { }
    }
}
