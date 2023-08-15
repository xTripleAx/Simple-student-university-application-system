using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TheTask.Data
{
    public class Major
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string MajorName { get; set; }

        public int FacultyId { get; set; }

        [ForeignKey("FacultyId")]
        public Faculty Faculty { get; set; }
    }
}