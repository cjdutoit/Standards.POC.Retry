using System;
using System.Linq;
using System.Threading.Tasks;
using Standards.POC.Retry.Api.Models.Students;

namespace Standards.POC.Retry.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Student> InsertStudentAsync(Student student);
    }
}
