using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.Utilities;
using IntroSE.Kanban.Frontend.ViewModel;

namespace IntroSE.Kanban.Frontend.View
{
    /// <summary>
    /// Interaction logic for BoardPage.xaml
    /// </summary>
    public partial class BoardPage : Window
    {
        private BoardPageViewModel VM;
        public BoardPage()
        {
            InitializeComponent();
            VM = new BoardPageViewModel();
            VM.SetWindow(this);
            DataContext = VM;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            VM.Submit_Click();
        }

        private void ChosenBoard_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            VM.ChosenBoard_Click();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            VM.LogOut_Click();
        }

        /// <summary>
        /// initialize the window
        /// </summary>
        /// <param name="email"></param>
        public void Initialize(string email)
        {
            VM.Initialize(email);
        }
        
    }
}
