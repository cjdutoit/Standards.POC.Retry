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
                ValidateStudentOnAdd(student);

                return await this.storageBroker.InsertStudentAsync(student);
            });

        public IQueryable<Student> RetrieveAllStudents() =>
            TryCatch(() => this.storageBroker.SelectAllStudents());
    }
}