using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    [Table("CourseStudents")]
    public class CourseStudent : BaseModel
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
    }
}
