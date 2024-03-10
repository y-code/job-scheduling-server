using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobScheduling.Core
{
	public class JobSchedulingDbContextFactory : IDesignTimeDbContextFactory<JobSchedulingDbContext>
	{
		public JobSchedulingDbContextFactory()
		{
		}

        public JobSchedulingDbContext CreateDbContext(string[] args)
        {
			var builder = new DbContextOptionsBuilder<JobSchedulingDbContext>();
			builder.UseNpgsql(JobSchedulingDbContext.DB_CONNECTION);
			return new JobSchedulingDbContext(builder.Options);
        }
    }
}
