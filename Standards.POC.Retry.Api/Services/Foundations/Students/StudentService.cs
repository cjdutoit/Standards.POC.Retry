// ---------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Standards.POC.Retry.Api.Brokers.DateTimes;
using Standards.POC.Retry.Api.Brokers.Loggings;
using Standards.POC.Retry.Api.Brokers.Storages;
using Standards.POC.Retry.Api.Models.Students;

namespace Standards.POC.Retry.Api.Services.Foundations.Students
{
    public partial class StudentService : IStudentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public StudentService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Student> AddStudentAsync(Student student) =>
            TryCatch(async () =>
            {
                return await WithRetry(async () =>
                {
                    ValidateStudentOnAdd(student);

                    return await this.storageBroker.InsertStudentAsync(student);
                });
            });

        public IQueryable<Student> RetrieveAllStudents() =>
            TryCatch(() => this.storageBroker.SelectAllStudents());

        public ValueTask<Student> RetrieveStudentByIdAsync(Guid studentId) =>
            TryCatch(async () =>
            {
                ValidateStudentId(studentId);

                Student maybeStudent = await this.storageBroker
                    .SelectStudentByIdAsync(studentId);

                ValidateStorageStudent(maybeStudent, studentId);
                ValidateRetrievedStudent(maybeStudent);

                return maybeStudent;
            });

        public ValueTask<Student> ModifyStudentAsync(Student student) =>
            TryCatch(async () =>
            {
                ValidateStudentOnModify(student);

                Student maybeStudent =
                    await this.storageBroker.SelectStudentByIdAsync(student.Id);

                ValidateStorageStudent(maybeStudent, student.Id);
                ValidateAgainstStorageStudentOnModify(inputStudent: student, storageStudent: maybeStudent);

                return await this.storageBroker.UpdateStudentAsync(student);
            });

        public ValueTask<Student> RemoveStudentByIdAsync(Guid studentId) =>
            TryCatch(async () =>
            {
                ValidateStudentId(studentId);

                Student maybeStudent = await this.storageBroker
                    .SelectStudentByIdAsync(studentId);

                ValidateStorageStudent(maybeStudent, studentId);

                return await this.storageBroker.DeleteStudentAsync(maybeStudent);
            });
    }
}