using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheTask.Data;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Claims;

namespace TheTask.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {

        private readonly ILogger<EmployeeController> _logger2;
        private readonly MyDBContext db;

        public EmployeeController(ILogger<EmployeeController> logger, MyDBContext db)
        {
            _logger2 = logger;
            this.db = db;
        }

        public IActionResult Index()  //The Dashboard page of the employee views
        {
            return View();
        }

        public IActionResult ApplicationList()  //Shows the applications of the database from all applicants
        {
            IEnumerable<Application> apps = db.Application;
            return View(apps);
        }


        public IActionResult ViewDetails(int id) //Views the details of specific applications chosen after pressing the button
        {
            var app = db.Application.SingleOrDefault(a => a.Id == id);
            var faculty = db.faculty;
            ApplicationFacultyViewModel model = new ApplicationFacultyViewModel();
            model.Application = app;
            model.faculties = faculty.ToList();
            return View(model);
        }



        public IActionResult ShowCertificate(int applicationId) //Views the certificate attached to the application as a pdf file available to download
        {
            var application = db.Application.Find(applicationId);
            if (application != null && !string.IsNullOrEmpty(application.filePath))
            {
                var fileStream = new FileStream(application.filePath, FileMode.Open, FileAccess.Read);
                return File(fileStream, "application/pdf");
            }
            return NotFound();
        }


        public IActionResult SendEmail(int id) //view the email sending page if the employee pressed the approve button
        {
            var application = db.Application.SingleOrDefault(b => b.Id == id);
            if (application != null)
            {
                return View(application);
            }
            else
            {
                return RedirectToAction("ApplicationList");
            }
        }

        [HttpPost]
        public IActionResult SendEmailPOST(int appId, string approvalDateTime, string notes)  //Sends the email after the employee confirmed the date of visit
        {
            var application = db.Application.SingleOrDefault(c => c.Id == appId);
            if (application != null)
            {
                if (DateTime.TryParse(approvalDateTime, out DateTime selectedDateTime))
                {

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("USAL", "testthetask77@gmail.com"));
                    message.To.Add(new MailboxAddress("", application.ApplicantEmail));
                    message.Subject = "Application Approved";

                    message.Body = new TextPart("plain")
                    {
                        Text = $"Your application was Approved by our registration committee. \nPlease visit or Registar office on {selectedDateTime.ToString("dd-MM-yyyy")} at {selectedDateTime.ToString("hh:mm tt")}. \n \n Best Regards, \n USAL"
                    };
                    using (var client = new SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                        client.Authenticate("testthetask77@gmail.com", "kqprckhpgwfnimew");
                        client.Send(message);
                        client.Disconnect(true);
                    }

                    string empidClaim = User.FindFirstValue(ClaimTypes.Name); //Updating the database with the required info of the application
                    int empid = int.Parse(empidClaim);
                    application.EmployeeId = empid;
                    application.ApplicationStatus = "Approved";
                    application.AdmissionDate = DateTime.Now;
                    TempData["FormSubmitted"] = "Approved";
                    if (notes != null)
                    {
                        application.AdmissionNotes = notes;
                    }
                    db.SaveChanges();
                    return RedirectToAction("ApplicationList");
                }
                else
                {
                    return RedirectToAction("ViewDetails", appId);
                }
            }
            return RedirectToAction("ViewDetails", appId);
        }



        public IActionResult DisapproveEmail(int id) //Shows the send email page if the employee chose to disapprove the application
        {
            var application = db.Application.SingleOrDefault(b => b.Id == id);
            if (application != null)
            {
                return View(application);
            }
            else
            {
                return RedirectToAction("ApplicationList");
            }
        }



        [HttpPost]
        public IActionResult DisapproveEmailPOST(int appId, string notes) //Sends the email of disapprovement if the employee confirms
        {
            var application = db.Application.SingleOrDefault(c => c.Id == appId);
            if (application != null)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("USAL", "testthetask77@gmail.com"));
                message.To.Add(new MailboxAddress("", application.ApplicantEmail));
                message.Subject = "Application Disapproved";

                message.Body = new TextPart("plain")
                {
                    Text = $"Your application was disapproved by our registration committee. \nPlease feel free to contact us for further Information. \n \n Best Regards, \n USAL"
                };
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate("testthetask77@gmail.com", "kqprckhpgwfnimew");
                    client.Send(message);
                    client.Disconnect(true);
                }

                string empidClaim = User.FindFirstValue(ClaimTypes.Name);
                int empid = int.Parse(empidClaim);
                application.EmployeeId = empid;
                application.ApplicationStatus = "Disapproved";
                application.AdmissionDate = DateTime.Now;
                if (notes != null)
                {
                    application.AdmissionNotes = notes;
                }
                db.SaveChanges();
                TempData["FormSubmitted"] = "Disapproved";
                return RedirectToAction("ApplicationList");

            }
            else
            {
                return RedirectToAction("ViewDetails", appId);
            }
        }
    }

}