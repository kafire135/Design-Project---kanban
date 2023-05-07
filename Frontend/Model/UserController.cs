using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Frontend.Utilities;

namespace IntroSE.Kanban.Frontend.Model
{
    public class UserController
    {

        UserService userService;

        public UserController()
        {
            ServiceLayerFactory.GetInstance().BackendInitializer.LoadData();
            userService = ServiceLayerFactory.GetInstance().UserService;
        }

        /// <summary>
        /// logs a user in
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Utilities.Response<string> Login(string email, string password)
        {
            string json = userService.LogIn(email, password);
            return Utilities.JsonEncoder.BuildFromJson<Utilities.Response<string>>(json);
            
        }

        /// <summary>
        /// registers a new user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Utilities.Response<string> Register(string email, string password)
        {
            string json = userService.Register(email, password);
            return Utilities.JsonEncoder.BuildFromJson<Utilities.Response<string>>(json);
        }

        /// <summary>
        /// logs a user out
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Utilities.Response<string> Logout(string email)
        {
            string json = userService.LogOut(email);
            return Utilities.JsonEncoder.BuildFromJson<Utilities.Response<string>>(json);
        }
    }
}
