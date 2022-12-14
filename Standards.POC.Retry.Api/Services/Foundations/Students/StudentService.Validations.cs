// ---------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using Standards.POC.Retry.Api.Models.Students;
using Standards.POC.Retry.Api.Models.Students.Exceptions;

namespace Standards.POC.Retry.Api.Services.Foundations.Students
{
    public partial class StudentService
    {
        private IEnumerable<(dynamic Rule, string Parameter)> SharedValidations(Student student)
        {
            List<(dynamic Rule, string Parameter)> sharedValidations =
                new List<(dynamic Rule, string Parameter)>{
                    (Rule: IsInvalid(student.Id), Parameter: nameof(Student.Id)),

                    // TODO: Add any other shared structural validation rules 

                    (Rule: IsInvalid(student.CreatedDate), Parameter: nameof(Student.CreatedDate)),
                    (Rule: IsInvalid(student.CreatedByUserId), Parameter: nameof(Student.CreatedByUserId)),
                    (Rule: IsInvalid(student.UpdatedDate), Parameter: nameof(Student.UpdatedDate)),
                    (Rule: IsInvalid(student.UpdatedByUserId), Parameter: nameof(Student.UpdatedByUserId))
                };

            return sharedValidations;
        }

        private void ValidateStudentOnAdd(Student student)
        {
            ValidateStudentIsNotNull(student);

            Validate(
                SharedValidations(student),

                (Rule: IsNotSame(
                    firstDate: student.UpdatedDate,
                    secondDate: student.CreatedDate,
                    secondDateName: nameof(Student.CreatedDate)),
                Parameter: nameof(Student.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: student.UpdatedByUserId,
                    secondId: student.CreatedByUserId,
                    secondIdName: nameof(Student.CreatedByUserId)),
                Parameter: nameof(Student.UpdatedByUserId)),

                (Rule: IsNotRecent(student.CreatedDate), Parameter: nameof(Student.CreatedDate)));
        }

        private void ValidateStudentOnModify(Student student)
        {
            ValidateStudentIsNotNull(student);

            Validate(
                SharedValidations(student),

                (Rule: IsSame(
                    firstDate: student.UpdatedDate,
                    secondDate: student.CreatedDate,
                    secondDateName: nameof(Student.CreatedDate)),
                Parameter: nameof(Student.UpdatedDate)),

                (Rule: IsNotRecent(student.UpdatedDate), Parameter: nameof(student.UpdatedDate)));
        }

        private void ValidateRetrievedStudent(Student student)
        {
            Validate(SharedValidations(student));
        }

        public void ValidateStudentId(Guid studentId) =>
            Validate((Rule: IsInvalid(studentId), Parameter: nameof(Student.Id)));

        private static void ValidateStorageStudent(Student maybeStudent, Guid studentId)
        {
            if (maybeStudent is null)
            {
                throw new NotFoundStudentException(studentId);
            }
        }

        private static void ValidateStudentIsNotNull(Student student)
        {
            if (student is null)
            {
                throw new NullStudentException();
            }
        }

        private static void ValidateAgainstStorageStudentOnModify(Student inputStudent, Student storageStudent)
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputStudent.CreatedDate,
                    secondDate: storageStudent.CreatedDate,
                    secondDateName: nameof(Student.CreatedDate)),
                Parameter: nameof(Student.CreatedDate)),

                (Rule: IsNotSame(
                    firstId: inputStudent.CreatedByUserId,
                    secondId: storageStudent.CreatedByUserId,
                    secondIdName: nameof(Student.CreatedByUserId)),
                Parameter: nameof(Student.CreatedByUserId)),

                (Rule: IsSame(
                    firstDate: inputStudent.UpdatedDate,
                    secondDate: storageStudent.UpdatedDate,
                    secondDateName: nameof(Student.UpdatedDate)),
                Parameter: nameof(Student.UpdatedDate)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
            };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = this.dateTimeBroker.GetCurrentDateTimeOffset();
            TimeSpan timeDifference = currentDateTime.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }

        private static void Validate(
            IEnumerable<(dynamic Rule, string Parameter)> sharedValidations,
            params (dynamic Rule, string Parameter)[] validations)
        {
            List<(dynamic Rule, string Parameter)> allValidations = new List<(dynamic Rule, string Parameter)>();
            allValidations.AddRange(sharedValidations);
            allValidations.AddRange(validations);

            Validate(allValidations.ToArray());
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidStudentException = new InvalidStudentException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidStudentException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidStudentException.ThrowIfContainsErrors();
        }
    }
}