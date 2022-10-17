using System;
using Microsoft.AspNetCore.Identity;

namespace Standards.POC.Retry.Api.Models.Users
{
    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
