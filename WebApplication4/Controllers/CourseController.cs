﻿using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using WebApplication4.Models;
using WebApplication4.ModelViews;
using WebApplication4.Requests;
using WebApplication4.Services;

namespace WebApplication4.Controllers
{
    public class CourseController : Controller
    {
        private ICourseService _CourseService;

        public CourseController()
        {
            _CourseService = new CourseService();
        }


        public IActionResult Index()
        {
            return View();
        }

        public JsonResult List(string keyword)
        {
            using var db = new StudentDbContext();

            var courseDb = db.Courses.Where(x => x.IsDeleted == 0);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                courseDb = courseDb.Where(x => x.Name.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
            }

            var courseIds = courseDb.Select(x => x.Id).ToList();
            var courseStudents = db.CourseStudents.Where(x => courseIds.Contains(x.CourseId) && x.IsDeleted == 0)
                .Join(db.Students, c => c.StudentId, s => s.Id, (c, s) => new { c, s })
                .Where(x => x.s.IsDeleted == 0)
                .ToList();

            var courses = courseDb.ToList();
            var data = new List<CourseModel>();
            foreach (var course in courses)
            {
                data.Add(new CourseModel
                {
                    Id = course.Id,
                    Name = course.Name,
                    Code = course.Code,
                    MaxStudentNum = course.MaxStudentNum,
                    CurrentStudentNum = courseStudents.Count(x => x.c.CourseId == course.Id)
                });
            }

            return Json(data);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyCode(string code)
        {
            if (!_CourseService.VerifyCode(code))
            {
                return Json($"Code {code} is already in use.");
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult<ResponseModel> Create([FromBody] AddCourseRequest request)
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

            var result = _CourseService.Store(request);
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
                Message = "Create course successfully"
            };
        }

        public ActionResult<ResponseModel<CourseModel>> GetById(int id)
        {
            using var db = new StudentDbContext();

            var course = db.Courses.FirstOrDefault(x => x.Id == id && x.IsDeleted == 0);
            if (course == null)
            {
                return new ResponseModel<CourseModel>
                {
                    Status = 0,
                    Message = "Course does not exist"
                };
            }

            return new ResponseModel<CourseModel>
            {
                Status = 1,
                Data = new CourseModel
                {
                    Id = id,
                    Name = course.Name,
                    Code = course.Code,
                    MaxStudentNum = course.MaxStudentNum
                }
            };
        }

        [HttpPost]
        public ActionResult<ResponseModel> Update([FromBody] UpdateCourseRequest request)
        {
            // validate
            if (!ModelState.IsValid)
            {
                var message = string.Join(" </br> ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            var result = _CourseService.Update(request);
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
                Message = "Update course successfully"
            };
        }

        [HttpPost]
        public ActionResult<ResponseModel> Delete(int id)
        {
            var result = _CourseService.Delete(id);
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
                Message = "Delete course successfully"
            };
        }

        [HttpGet]
        public ActionResult<ResponseModel<List<StudentCourseModel>>> AllStudentsOfCourse(int id, string keyword)
        {
            using var db = new StudentDbContext();

            var course = db.Courses.FirstOrDefault(x => x.Id == id && x.IsDeleted == 0);
            if (course == null)
            {
                return new ResponseModel<List<StudentCourseModel>>
                {
                    Status = 0,
                    Message = "Course does not exist"
                };
            }

            var enrolledStudentIds = db.CourseStudents.Where(x => x.CourseId == id && x.IsDeleted == 0)
                .Select(x => x.StudentId)
                .ToList();

            var studentDb = db.Students.Where(x => x.IsDeleted == 0);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                studentDb = studentDb.Where(x => x.FullName.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
            }

            var data = new List<StudentCourseModel>();
            foreach (var student in studentDb)
            {
                data.Add(new StudentCourseModel
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Code = student.Code,
                    Age = student.Age,
                    Address = student.Address,
                    IsEnrolled = enrolledStudentIds.Any(x => x == student.Id) ? 1 : 0
                });
            }

            return new ResponseModel<List<StudentCourseModel>>
            {
                Status = 1,
                Data = data
            };
        }

        [HttpGet]
        public ActionResult<ResponseModel<List<StudentModel>>> StudentsOfCourse(int id, string keyword)
        {
            using var db = new StudentDbContext();

            var course = db.Courses.FirstOrDefault(x => x.Id == id && x.IsDeleted == 0);
            if (course == null)
            {
                return new ResponseModel<List<StudentModel>>
                {
                    Status = 0,
                    Message = "Course does not exist"
                };
            }

            var studentIds = db.CourseStudents.Where(x => x.CourseId == id && x.IsDeleted == 0)
                .Select(x => x.StudentId)
                .ToList();

            var studentDb = db.Students.Where(x => x.IsDeleted == 0 && studentIds.Contains(x.Id));

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                studentDb = studentDb.Where(x => x.FullName.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
            }

            var data = studentDb.Select(x => new StudentModel
            {
                Id = x.Id,
                FullName = x.FullName,
                Code = x.Code,
                Age = x.Age,
                Address = x.Address
            })
            .ToList();

            return new ResponseModel<List<StudentModel>>
            {
                Status = 1,
                Data = data
            };
        }

        [HttpGet]
        public ActionResult<ResponseModel<List<StudentModel>>> StudentsToEnroll(int id, string keyword)
        {
            using var db = new StudentDbContext();

            var course = db.Courses.FirstOrDefault(x => x.Id == id && x.IsDeleted == 0);
            if (course == null)
            {
                return new ResponseModel<List<StudentModel>>
                {
                    Status = 0,
                    Message = "Course does not exist"
                };
            }

            var studentIds = db.CourseStudents.Where(x => x.CourseId == id && x.IsDeleted == 0)
                .Select(x => x.StudentId)
                .ToList();

            if (studentIds.Count >= course.MaxStudentNum)
            {
                return new ResponseModel<List<StudentModel>>
                {
                    Status = 0,
                    Message = "Course is full"
                };
            }

            var studentDb = db.Students.Where(x => x.IsDeleted == 0 && !studentIds.Contains(x.Id));

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                studentDb = studentDb.Where(x => x.FullName.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
            }

            var data = studentDb.Select(x => new StudentModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Code = x.Code,
                    Age = x.Age,
                    Address = x.Address
                })
                .ToList();

            return new ResponseModel<List<StudentModel>>
            {
                Status = 1,
                Data = data
            };
        }

        [HttpPost]
        public ActionResult<ResponseModel> EnrollStudentToCourse([FromBody] EnrollStudentToCourseRequest request)
        {
            var result = _CourseService.EnrollStudent(request);
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
                Message = "Enroll student to course successfully"
            };
        }

        [HttpPost]
        public ActionResult<ResponseModel> RemoveStudentFromCourse([FromBody] RemoveStudentFromCourseRequest request)
        {
            var result = _CourseService.RemoveStudent(request);
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
                Message = "Remove student from course successfully"
            };
        }
    }
}
