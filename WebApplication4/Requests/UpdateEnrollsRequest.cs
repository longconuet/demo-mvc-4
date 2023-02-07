using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Requests
{
    public class UpdateEnrollsRequest
    {
        [Required]
        public int CourseId { get; set; }

        public List<int> EnrolledStudentIds { get; set; } = new List<int>();
    }
}
