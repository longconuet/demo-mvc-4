namespace WebApplication4.ModelViews
{
    public class PagedStudentModel
    {
        public List<StudentModel> Students { get; set; } = new List<StudentModel>();
        public int TotalCount { get; set;}
        public int TotalItem { get; set;}
        public int TotalPage { get; set;}
        public int PageSize { get; set;}
        public int PageCurrent { get; set;}

    }
}
