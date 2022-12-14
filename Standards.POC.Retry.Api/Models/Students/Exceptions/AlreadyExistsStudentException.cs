using System;
using Xeptions;

namespace Standards.POC.Retry.Api.Models.Students.Exceptions
{
    public class AlreadyExistsStudentException : Xeption
    {
        public AlreadyExistsStudentException(Exception innerException)
            : base(message: "Student with the same Id already exists.", innerException)
        { }
    }
}