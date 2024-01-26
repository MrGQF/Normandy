using Microsoft.EntityFrameworkCore;
using System;

namespace Normandy.Identity.UserData.EnityFrameworkCore.DbContexts
{
    public class UserDataContextOptions
    {
        /// <summary>
        /// Callback to configure the EF DbContext.
        /// </summary>
        /// <value>
        /// The configure database context.
        /// </value>
        public Action<DbContextOptionsBuilder> ConfigureDbContext { get; set; }
    }
}
