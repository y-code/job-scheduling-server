using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace JobSchedulingServer
{
	public class JobSchedulingDbContext : DbContext
	{
        public const string DB_CONNECTION = "User ID=postgres;Password=asdf;Host=localhost;Port=5432;Database=job_scheduling;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=30;Connection Lifetime=0;";

        public JobSchedulingDbContext(DbContextOptions<JobSchedulingDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddQuartz(builder => builder.UsePostgreSql());
        }
    }
}
