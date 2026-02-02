using BusinessService.Models.Employee;
using DataConnection.DBContext;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Employees.AnyAsync()) return;

        // Authors listed on Moonshot Partners "Authors" page (used here as sample employees).
        // Source: https://www.moonshot.partners/author :contentReference[oaicite:2]{index=2}
        var seedPeople = new[]
        {
            new { FullName = "David Gomez", Role = "Product Designer" },
            new { FullName = "Francisco Hernández", Role = "Chief Operating Officer" },
            new { FullName = "Jessica Leon", Role = "People & Operations Manager" },
            new { FullName = "Juan Lagrange", Role = "Co-Founder" },
            new { FullName = "Katherinne Jara Cáceres", Role = "UI Senior Designer and Branding" },
            new { FullName = "Leonardo Cabeza", Role = "Senior Software Engineer" },
            new { FullName = "Moonshot Editorial Team", Role = "Editorial" },
            new { FullName = "Moonshot-Girls", Role = "Women in Tech Group" },
            new { FullName = "Pedro Mejía", Role = "Head of Sales & Growth" },
            new { FullName = "Ricardo Rodriguez", Role = "Co-Founder at Moonshot Partners" },
            new { FullName = "Rubens Pasquale", Role = "Chief Executive Officer" },
            new { FullName = "Sasha Briceño", Role = "Content Creator and Co-Founder of Noodlesoup Studio" },
            new { FullName = "Valentina Delfino", Role = "Project Manager" },
            new { FullName = "Veronica Delfino", Role = "Project Manager & SEO Strategist" }
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
                    EmployeeNumber = $"MS{(idx + 1).ToString("D3")}-{p.Role.Replace(' ', '_')}",
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
