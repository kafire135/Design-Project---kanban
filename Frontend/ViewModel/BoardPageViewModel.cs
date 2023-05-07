using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using IntroSE.Kanban.Frontend.ViewModel.UIElements;
using IntroSE.Kanban.Frontend.Utilities;
using IntroSE.Kanban.Frontend.Model.DataClasses;
using System.Windows;
using IntroSE.Kanban.Frontend.View;

namespace IntroSE.Kanban.Frontend.ViewModel
{
    public class BoardPageViewModel : Notifier
    {
        private Window window;

        private static readonly int Height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
        private static readonly int Width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;

        private int CHOSENBOARD_X = Width/2 -700;
        private int CHOSENBOARD_Y = Height/2 + 20;


        private int SUBMIT_X = Width / 2 - 300;
        private int SUBMIT_Y = Height / 2 + 20;

        private int LABEL_X = Width / 2 -600;
        private int LABEL_Y = Height / 2- 100;

        private int CHOOSEYOURBOARD_X = Width / 2 - 300;
        private int CHOOSEYOURBOARD_Y = Height / 2 + 150;

        private int LOGOUT_X = Width / 2 - 500;
        private int LOGOUT_Y = Height / 2 + 150;

        private TextBox chosenBoard;
        private Button submit;
        private Button logout;
        private Label errorMessage;
        private Label chooserYourBoard;

        /// <summary>
        /// controls the boardname field
        /// </summary>
        public string BoardName { get; set; }

        /// <summary>
        /// controls the boardid field
        /// </summary>
        public int BoardID { get; set; }

        public ObservableCollection<Board> BoardList { get; set; }

        /// <summary>
        /// returns chosenboard
        /// </summary>
        public TextBox ChosenBoard => chosenBoard;

        /// <summary>
        /// returns submit
        /// </summary>
        public Button Submit => submit;

        /// <summary>
        /// returns errormessage
        /// </summary>
        public Label ErrorMessage => errorMessage;

        /// <summary>
        /// returns chooseyourboard
        /// </summary>
        public Label ChooseYourBoard => chooserYourBoard;

        /// <summary>
        /// returns logout
        /// </summary>
        public Button Logout => logout;

        private Model.BoardController boardController;
        private Model.UserController uc;

        private string email;

        public BoardPageViewModel()
        {
            boardController = new Model.BoardController();
            uc = new();
            chosenBoard = new(CHOSENBOARD_X, CHOSENBOARD_Y, 0, 0, "Insert your chosen boardId", true);
            submit = new(SUBMIT_X, SUBMIT_Y, "Submit");
            errorMessage = new(LABEL_X, LABEL_Y, false);
            chooserYourBoard = new(CHOOSEYOURBOARD_X, CHOOSEYOURBOARD_Y, true);
            logout = new(LOGOUT_X, LOGOUT_Y, "LogOut");
        }
        
        /// <summary>
        /// initialize the neccessary data
        /// </summary>
        /// <param name="email"></param>
        public void Initialize(string email)
        {
            this.email = email;
            BoardList = boardController.GetBoards(email);
            if(BoardList == null)
            {
                ErrorMessage.Content = "The user has no boards";
                errorMessage.Show();
            }
            RaisePropertyChanged("BoardList");
        }

        /// <summary>
        /// set the window field for later use
        /// </summary>
        /// <param name="window"></param>
        public void SetWindow(Window window)
        {
            this.window = window;
        }

        /// <summary>
        /// clears the chosenboard content on the first click
        /// </summary>
        public void ChosenBoard_Click()
        {
            if(chosenBoard.FirstClick)
            {
                chosenBoard.Content = "";
                chosenBoard.FirstClick = false;
            }
        }

        /// <summary>
        /// continues to task window
        /// </summary>
        public void Submit_Click()
        {
            if (chosenBoard.FirstClick == false && chosenBoard.Content != null)
            {
                string text = chosenBoard.Content;
                if (int.TryParse(text, out _))
                {
                    int number = int.Parse(text);
                    if (number >= 0 && number <= BoardList.Count-1)
                    {
                        TaskPage TP = new();
                        TP.Initialize(email, number);
                        TP.Show();
                        window.Close();
                    }
                }
            }
            ErrorMessage.Content = "You must enter an exsisting board Id";
            errorMessage.Show();
        }

        /// <summary>
        /// logs out
        /// </summary>
        public void LogOut_Click()
        {
            uc.Logout(email);
            LandingPage landingPage = new LandingPage();
            landingPage.Show();
            window.Close();
        }
    }
}
