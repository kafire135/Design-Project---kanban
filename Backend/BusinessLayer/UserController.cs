using System;
using System.Text.RegularExpressions;
using IntroSE.Kanban.Backend.Exceptions;
using IntroSE.Kanban.Backend.Utilities;
using IntroSE.Kanban.Backend.DataAccessLayer;


namespace IntroSE.Kanban.Backend.BusinessLayer
{

    /// <summary>
    ///This class controls the actions operated by the user.<br/>
    ///<br/>
    ///<code>Supported operations:</code>
    ///<br/>
    /// <list type="bullet">Register()</list>
    /// <list type="bullet">DeleteUser()</list>
    /// <list type="bullet">LogIn()</list>
    /// <list type="bullet">LogOut()</list>
    /// <list type="bullet">SetPassword()</list>
    /// <list type="bullet">SetEmail()</list>
    /// <list type="bullet">SearchUser()</list>
    /// <list type="bullet">IsLegalPassword()</list>
    /// <br/><br/>
    /// ===================
    /// <br/>
    /// <c>Ⓒ Hadas Printz</c>
    /// <br/>
    /// ===================
    /// </summary>
    public class UserController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Backend\\BusinessLayer\\UserController.cs");

        private UserDataOperations userData;
        private static readonly int MIN_PASS_LENGTH = 6;
        private static readonly int MAX_PASS_LENGTH = 20;
        private UserControllerDTO userDTO;

        /// <summary>
        /// Creates an empty <c>BinaryTree</c> userList <br/>
        /// Creates an empty <c>Dictionary</c> loggedIn
        /// </summary>
        public UserController(UserDataOperations userData)
        {
            this.userData = userData;
            userDTO = DataAccessLayerFactory.GetInstance().UserControllerDTO;
        }

        /// <summary>
        /// Add new <c>User</c> to <c>UserData</c> userData <br/>
        /// logIn <c>User</c> <br/>
        /// <b>Throws</b> <c>ElementAlreadyExistsException</c> if the user allready exists and <c>ArgumentException</c> if the password entered is illegal or the email entered is illegal and if the user is loggedIn
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <exception cref="ArgumentException"></exception>

        public void Register(CIString email, string password)
        {
            log.Debug("Register() for: " + email);
            if (!IsLegalPassword(password)) {
                log.Error("Register() failed: '" + password + "' is illegal");
                throw new ArgumentException("Password illegal"); }
            if (!IsEmailValid(email))
            {
                log.Error("Register() failed: '" + email + "' is illegal");
                throw new ArgumentException("email illegal"); }
            try
            {
                userData.AddUser(email,password);
                userData.SetLoggedIn(email);

                //DAL CALLS
                userDTO.AddUser(email, password);

                log.Debug("Register() success");
            }
            catch (ElementAlreadyExistsException)
            {
                log.Error("Register() failed: '" + email + "' already exist in the system");
                throw new ArgumentException("Email is already in use");
            }
            catch (ArgumentException)
            {
                log.Error("Register() failed: '" + email + "' already loggedIn");
                throw new ArgumentException("A user whith that " + email +
                    " already loggedIn");
            }

        }

