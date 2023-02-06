using System.Web.Mvc;
using WebApplication4.ModalViews;
using WebApplication4.Models;
using WebApplication4.Requests;

namespace WebApplication4.Services
{
    public interface IStudentService
    {
        bool VerifyCode(string code);
        ServiceResponse Store(AddStudentRequest student);
        ServiceResponse Update(UpdateStudentRequest request);
        ServiceResponse Delete(int id);
    }

    public class StudentService : IStudentService
    {
        public bool VerifyCode(string code)
        {
            using var db = new StudentDbContext();

            return db.Students.Any(x => x.Code == code && x.IsDeleted == 0);
        }

        public ServiceResponse Store(AddStudentRequest request)
        {
            try
            {
                using var db = new StudentDbContext();

                var checkCode = db.Students.Any(x => x.Code == request.Code && x.IsDeleted == 0);
                if (checkCode)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Code is used"
                    };
                }

                db.Students.Add(new Student
                {
                    Code = request.Code,
                    FullName = request.FullName,
                    Age = request.Age,
                    Address = request.Address
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
                    Message = "Create student failed"
                };
            }
        }

        public ServiceResponse Update(UpdateStudentRequest request)
        {
            try
            {
                using var db = new StudentDbContext();

                var student = db.Students.FirstOrDefault(x => x.Id == request.Id && x.IsDeleted == 0);
                if (student == null)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Student is not exist"
                    };
                }

                student.FullName = request.FullName;
                student.Age = request.Age;
                student.Address = request.Address;
                student.UpdatedAt = DateTime.Now;

                db.Students.Update(student);
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
                    Message = "Update student failed"
                };
            }
        }

        public ServiceResponse Delete(int id)
        {
            try
            {
                using var db = new StudentDbContext();

                var student = db.Students.FirstOrDefault(x => x.Id == id && x.IsDeleted == 0);
                if (student == null)
                {
                    return new ServiceResponse
                    {
                        Status = 0,
                        Message = "Student does not exist"
                    };
                }

                student.IsDeleted = 1;
                student.UpdatedAt = DateTime.Now;

                db.Students.Update(student);
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
                    Message = "Delete student failed"
                };
            }
        }
    }
}
