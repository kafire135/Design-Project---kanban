using System.Windows;
using IntroSE.Kanban.Frontend.ViewModel;

namespace IntroSE.Kanban.Frontend.View
{
    public partial class TaskPage : Window
    {
        private TaskViewModel VM;

        public TaskPage()
        {
            InitializeComponent();
            VM = new TaskViewModel();
            VM.SetWindow(this);
            DataContext = VM;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            VM.Return_Click();
        }

        /// <summary>
        /// initialize the window
        /// </summary>
        /// <param name="email"></param>
        public void Initialize(string email,int boardId)
        {
            VM.Initialize(email,boardId);
        }
    }
}