        /// <summary>
        ///Delate <c>User</c> from the <c>UserData</c> userData <br/> <br/>
        ///<b>Throws</b> <c>UserDoesNotExistException</c> if the user doesn't exist in the userData
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="UserDoesNotExistException"></exception>
        public void DeleteUser(CIString email)
        {
            throw new NotImplementedException("DEPRECATED METHOD: Not updated to support current requirements");


#pragma warning disable CS0162 // Unreachable code detected
            log.Debug("DeleteUser() for: " + email);

            try
            {
                userData.RemoveUser(email);
                log.Debug("DeleteUser() success");
            }
            catch (UserDoesNotExistException)
            {
                log.Error("DeleteUser() failed: " + email + " doesn't exist in the system");
                throw new UserDoesNotExistException("User doesn't exist in the system");
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        /// <summary>
        /// Log in User to the system-Add user to <c>Dictionary</c> loogedIn <br/><br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the user is allready loggedIn or <br/> if user with those email and password doesn't exist or <br/> if email illegal
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <exception cref="ArgumentException"></exception>
        public void LogIn(CIString email, string password)
        {
            log.Debug("LogIn() for: " + email);
            if(!IsEmailValid(email))
            {
                log.Error("LogIn() failed: " + email + " illegal");
                throw new ArgumentException("Illegal email");
            }
            if (userData.UserExists(email))
            {
                if (userData.SearchUser(email).CheckPasswordMatch(password))
                {
                    try
                    {
                        userData.SetLoggedIn(email);
                        log.Debug("LogIn() success");
                    }
                    catch(ArgumentException)
                    {
                        log.Error("LogIn() failed: user with '" + email + "' already loggedIn");
                        throw new ArgumentException("User is already logged in");
                    }      
                }
                else
                {
                    log.Error("LogIn() failed: " + password + "incorrect");
                    throw new ArgumentException("Incorrect email or password");
                }
            }
            else
            {
                log.Error("LogIn() failed: there is no user with " + email + " in the system");
                throw new ArgumentException("Incorrect email or password");  
            }
        }

        /// <summary>
        /// Log out user from the system- Remove user from <c>Dictionary</c> loogedIn <br/><br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the user isn't logged in or <br/> if email is illegal
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="ArgumentException"></exception>
        public void LogOut(CIString email)
        {
            log.Debug("LogOut() for " + email);
            if(!IsEmailValid(email))
            {
                log.Error("LogOut() failed: " + email + " illegal");
                throw new ArgumentException("Illegal email");
            }
            if (userData.UserExists(email) == false)
            {
                log.Error("LogOut() failed: there is no user with " + email + " in the system");
                throw new ArgumentException("There is no such user in the system");
            }
            try
            {
                userData.SetLoggedOut(email);
                log.Debug("LogOut() success");
            }
            catch (ArgumentException)
            {
                log.Error("LogOut() failed: " + email + "is not logget in");
                throw new ArgumentException("User isn't loggedIn");
            }
            
        }

        /// <summary>
        /// Replace the user's password after ensuring the user entered his old password correctly <br/><br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the user doesn't exist in the system <b>or</b> <br/> if the new password entered is illegal <b>or</b> <br/> if the old password entered doesn't match the user's password <br/>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="old"></param>
        /// <param name="newP"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetPassword(CIString email, string old, string newP)
        {
            log.Debug("SetPassword() for : '" + email + "' from '" + old + "' to '" + newP);
            if (userData.UserExists(email) == false)
            {
                log.Error("SetPassword() failed: '" + email + "' is not in the system");
                throw new ArgumentException("User is not in the system");
            }
            if (!IsLegalPassword(newP))
            {
                log.Error("SetPassword() failed: '" + newP + "' is illegal");
                throw new ArgumentException("New password is illegal");
            }
            User user = SearchUser(email);
            if (user.CheckPasswordMatch(old))
            {
                user.Password = newP;

                //DAL CALLS
                userDTO.ChangePassword(user.Email.Value, newP);

                log.Debug("SetPassword() success");
            }
            else
            {
                log.Error("SetPassword() failed: '" + old + "' incorrect");
                throw new ArgumentException("Old Password incorrect");
            }
        }

        /// <summary>
        /// Change the user's email <br/><br/>
        /// <b>Throws</b> <c>ArgumentException</c> if the user doesn't exist in the system <b>or</b> <br/> if a user with the new email exist in the system
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newE"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetEmail(CIString email, CIString newE)
        {
            log.Debug("SetEmail() for '" + email + "' to '" + newE);
            if (userData.UserExists(email) == false)
            {
                log.Error("SetEmail() failed: '" + email + "' doesn't exist");
                throw new ArgumentException("User dosen't exist");
            }
            if (!IsEmailValid(newE))
            {
                log.Error("SetEmail() failed: " + newE + " is illegal");
                throw new ArgumentException("New email illegal");
            }
            if (userData.UserExists(newE) == true)
            {
                log.Error("SetEmail() failed: user with " + newE + " allready exist in the system");
                throw new ArgumentException("A user with that email already exists in the system");
            }
            SearchUser(email).Email = newE;

            //DAL CALLS
            userDTO.ChangeEmail(email, newE.Value);


            log.Debug("SetEmail() success");
        }

        /// <summary>
        ///Return user with the email entered <br/><br/>
        ///<b>Throws</b> <c>UserDoesNotExistException</c> if no user with that email exist in the system <br/>
        /// </summary> 
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="UserDoesNotExistException"></exception>
        public User SearchUser(CIString email)
        {
            log.Debug("SearchUser() for: '" + email);
            try
            {
                User user = userData.SearchUser(email);
                log.Debug("SearchUser() success");
                return user;
            }
            catch (UserDoesNotExistException)
            {
                log.Error("SearchUser() failed: user with '" + email + "' doesn't exist in the system");
                throw;
            }
        }

        /// <summary>
        /// Check the legality of a password <br/><br/>
        /// Returns: <br/>
        /// <b>True</b> if the password is legal and <b>False</b> otherwise
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        private static bool IsLegalPassword(string pass)
        {
            Regex smallLetters = new Regex(@"[a-z]");
            Regex capitalLetters = new Regex(@"[A-Z]");
            Regex numbers = new Regex(@"[0-9]");

            if (smallLetters.Matches(pass).Count > 0 &
                capitalLetters.Matches(pass).Count > 0 &
                numbers.Matches(pass).Count > 0 &
                pass.Length >= MIN_PASS_LENGTH &
                pass.Length <= MAX_PASS_LENGTH)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check the legality of a email <br/><br/>
        /// Returns: <br/>
        /// <b>True</b> if the email is legal and <b>False</b> otherwise
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static bool IsEmailValid(CIString email)
        {
            Regex valid1 = new Regex(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$");
            //Regex valid2 = new Regex(@"^\S+@\S+\.\S+$");
            if (valid1.Matches(email.Value).Count != 1 /*&& valid2.Matches(email.Value).Count == 1*/)
            {
                return false;
            }
            string domain = email.Value.Split('@')[1];

            if (domain.Contains('_')) return false;
            foreach (string str in domain.Split('.'))
            {
                if(str.StartsWith('-') | str.EndsWith('-') | str.Length > 63) return false;
            }
            string[] subdomains = domain.Split('.');
            if (new Regex(@"[a-zA-Z]").Matches(subdomains[subdomains.Length - 1]).Count == 0) return false;

            return true;
        }

    }
}

