using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using IntroSE.Kanban.Frontend.Model;
using IntroSE.Kanban.Frontend.Utilities;
using IntroSE.Kanban.Frontend.View;
using IntroSE.Kanban.Frontend.ViewModel.UIElements;

namespace IntroSE.Kanban.Frontend.ViewModel
{

    public partial class LandingPageViewModel : Notifier
    {
        Window window;

        private /*static*/ int Height/* = (int)System.Windows.SystemParameters.PrimaryScreenHeight*/;
        private /*static*/ int Width/* = (int)System.Windows.SystemParameters.PrimaryScreenWidth*/;

        private int Login_Button_X/* = Width / 2 - 95*/;
        private int Login_Button_Y/* = Height/2*/;
        private int Register_Button_X/* = Width / 2 - 95*/;
        private int Register_Button_Y/* = Height / 2 + Height / 10*/;
        private int Return_Button_X/* = Width / 2 - 95 - 100*/;
        private int Return_Button_Y/* = Height / 2 + Height / 10*/;

        private int Login_Button_Login_Screen_X/* = Width / 2 - 95 + 100*/;
        private int Login_Button_Login_Screen_Y/* = Height / 2 + Height / 10*/;
        private int Register_Button_Register_Screen_X/* = Width / 2 - 95 + 100*/;
        private int Register_Button_Register_Screen_Y/* = Height / 2 + Height / 10*/;

        private int EmailBox_X/* = Width / 2 -125*/;
        private int EmailBox_Y/* = Height / 2 - Height / 15*/;
        private int PasswordBox_X/* = Width/2 -125 */;
        private int PasswordBox_Y/* = Height/2 - Height / 30*/;
        private int ErrorMessage_X/* = Width / 2 - 125*/;
        private int ErrorMessage_Y/* = Height / 2*/;


        private Button loginButton;
        private Button registerButton;
        private Button returnButton;
        private TextBox emailBox;
        private TextBox passwordBox;
        private bool LoginOrRegisterScreen = true;
        private Label errorMessage;

        public Button LoginButton => loginButton;
        public Button RegisterButton => registerButton;
        public Button ReturnButton => returnButton;
        public TextBox EmailBox => emailBox;
        public TextBox PasswordBox => passwordBox;
        public Label ErrorMessage => errorMessage;
        public string ImageMargin => $"-{Width * 0.15},-{Height * 0.15},-{Width * 0.15},-{Height * 0.15}";

        private UserController LRController;

