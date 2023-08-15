using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Claims;
using TheTask.Data;

namespace TheTask.Controllers
{
    public class MoodleController : Controller
    {
        //Controller variables logger, the database, and configuration to access settings and values
        private readonly ILogger<MoodleController> _logger3;
        private readonly MyDBContext db;
        private readonly IConfiguration _configuration;

        public MoodleController(ILogger<MoodleController> logger, MyDBContext db,IConfiguration configuration)
        {
            _logger3 = logger;
            this.db = db;
            _configuration = configuration;
        }


        public IActionResult Login() //The login form to the system using a user account
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(int user_id, string password)  //The function that handle the login storing user info in a session and redirecting through role
        {
            string connectionString = _configuration.GetConnectionString("Default");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();  //Using asynchronous to connect with database and send info at the same time

                using (MySqlCommand command = new MySqlCommand("CheckLogin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;  //Using a procedure

                    command.Parameters.AddWithValue("@p_user_id", user_id);  //filling in the parameters of the procedure
                    command.Parameters.AddWithValue("@p_password", password);

                    command.Parameters.Add(new MySqlParameter("@p_VALID", MySqlDbType.Int32)); //getting the out values of the procedure
                    command.Parameters["@p_VALID"].Direction = ParameterDirection.Output;

                    command.Parameters.Add(new MySqlParameter("@p_role", MySqlDbType.Text));
                    command.Parameters["@p_role"].Direction = ParameterDirection.Output;

                    await command.ExecuteNonQueryAsync(); //Executing the procedure

                    int result = Convert.ToInt32(command.Parameters["@p_VALID"].Value);  //saving the values of the out into variables
                    string role = Convert.ToString(command.Parameters["@p_role"].Value);

                    if (result == -1)  //if result = -1 this means id is not found
                    {
                        ModelState.AddModelError("Hello", "Userid or Password incorrect!");
                        return View();
                    }
                    else //else retreiving the data of the user and authunticating his login
                    {
                        var claims = new List<Claim>  //Claims that hold the users info in order to use in Authentication
                        {
                            new Claim(ClaimTypes.Name, user_id.ToString()),
                            new Claim(ClaimTypes.Role, role) // Add the role claim
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "LoginAuthentication");
                        await HttpContext.SignInAsync("LoginAuthentication", new ClaimsPrincipal(claimsIdentity)); //sign in the user using his info
                        return RedirectToAction("Index", $"{role}"); // Redirect to role-specific index
                    }
                }
            }

        }


        public async Task<IActionResult> Logout()  //Logout function to remove the data of the logged in user
        {
            await HttpContext.SignOutAsync("LoginAuthentication"); // Sign out the user
            return RedirectToAction("Login", "Moodle");
        }
    }
}