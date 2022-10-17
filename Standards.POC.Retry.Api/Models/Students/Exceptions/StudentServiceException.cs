using System;
using Xeptions;

namespace Standards.POC.Retry.Api.Models.Students.Exceptions
{
    public class StudentServiceException : Xeption
    {
        public StudentServiceException(Exception innerException)
            : base(message: "Student service error occurred, contact support.", innerException)
        { }
    }
}