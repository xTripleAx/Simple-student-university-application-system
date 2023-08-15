using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheTask.Data
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [DisplayName("First Name")]
        public string ApplicantFirstName { get; set; }

        [MaxLength(50)]
        [DisplayName("Last Name")]
        public string ApplicantLastName { get; set; }

        [DisplayName("Email")]
        public string ApplicantEmail { get; set; }

        [DisplayName("Phone Number")]
        public string ApplicantPhoneNumber { get; set; }

        [DisplayName("Address")]
        public string ApplicantAddress { get; set; }

        [DisplayName("Degree")]
        public string ApplicantStatus { get; set; }

        [DisplayName("Status")]
        public string? ApplicationStatus { get; set; }

        public DateTime ApplicationDate { get; set; }

        [DisplayName("Certificate")]
        [NotMapped]
        public IFormFile ApplicationData { get; set; }

        public string? filePath { get; set; }

        public string? AdmissionNotes { get; set; }

        public DateTime? AdmissionDate { get; set; }

        [DisplayName("Gender")]
        public string ApplicantGender { get; set; }

        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        public int FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculty? Faculty { get; set; }

    }

}