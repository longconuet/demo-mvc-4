using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    [Table("Students")]
    public class Student : BaseModel
    {
        public string FullName { get; set; } = "";
        public int Age { get; set; }
        public string Code { get; set; } = "";
        public string? Address { get; set; }
    }
}
