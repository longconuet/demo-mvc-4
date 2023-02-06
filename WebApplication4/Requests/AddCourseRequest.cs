using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebApplication4.Requests
{
    public class AddCourseRequest
    {
        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 5)]
        public string Name { get; set; } = "";

        [Required]
        [Range(1, 200, ErrorMessage = "{0} length must be between {1} and {2}.")]
        public int MaxStudentNum { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be less than or equal {1}.")]
        public string Code { get; set; } = "";
    }
}
