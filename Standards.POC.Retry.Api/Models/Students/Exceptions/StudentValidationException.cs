using Xeptions;

namespace Standards.POC.Retry.Api.Models.Students.Exceptions
{
    public class StudentValidationException : Xeption
    {
        public StudentValidationException(Xeption innerException)
            : base(message: "Student validation errors occurred, please try again.",
                  innerException)
        { }
    }
}