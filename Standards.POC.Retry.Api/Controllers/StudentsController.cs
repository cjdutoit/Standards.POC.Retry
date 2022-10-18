// ---------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
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
        [EnableQuery]
        public ActionResult<IQueryable<Student>> Get()
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

        [HttpPut]
        public async ValueTask<ActionResult<Student>> PutStudentAsync(Student student)
        {
            try
            {
                Student modifiedStudent =
                    await this.studentService.ModifyStudentAsync(student);

                return Ok(modifiedStudent);
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

        [HttpDelete("{studentId}")]
        public async ValueTask<ActionResult<Student>> DeleteStudentByIdAsync(Guid studentId)
        {
            try
            {
                Student deletedStudent =
                    await this.studentService.RemoveStudentByIdAsync(studentId);

                return Ok(deletedStudent);
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
            catch (StudentDependencyValidationException studentDependencyValidationException)
                when (studentDependencyValidationException.InnerException is LockedStudentException)
            {
                return Locked(studentDependencyValidationException.InnerException);
            }
            catch (StudentDependencyValidationException studentDependencyValidationException)
            {
                return BadRequest(studentDependencyValidationException);
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