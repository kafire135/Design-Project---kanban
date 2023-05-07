using IntroSE.Kanban.Frontend.Model;
using IntroSE.Kanban.Frontend.Model.DataClasses;
using IntroSE.Kanban.Frontend.Utilities;
using IntroSE.Kanban.Frontend.View;
using IntroSE.Kanban.Frontend.ViewModel.UIElements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IntroSE.Kanban.Frontend.ViewModel
{
    public class TaskViewModel : Notifier
    {
        private Window window;
        private Button backButton;
        private string email;
        private Board board;
        private BoardController boardController;

        public TaskViewModel()
        {
            backButton = new(306, 205, "Back");
            boardController = new BoardController();
        }

        public Button BackButton => backButton;
        public Board Board => board;

        /// <summary>
        /// initialize all the neccessary data
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardId"></param>
        public void Initialize(string email, int boardId)
        {
            this.email = email;
            board = boardController.SearchBoard(email,boardId);
            RaisePropertyChanged("Board");
        }

        /// <summary>
        /// return button action
        /// </summary>
        public void Return_Click()
        {
            BoardPage boardPage = new BoardPage();
            boardPage.Initialize(email);
            boardPage.Show();
            window.Close();
        }

        /// <summary>
        /// set window field for later use
        /// </summary>
        /// <param name="window"></param>
        public void SetWindow(Window window)
        {
            this.window = window;
        }
    }
}
