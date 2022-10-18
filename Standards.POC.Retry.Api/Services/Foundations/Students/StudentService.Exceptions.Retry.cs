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
using static Standards.POC.Retry.Api.Services.Foundations.Students.StudentService;

namespace Standards.POC.Retry.Api.Services.Foundations.Students
{
    public partial class StudentService
    {
        private readonly int retriesAllowed = 3;
        private readonly TimeSpan delayBetweenRetries = TimeSpan.FromMilliseconds(3);

        private readonly List<Type> retryExceptionTypes =
            new List<Type>()
            {
                typeof(DbUpdateException),
                typeof(DbUpdateConcurrencyException)
            };

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

    public static class Retry
    {
        public static ValueTask<Student> RetryIfFailed
                          (this ReturningStudentFunction func)
        {
            return () =>
            {
                var attempts = 0;
                int maxRetry = 3;
                TimeSpan delayBetweenRetries = TimeSpan.FromMicroseconds(3);

                List<Type> retryExceptionTypes =
                    new List<Type>()
                    {
                        typeof(DbUpdateException),
                        typeof(DbUpdateConcurrencyException)
                    };

                while (true)
                {
                    try
                    {
                        attempts++;
                        return func();
                    }
                    catch (Exception ex)
                    {
                        if (retryExceptionTypes.Any(exception => exception == ex.GetType()))
                        {
                            if (attempts == maxRetry)
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
            };
        }
    }
}