using BusinessService.Models.Employee;
using Microsoft.EntityFrameworkCore;

namespace DataConnection.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<EmployeeOvertime> Overtimes => Set<EmployeeOvertime>();
        public DbSet<EmployeeWorkHours> WorkHours => Set<EmployeeWorkHours>();
        public DbSet<EmployeeAbsence> Absences => Set<EmployeeAbsence>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeOvertime>()
                .HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId);

            modelBuilder.Entity<EmployeeWorkHours>()
                .HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId);

            modelBuilder.Entity<EmployeeAbsence>()
                .HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId);
        }
    }
}