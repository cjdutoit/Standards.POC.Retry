using System;
using Xeptions;

namespace Standards.POC.Retry.Api.Models.Students.Exceptions
{
    public class NotFoundStudentException : Xeption
    {
        public NotFoundStudentException(Guid studentId)
            : base(message: $"Couldn't find student with studentId: {studentId}.")
        { }
    }
}