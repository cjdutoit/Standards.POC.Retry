using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Standards.POC.Retry.Api.Models.Students;
using Standards.POC.Retry.Api.Models.Students.Exceptions;
using Standards.POC.Retry.Api.Services.Foundations.Students;

namespace Standards.POC.Retry.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : RESTFulController
    {
        private readonly IStudentService studentService;

        public StudentsController(IStudentService studentService) =>
            this.studentService = studentService;

        [HttpPost]
        public async ValueTask<ActionResult<Student>> PostStudentAsync(Student student)
        {
            try
            {
                Student addedStudent =
                    await this.studentService.AddStudentAsync(student);

                return Created(addedStudent);
            }
            catch (StudentValidationException studentValidationException)
            {
                return BadRequest(studentValidationException.InnerException);
            }
            catch (StudentDependencyValidationException studentValidationException)
                when (studentValidationException.InnerException is InvalidStudentReferenceException)
            {
                return FailedDependency(studentValidationException.InnerException);
            }
            catch (StudentDependencyValidationException studentDependencyValidationException)
               when (studentDependencyValidationException.InnerException is AlreadyExistsStudentException)
            {
                return Conflict(studentDependencyValidationException.InnerException);
            }
            catch (StudentDependencyException studentDependencyException)
            {
                return InternalServerError(studentDependencyException);
            }
            catch (StudentServiceException studentServiceException)
            {
                return InternalServerError(studentServiceException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Student>> GetAllStudents()
        {
            try
            {
                IQueryable<Student> retrievedStudents =
                    this.studentService.RetrieveAllStudents();

                return Ok(retrievedStudents);
            }
            catch (StudentDependencyException studentDependencyException)
            {
                return InternalServerError(studentDependencyException);
            }
            catch (StudentServiceException studentServiceException)
            {
                return InternalServerError(studentServiceException);
            }
        }

        [HttpGet("{studentId}")]
        public async ValueTask<ActionResult<Student>> GetStudentByIdAsync(Guid studentId)
        {
            try
            {
                Student student = await this.studentService.RetrieveStudentByIdAsync(studentId);

                return Ok(student);
            }
            catch (StudentValidationException studentValidationException)
                when (studentValidationException.InnerException is NotFoundStudentException)
            {
                return NotFound(studentValidationException.InnerException);
            }
            catch (StudentValidationException studentValidationException)
            {
                return BadRequest(studentValidationException.InnerException);
            }
            catch (StudentDependencyException studentDependencyException)
            {
                return InternalServerError(studentDependencyException);
            }
            catch (StudentServiceException studentServiceException)
            {
                return InternalServerError(studentServiceException);
            }
        }
    }
}