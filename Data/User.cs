using System.ComponentModel.DataAnnotations;

namespace TheTask.Data
{
    public partial class User
    {
        [Key]
        public int user_id { get; set; }

        public string password { get; set; } = null!;

        public string? first_name { get; set; } = null!;

        public string? last_name { get; set; } = null!;

        public string? email { get; set; } = null!;

        public string? phone_number { get; set; }

        public DateOnly? date_of_birth { get; set; }

        public string? gender { get; set; }

        public string? role { get; set; }

        public DateTime? registration_date { get; set; }
    }
}
