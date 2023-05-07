using System;
using System.Text.Json.Serialization;
using IntroSE.Kanban.Backend.Utilities;

namespace IntroSE.Kanban.Backend.BusinessLayer
{

	/// <summary>
	///This class implements the object <c>User</c>
	///<br/>
	///<code>Supported operations:</code>
	///<br/>
	/// <list type="bullet">SetPassword()</list>
	/// <list type="bullet">SetEmail()</list>
	/// <list type="bullet">GetEmail()</list>
	/// <list type="bullet">CheckPasswordMatch()</list>
	/// <br/><br/>
	/// ===================
	/// <br/>
	/// <c>Ⓒ Hadas Printz</c>
	/// <br/>
	/// ===================
	/// </summary>
	public class User
    {
		private CIString email;
		private string password;

		/// <summary>
		/// Initialize email and password fields
		/// </summary>
		/// <param name="email"></param>
		/// <param name="password"></param>
		[JsonConstructor]
		public User(CIString email, string password)
		{
			this.email = email;
			this.password = password;
		}

		public User(DataAccessLayer.UserDTO userDTO)
		{
			email = userDTO.Email;
			password = userDTO.Password;
		}

		public CIString Email { set { email = value; } get { return email; } }
		public string Password { set { password = value; } }

		/// <summary>
		/// Check if the user's password match the password entered <br/><br/>
		/// <b>Throws</b> <n>ArgumentNullException</n> if the password entered is null <br/><br/>
		/// Returns: <b>True</b> if the password match and <b>False</b> otherWise
		/// </summary>
		/// <param name="pass"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool CheckPasswordMatch(string pass)
		{
			if (pass == null)  throw new ArgumentNullException("password is null"); 
			if (password.Equals(pass)) {
				return true;
			}
			return false;
		}

		public static implicit operator User(DataAccessLayer.UserDTO other)
		{
			return new User(other);
		}

			//====================================================
			//                  Json related
			//====================================================
		
	}
}

