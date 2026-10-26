using BusinessService.Models.Employee;
using DataConnection.DBContext;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Employees.AnyAsync()) return;

        var seedPeople = new[]
        {
            new { FullName = "Alex", Role = "Product Designer" },
            new { FullName = "Ben", Role = "Chief Operating Officer" },
            new { FullName = "Chloe", Role = "People & Operations Manager" },
            new { FullName = "Daniel", Role = "Co-Founder" },
            new { FullName = "Emma", Role = "Senior UI Designer" },
            new { FullName = "Finn", Role = "Senior Software Engineer" },
            new { FullName = "Grace", Role = "Editor" },
            new { FullName = "Hannah", Role = "Software Engineer" },
            new { FullName = "Isaac", Role = "Head of Sales & Growth" },
            new { FullName = "Jack", Role = "Co-Founder" },
            new { FullName = "Kate", Role = "Chief Executive Officer" },
            new { FullName = "Liam", Role = "Content Creator" },
            new { FullName = "Maya", Role = "Project Manager" },
            new { FullName = "Noah", Role = "Project Manager & SEO Strategist" }
        };

        static (string first, string last) SplitName(string fullName)
        {
            // Keep it simple for demo data:
            // - if more than 1 token, first token = FirstName, remainder = LastName
            // - if single token, LastName = ""
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return ("", "");
            if (parts.Length == 1) return (parts[0], "");
            return (parts[0], string.Join(' ', parts.Skip(1)));
        }

        static decimal SalaryFromRole(string role)
        {
            // Simple heuristic for demo-only numbers (feel free to change).
            role = role.ToLowerInvariant();

            if (role.Contains("chief executive") || role.Contains("ceo")) return 120_000m;
            if (role.Contains("co-founder") || role.Contains("founder")) return 110_000m;
            if (role.Contains("chief operating") || role.Contains("coo")) return 95_000m;
            if (role.Contains("senior software engineer")) return 80_000m;
            if (role.Contains("project manager")) return 65_000m;
            if (role.Contains("head of")) return 75_000m;
            if (role.Contains("designer")) return 60_000m;
            if (role.Contains("people") || role.Contains("operations")) return 55_000m;
            if (role.Contains("content")) return 50_000m;

            return 50_000m;
        }

        var employees = seedPeople
            .Select((p, idx) =>
            {
                var (first, last) = SplitName(p.FullName);

                return new Employee
                {
                    Id = idx + 1,
                    EmployeeNumber = $"EMP{(idx + 1).ToString("D3")}-{p.Role.Replace(' ', '_')}",
                    FirstName = first,
                    LastName = last,
                    Salary = SalaryFromRole(p.Role)
                };
            })
            .ToList();

        db.Employees.AddRange(employees);

        // --- Fake time series data (work hours + overtime) ---
        var start = new DateTime(2025, 1, 1);
        var rnd = new Random(42);

        for (int day = 0; day < 120; day++)
        {
            var date = start.AddDays(day);

            foreach (var e in employees)
            {
                // Normal work hours
                db.WorkHours.Add(new EmployeeWorkHours
                {
                    EmployeeId = e.Id,
                    Date = date,
                    Hours = 7.5m + (decimal)(rnd.NextDouble() * 1.5) // 7.5 - 9.0
                });

                // Overtime (some days)
                if (rnd.Next(0, 5) == 0)
                {
                    db.Overtimes.Add(new EmployeeOvertime
                    {
                        EmployeeId = e.Id,
                        Date = date,
                        Hours = (decimal)(rnd.NextDouble() * 3.0) // 0 - 3
                    });
                }
            }
        }

        // --- Fake absences (by week) ---
        // Just a few random examples.
        db.Absences.AddRange(new[]
        {
            new EmployeeAbsence { EmployeeId = 2,  Year = 2025, WeekNumber = 3,  Reason = "Sick",    HoursAbsent = 7.5m },
            new EmployeeAbsence { EmployeeId = 5,  Year = 2025, WeekNumber = 6,  Reason = "Holiday", HoursAbsent = 15m },
            new EmployeeAbsence { EmployeeId = 11, Year = 2025, WeekNumber = 8,  Reason = "Sick",    HoursAbsent = 7.5m },
        });

        await db.SaveChangesAsync();
    }
}
