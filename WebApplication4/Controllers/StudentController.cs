using Microsoft.AspNetCore.Mvc;
using WebApplication4.ModelViews;
using WebApplication4.Requests;
using WebApplication4.Services;
using System.Linq.Dynamic;
using System.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class StudentController : Controller
    {
        private IStudentService _studentService;

        public StudentController()
        {
            _studentService = new StudentService();
        }


        public IActionResult Index()
        {
            return View();
        }

        public JsonResult List(string keyword, int? page)
        {
            using var db = new StudentDbContext();

            var studentDb = db.Students.Where(x => x.IsDeleted == 0);

            
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                studentDb = studentDb.Where(x => x.FullName.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
            }

            // paging
            int pageSize = 2;
            int pageIndex = page ?? 1;
            int start = (pageIndex - 1) * pageSize;
            int totalCount = studentDb.Count();
            int totalPage = totalCount / pageSize;
            if (totalCount % pageSize > 0)
            {
                totalPage += 1;
            }

            var students = studentDb.Skip(start).Take(pageSize).ToList();
            int totalItem = students.Count;

            var studentIds = students.Select(x => x.Id).ToList();
            var courseStudents = db.CourseStudents.Where(x => studentIds.Contains(x.StudentId) && x.IsDeleted == 0)
                .Join(db.Courses, cs => cs.CourseId, c => c.Id, (cs, c) => new { cs, c })
                .Where(x => x.c.IsDeleted == 0)
                .ToList();

            var data = new List<StudentModel>();
            foreach (var student in students)
            {
                data.Add(new StudentModel
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Code = student.Code,
                    Age = student.Age,
                    Address = student.Address,
                    Courses = courseStudents.Where(x => x.cs.StudentId == student.Id).Select(x => x.c.Name).ToList()
                });
            }

            return Json(new PagedStudentModel
            {
                Students = data,
                TotalCount = totalCount,
                TotalItem = totalItem,
                TotalPage = totalPage,
                PageSize = pageSize,
                PageCurrent = pageIndex
            });
        }

        public ActionResult LoadData()
        {
            try
            {
                //Creating instance of DatabaseContext class  
                using (var _context = new StudentDbContext())
                {

                    Request.Form.TryGetValue("draw", out var draws);
                    var draw = draws.FirstOrDefault();

                    Request.Form.TryGetValue("start", out var starts);
                    var start = starts.FirstOrDefault();

                    Request.Form.TryGetValue("length", out var lengths);
                    var length = lengths.FirstOrDefault();

                    Request.Form.TryGetValue("order[0][column]", out var orderColumns);
                    var orderColumn = orderColumns.FirstOrDefault();

                    Request.Form.TryGetValue("columns[" + orderColumn + "][name]", out var sortColumns);
                    var sortColumn = sortColumns.FirstOrDefault();

                    Request.Form.TryGetValue("order[0][dir]", out var sortColumnDirs);
                    var sortColumnDir = sortColumnDirs.FirstOrDefault();

                    Request.Form.TryGetValue("search[value]", out var searchValues);
                    var searchValue = searchValues.FirstOrDefault();

                    //var start = Request.Form.GetValues("start").FirstOrDefault();
                    //var length = Request.Form.GetValues("length").FirstOrDefault();
                    //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                    //var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                    //Paging Size (10,20,50,100)    
                    int pageSize = length != null ? Convert.ToInt32(length) : 0;
                    int skip = start != null ? Convert.ToInt32(start) : 0;
                    int recordsTotal = 0;

                    // Getting all data    
                    var studentData = _context.Students.Where(x => x.IsDeleted == 0);

                    //Sorting    
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        if (sortColumnDir == "asc")
                        {
                            switch (sortColumn)
                            {
                                case "fullName":
                                    studentData = studentData.OrderBy(x => x.FullName);
                                    break;
                                case "age":
                                    studentData = studentData.OrderBy(x => x.Age);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (sortColumn)
                            {
                                case "fullName":
                                    studentData = studentData.OrderByDescending(x => x.FullName);
                                    break;
                                case "age":
                                    studentData = studentData.OrderByDescending(x => x.Age);
                                    break;
                                default:
                                    break;
                            }
                        }
                        //studentData = studentData.OrderBy(sortColumn, sortColumnDir);
                    }

                    //Search    
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        studentData = studentData.Where(x => x.FullName.ToLower().Contains(searchValue) || x.Code.ToLower().Contains(searchValue));
                    }

                    //total number of rows count     
                    recordsTotal = studentData.Count();
                    //Paging     
                    var students = studentData.Skip(skip).Take(pageSize).ToList();

                    var studentIds = students.Select(x => x.Id).ToList();
                    var courseStudents = _context.CourseStudents.Where(x => studentIds.Contains(x.StudentId) && x.IsDeleted == 0)
                        .Join(_context.Courses, cs => cs.CourseId, c => c.Id, (cs, c) => new { cs, c })
                        .Where(x => x.c.IsDeleted == 0)
                        .ToList();

                    var data = new List<StudentModel>();
                    foreach (var student in students)
                    {
                        data.Add(new StudentModel
                        {
                            Id = student.Id,
                            FullName = student.FullName,
                            Code = student.Code,
                            Age = student.Age,
                            Address = student.Address,
                            Courses = courseStudents.Where(x => x.cs.StudentId == student.Id).Select(x => x.c.Name).ToList()
                        });
                    }

                    //Returning Json Data    
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyCode(string code)
        {
            if (!_studentService.VerifyCode(code))
            {
                return Json($"Code {code} is already in use.");
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult<ResponseModel> Create([FromBody] AddStudentRequest request)
        {
            // validate
            if (!ModelState.IsValid)
            {
                var message = string.Join(" </br> ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return new ResponseModel
                {
                    Status = 0,
                    Message = message
                };
            }

            var result = _studentService.Store(request);
            if (result.Status == 0)
            {
                return new ResponseModel
                {
                    Status = 0,
                    Message = result.Message
                };
            }

            return new ResponseModel
            {
                Status = 1,
                Message = "Create student successfully"
            };
        }

        public ActionResult<ResponseModel<StudentModel>> GetById(int id)
        {
            using var db = new StudentDbContext();

            var student = db.Students.FirstOrDefault(x => x.Id == id && x.IsDeleted == 0);
            if (student == null)
            {
                return new ResponseModel<StudentModel>
                {
                    Status = 0,
                    Message = "Student does not exist"
                };
            }

            return new ResponseModel<StudentModel>
            {
                Status = 1,
                Data = new StudentModel
                {
                    Id = id,
                    FullName = student.FullName,
                    Code = student.Code,
                    Age = student.Age,
                    Address = student.Address
                }
            };
        }

        [HttpPost]
        public ActionResult<ResponseModel> Update([FromBody] UpdateStudentRequest request)
        {
            // validate
            if (!ModelState.IsValid)
            {
                var message = string.Join(" </br> ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            var result = _studentService.Update(request);
            if (result.Status == 0)
            {
                return new ResponseModel
                {
                    Status = 0,
                    Message = result.Message
                };
            }

            return new ResponseModel
            {
                Status = 1,
                Message = "Update student successfully"
            };
        }

        [HttpPost]
        public ActionResult<ResponseModel> Delete(int id)
        {
            var result = _studentService.Delete(id);
            if (result.Status == 0)
            {
                return new ResponseModel
                {
                    Status = 0,
                    Message = result.Message
                };
            }

            return new ResponseModel
            {
                Status = 1,
                Message = "Delete student successfully"
            };
        }
    }
}