        public LandingPageViewModel()
        {
            
            LRController = new();
            loginButton = new(Login_Button_X,Login_Button_Y, "Login");
            registerButton = new(Register_Button_X,Register_Button_Y, "Register");
            returnButton = new(Return_Button_X,Return_Button_Y,"Return", false);
            emailBox = new(EmailBox_X, EmailBox_Y, "Insert email here", false);
            passwordBox = new(PasswordBox_X, PasswordBox_Y, "Insert password here", false);
            errorMessage = new(ErrorMessage_X, ErrorMessage_Y, false);
            UpdateMargins(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        }

        /// <summary>
        /// resets the errormessage
        /// </summary>
        public void ResetErrorMessage()
        {
            errorMessage.Hide();
            errorMessage.Content = "";
        }

        /// <summary>
        /// login click action
        /// </summary>
        public void LoginClick()
        {
            if (LoginOrRegisterScreen)
            {
                registerButton.Hide();
                loginButton.Y = Login_Button_Login_Screen_Y;
                loginButton.X = Login_Button_Login_Screen_X;
                returnButton.Show();
                emailBox.Show();
                passwordBox.Show();
                LoginOrRegisterScreen = false;
            }
            else
            {

                Response<string> res = LRController.Login(emailBox.Content,passwordBox.Content);
                if (res.operationState == false)
                {
                    ErrorMessage.Content = res.returnValue;
                    errorMessage.Show();
                }
                else
                {
                    BoardPage BP = new();
                    BP.Initialize(EmailBox.Content);
                    BP.Show();
                    window.Close();
                }
                
            }
        }

        /// <summary>
        /// register click action
        /// </summary>
        public void RegisterClick()
        {
            if (LoginOrRegisterScreen)
            {
                loginButton.Hide();
                registerButton.X = Register_Button_Register_Screen_X;
                returnButton.Show();
                emailBox.Show();
                passwordBox.Show();
                LoginOrRegisterScreen = false;
            }
            else
            {
                Response<string> res = LRController.Register(emailBox.Content, passwordBox.Content);
                if (res.operationState == false)
                {
                    ErrorMessage.Content = res.returnValue;
                    errorMessage.Show();
                }
                else
                {
                    BoardPage BP = new();
                    BP.Initialize(EmailBox.Content);
                    BP.Show();
                    window.Close();
                }
            }
        }

        /// <summary>
        /// removes text from the textbox on first click
        /// </summary>
        /// <param name="name"></param>
        public void TextBoxClick(string name)
        {
            switch (name)
            {
                case "EmailBox":
                    if (emailBox.FirstClick)
                    {
                        emailBox.Content = "";
                        emailBox.FirstClick = false;
                    }
                    break;
                case "PasswordBox":
                    if (passwordBox.FirstClick)
                    {
                        passwordBox.Content = "";
                        passwordBox.FirstClick = false;
                    }
                    break;
            }
        }

        /// <summary>
        /// returns to front page
        /// </summary>
        public void ReturnToFrontPage()
        {
            ResetErrorMessage();
            loginButton.X = Login_Button_X;
            loginButton.Y = Login_Button_Y;
            registerButton.X = Register_Button_X;

            returnButton.Hide();
            registerButton.Show();
            loginButton.Show();

            emailBox = new(EmailBox_X,EmailBox_Y,"Insert email here", false);
            RaisePropertyChanged("EmailBox");

            passwordBox = new(PasswordBox_X, PasswordBox_Y,"Insert password here", false);
            RaisePropertyChanged("PasswordBox");

            LoginOrRegisterScreen = true;
        }

        /// <summary>
        /// updates the UI elements' locations on the screen
        /// </summary>
        /// <param name="ActualWidth"></param>
        /// <param name="ActualHeight"></param>
        public void UpdateMargins(double ActualWidth, double ActualHeight)
        {
            Height = (int)ActualHeight;
            Width = (int)ActualWidth;
            UpdateSizes();

            if (LoginOrRegisterScreen)
            {
                loginButton.X = Login_Button_X;
                loginButton.Y = Login_Button_Y;
                registerButton.X = Register_Button_X;
                registerButton.Y = Register_Button_Y;
            }
            else
            {
                loginButton.X = Login_Button_Login_Screen_X;
                loginButton.Y = Login_Button_Login_Screen_Y;
                registerButton.X = Register_Button_Register_Screen_X;
                registerButton.Y = Register_Button_Register_Screen_Y;
            }
            returnButton.X = Return_Button_X;
            returnButton.Y = Return_Button_Y;
            emailBox.X = EmailBox_X;
            emailBox.Y = EmailBox_Y;
            passwordBox.X = PasswordBox_X;
            passwordBox.Y = PasswordBox_Y;
            errorMessage.X = ErrorMessage_X;
            errorMessage.Y = ErrorMessage_Y;
        }
        private void UpdateSizes()
        {
            Login_Button_X = Width / 2 - 80;
            Login_Button_Y = Height / 2;
            Register_Button_X = Width / 2 - 80;
            Register_Button_Y = Height / 2 + Height / 10;
            Return_Button_X = Width / 2 - 95 - 100;
            Return_Button_Y = Height / 2 + Height / 10;

            Login_Button_Login_Screen_X = Width / 2 - 95 + 100;
            Login_Button_Login_Screen_Y = Height / 2 + Height / 10;
            Register_Button_Register_Screen_X = Width / 2 - 95 + 100;
            Register_Button_Register_Screen_Y = Height / 2 + Height / 10;

            EmailBox_X = Width / 2 - 125;
            EmailBox_Y = Height / 2 - Height / 10;
            PasswordBox_X = Width / 2 - 125;
            PasswordBox_Y = Height / 2 - Height / 20;

            ErrorMessage_X = Width / 2 - 125;
            ErrorMessage_Y = Height / 2;

        }

        /// <summary>
        /// set the window field for later use
        /// </summary>
        /// <param name="window"></param>
        public void SetWindow(Window window)
        {
            this.window = window;
        }
    }
   
}
