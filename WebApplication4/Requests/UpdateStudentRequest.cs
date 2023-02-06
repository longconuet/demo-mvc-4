using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Requests
{
    public class UpdateStudentRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 5)]
        public string FullName { get; set; } = "";

        [Required]
        [Range(1, 200, ErrorMessage = "{0} length must be between {1} and {2}.")]
        public int Age { get; set; }

        public string? Address { get; set; }
    }
}
