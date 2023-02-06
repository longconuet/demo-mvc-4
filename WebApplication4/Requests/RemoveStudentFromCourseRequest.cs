using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Requests
{
    public class RemoveStudentFromCourseRequest
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int StudentId { get; set; }
    }
}
