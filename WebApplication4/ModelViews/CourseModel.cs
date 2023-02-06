namespace WebApplication4.ModelViews
{
    public class CourseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public int MaxStudentNum { get; set; }
        public int CurrentStudentNum { get; set; }
    }
}
