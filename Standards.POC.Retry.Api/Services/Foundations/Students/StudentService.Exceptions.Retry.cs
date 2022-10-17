// ---------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Standards.POC.Retry.Api.Models.Students;

namespace Standards.POC.Retry.Api.Services.Foundations.Students
{
    public partial class StudentService
    {
        private readonly int retriesAllowed = 3;
        private readonly TimeSpan delayBetweenRetries = TimeSpan.FromSeconds(3);

        private readonly List<Type> retryExceptionTypes =
            new List<Type>() { typeof(DbUpdateException), typeof(DbUpdateConcurrencyException) };

        private async ValueTask<Student> WithRetry(ReturningStudentFunction returningStudentFunction)
        {
            var attempts = 0;

            while (true)
            {
                try
                {
                    attempts++;
                    return await returningStudentFunction();
                }
                catch (Exception ex)
                {
                    if (retryExceptionTypes.Any(exception => exception == ex.GetType()))
                    {
                        this.loggingBroker
                            .LogInformation(
                                $"Error found. Retry attempt {attempts}/{retriesAllowed}. " +
                                    $"Exception: {ex.Message}");

                        if (attempts == retriesAllowed)
                        {
                            throw;
                        }

                        Task.Delay(delayBetweenRetries).Wait();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}