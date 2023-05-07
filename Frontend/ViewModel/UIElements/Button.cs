using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Frontend.ViewModel.UIElements
{
    public class Button : UIElement
    {
        public Button(int x, int y, int width, int height, string content, bool visibility) : base(x, y, width, height, content, visibility) { }
        public Button(int x, int y, string content) : base(x, y, content) { }
        public Button(int x, int y, string content, bool Visible) : base(x, y, content, Visible) { }
    }
}
