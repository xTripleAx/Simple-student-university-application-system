using System.ComponentModel.DataAnnotations;

namespace TheTask.Data
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string FacultyName { get; set; }
    }
}