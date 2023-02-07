using System.Web.Mvc;
using WebApplication4.ModalViews;
using WebApplication4.Models;
using WebApplication4.ModelViews;
using WebApplication4.Requests;

namespace WebApplication4.Services
{
    public interface ICourseService
    {
        bool VerifyCode(string code);
        ServiceResponse Store(AddCourseRequest request);
        ServiceResponse Update(UpdateCourseRequest request);
        ServiceResponse Delete(int id);
        ServiceResponse EnrollStudent(EnrollStudentToCourseRequest request);
        ServiceResponse RemoveStudent(RemoveStudentFromCourseRequest request);
        ServiceResponse UpdateEnrolls(UpdateEnrollsRequest request);
    }

    public class CourseService : ICourseService
    {
        public bool VerifyCode(string code)
        {
            using var db = new StudentDbContext();

            return db.Courses.Any(x => x.Code == code && x.IsDeleted == 0);
        }

        public ServiceResponse Store(AddCourseRequest request)
        {
            try
            {
                using var db = new StudentDbContext();

                var checkCode = db.Courses.Any(x => x.Code == request.Code && x.IsDeleted == 0);
                if (checkCode)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Code is used"
                    };
                }

                db.Courses.Add(new Course
                {
                    Code = request.Code,
                    Name = request.Name,
                    MaxStudentNum = request.MaxStudentNum
                });
                db.SaveChanges();

                return new ServiceResponse
                {
                    Status = 1
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse
                {
                    Status = 0,
                    Message = "Create course failed"
                };
            }
        }

        public ServiceResponse Update(UpdateCourseRequest request)
        {
            try
            {
                using var db = new StudentDbContext();

                var course = db.Courses.FirstOrDefault(x => x.Id == request.Id && x.IsDeleted == 0);
                if (course == null)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Course does not exist"
                    };
                }

                course.Name = request.Name;
                course.MaxStudentNum = request.MaxStudentNum;
                course.UpdatedAt = DateTime.Now;

                db.Courses.Update(course);
                db.SaveChanges();

                return new ServiceResponse
                {
                    Status = 1
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse
                {
                    Status = 0,
                    Message = "Update course failed"
                };
            }
        }

        public ServiceResponse Delete(int id)
        {
            try
            {
                using var db = new StudentDbContext();

                var course = db.Courses.FirstOrDefault(x => x.Id == id && x.IsDeleted == 0);
                if (course == null)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Course does not exist"
                    };
                }
                    
                course.IsDeleted = 1;
                course.UpdatedAt = DateTime.Now;

                db.Courses.Update(course);
                db.SaveChanges();

                return new ServiceResponse
                {
                    Status = 1
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse
                {
                    Status = 0,
                    Message = "Delete course failed"
                };
            }
        }

        public ServiceResponse EnrollStudent(EnrollStudentToCourseRequest request)
        {
            try
            {
                using var db = new StudentDbContext();

                if (!request.StudentIds.Any())
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Please select student"
                    };
                }

                var course = db.Courses.FirstOrDefault(x => x.Id == request.CourseId && x.IsDeleted == 0);
                if (course == null)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Course does not exist"
                    };
                }

                var enrolledStudentIds = db.CourseStudents.Where(x => x.CourseId == request.CourseId && x.IsDeleted == 0)
                    .Join(db.Students, c => c.StudentId, s => s.Id, (c, s) => new { c, s })
                    .Where(x => x.s.IsDeleted == 0)
                    .Select(x => x.c.StudentId)
                    .ToList();
                if (enrolledStudentIds.Count + request.StudentIds.Count > course.MaxStudentNum)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Course is full"
                    };
                }

                var studentIds = request.StudentIds.Except(enrolledStudentIds).ToList();

                var insertCourseStudents = studentIds.Select(x => new CourseStudent
                {
                    CourseId = request.CourseId,
                    StudentId = x
                }).ToList();

                db.CourseStudents.AddRange(insertCourseStudents);
                db.SaveChanges();

                return new ServiceResponse
                {
                    Status = 1
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse
                {
                    Status = 0,
                    Message = "Enroll student to course failed"
                };
            }
        }

        public ServiceResponse RemoveStudent(RemoveStudentFromCourseRequest request)
        {
            try
            {
                using var db = new StudentDbContext();

                var course = db.Courses.FirstOrDefault(x => x.Id == request.CourseId && x.IsDeleted == 0);
                if (course == null)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Course does not exist"
                    };
                }

                var courseStudent = db.CourseStudents.FirstOrDefault(x => x.CourseId == request.CourseId && x.StudentId == request.StudentId && x.IsDeleted == 0);
                if (courseStudent == null)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Student is not in course"
                    };
                }

                courseStudent.IsDeleted = 1;
                courseStudent.UpdatedAt = DateTime.Now;

                db.CourseStudents.Update(courseStudent);
                db.SaveChanges();

                return new ServiceResponse
                {
                    Status = 1
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse
                {
                    Status = 0,
                    Message = "Remove student from course failed"
                };
            }
        }

        public ServiceResponse UpdateEnrolls(UpdateEnrollsRequest request)
        {
            try
            {
                using var db = new StudentDbContext();
                List<int> insertStudentIds = new();
                List<int> deleteStudentIds = new();

                var course = db.Courses.FirstOrDefault(x => x.Id == request.CourseId && x.IsDeleted == 0);
                if (course == null)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Course does not exist"
                    };
                }

                var enrolledStudentIds = db.Students.Where(x => x.IsDeleted == 0 && request.EnrolledStudentIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToList();

                var oldEnrolledStudents = db.CourseStudents.Where(x => x.CourseId == request.CourseId && x.IsDeleted == 0).ToList();
                var oldEnrolledStudentIds = oldEnrolledStudents.Select(x => x.StudentId).ToList();
                if (enrolledStudentIds.Count > course.MaxStudentNum)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Course is full"
                    };
                }

                // insert
                insertStudentIds = enrolledStudentIds.Except(oldEnrolledStudentIds).ToList();
                if (insertStudentIds.Any())
                {
                    var insertCourseStudents = insertStudentIds.Select(x => new CourseStudent
                    {
                        CourseId = request.CourseId,
                        StudentId = x
                    }).ToList();

                    db.CourseStudents.AddRange(insertCourseStudents);
                }

                // delete
                deleteStudentIds = oldEnrolledStudentIds.Except(enrolledStudentIds).ToList();
                if (deleteStudentIds.Any())
                {
                    var deleteCourseStudents = oldEnrolledStudents.Where(x => deleteStudentIds.Contains(x.StudentId)).ToList();
                    deleteCourseStudents.ForEach(x => x.IsDeleted = 1);

                    db.CourseStudents.UpdateRange(deleteCourseStudents);
                }
                
                db.SaveChanges();

                return new ServiceResponse
                {
                    Status = 1
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse
                {
                    Status = 0,
                    Message = "Update enrolls failed"
                };
            }
        }
    }
}
