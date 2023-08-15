using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using TheTask.Data;
using MySql.Data.MySqlClient;
using System.Data;
using MimeKit;
using MailKit.Net.Smtp;

namespace TheTask.Controllers
{
    public class AdmissionController : Controller
    {
        private readonly ILogger<AdmissionController> _logger2;
        private readonly MyDBContext db;
        private readonly IConfiguration _configuration;

        public AdmissionController(ILogger<AdmissionController> logger,MyDBContext db, IConfiguration configuration) //Constructor initializing logger and configuration for some function uses
        {
            _logger2 = logger;
            this.db = db;
            _configuration = configuration;
        }

        public IActionResult Index() //The application form page that takes info from the user
        {
            var listOfFac = db.faculty.ToList();
            ApplicationFacultyViewModel viewmodel = new ApplicationFacultyViewModel()
            {
                Application = new Application(),
                faculties = listOfFac,
            };
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ApplicationFacultyViewModel obj) //The post method for the form saving a new application to the database
        {
            string connectionString = _configuration.GetConnectionString("Default");
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync(); //async to call database and send info
                using (var command = new MySqlCommand("CheckApplicationEligibility", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;  //calling the stored procedure to check applying eligibility
                    command.Parameters.AddWithValue("phoneNumber", obj.Application.ApplicantPhoneNumber);
                    command.Parameters.AddWithValue("email", obj.Application.ApplicantEmail);
                    try
                    {
                        await command.ExecuteNonQueryAsync();  //execute the procedure
                    }
                    catch (MySqlException ex)
                    {
                        if (ex.ErrorCode == -2147467259)  //If application is ineligible error message is sent to user that he can't apply 
                        {
                            ModelState.AddModelError("", ex.Message);
                            var hello = new ApplicationFacultyViewModel();
                            hello.Application = new Application();
                            hello.faculties = db.faculty.ToList();
                            return View(hello);
                        }
                    }
                    if (obj != null && obj.Application.ApplicationData != null && obj.Application.ApplicationData.Length > 0) //checks for data availability
                    {
                        if (!Regex.IsMatch(obj.Application.ApplicantPhoneNumber, @"^\d{2}/\d{6}$")) //confirms phone number matches the format
                        {
                            ModelState.AddModelError("Application.ApplicantPhoneNumber", "Phone number must match the pattern (e.g., 12/345678).");
                            var hi = new ApplicationFacultyViewModel();
                            hi.Application = new Application();
                            hi.faculties = db.faculty.ToList();
                            return View(hi);
                        }
                        var fileName = Path.GetFileNameWithoutExtension(obj.Application.ApplicationData.FileName);  //extracting the file and including it in a path with unique naming
                        var fileextension = Path.GetExtension(obj.Application.ApplicationData.FileName);
                        fileName = fileName + DateTime.Now.ToString("yyMMddmmssfff") + fileextension;
                        var filePath = Path.Combine("uploads", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await obj.Application.ApplicationData.CopyToAsync(fileStream);
                        }

                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("USAL", "testthetask77@gmail.com"));
                        message.To.Add(new MailboxAddress("", obj.Application.ApplicantEmail));
                        message.Subject = "Application Recieved";

                        message.Body = new TextPart("plain")
                        {
                            Text = $"Your application was Recieved. \nWe will answer with feedback soon. \n \n Best Regards, \n USAL"
                        };
                        using (var client = new SmtpClient())
                        {
                            client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                            client.Authenticate("testthetask77@gmail.com", "kqprckhpgwfnimew");
                            client.Send(message);
                            client.Disconnect(true);
                        }


                        obj.Application.ApplicationDate = DateTime.Now; //saving appropiate data to the database
                        obj.Application.filePath = filePath;
                        db.Application.Add(obj.Application);
                        await db.SaveChangesAsync();
                        TempData["FormSubmitted"] = "Disapproved";
                        return RedirectToAction("Index");
                    }
                }
            }
            ModelState.AddModelError("Application.ApplicationData", "Please select a certificate file."); 
            var hello2 = new ApplicationFacultyViewModel();
            hello2.Application = new Application();
            hello2.faculties = db.faculty.ToList();
            return View(hello2);
        }
    }
}