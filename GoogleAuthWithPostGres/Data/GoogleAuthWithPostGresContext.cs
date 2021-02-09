using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GoogleAuthWithPostGres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleAuthWithPostGres.Data
{
    public class GoogleAuthWithPostGresContext : DbContext
    {
        private IHttpContextAccessor _httpContextAccessor;

        public GoogleAuthWithPostGresContext(DbContextOptions<GoogleAuthWithPostGresContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<User> Users { get; set; }
    }
}
