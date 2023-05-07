using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Frontend.ViewModel.UIElements
{
    public class TextBox : UIElement
    {
        public bool FirstClick;
        public TextBox(int x, int y, int width, int height, string text, bool visibility) : base(x, y, width, height, text, visibility)
        {
            FirstClick = true;
        }
        public TextBox(int x, int y, int width, int height, string text) : this(x, y, width, height, text, true) { }
        public TextBox(int x, int y, string text) : this(x, y, 0, 0, text, true) { }
        public TextBox(int x, int y, string text, bool visibility) : this(x, y, 0, 0, text, visibility) { }
        public TextBox(int x, int y, int width, int height) : this(x, y, width, height, "", true) { }
    }
}
