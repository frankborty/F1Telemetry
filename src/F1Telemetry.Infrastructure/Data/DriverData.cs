using F1Telemetry.Core.Models;

namespace F1Telemetry.Infrastructure.Data
{
    public static class DriverData
    {
        public static IReadOnlyList<DriverInfo> Drivers { get; } =
        [
            new() { CarIndex = 1, Driver = "Yuki Tsunoda", Team = "Red Bull Racing", RaceNumber = 22 },
            new() { CarIndex = 2, Driver = "Alexander Albon", Team = "Williams", RaceNumber = 23 },
            new() { CarIndex = 3, Driver = "Lance Stroll", Team = "Aston Martin", RaceNumber = 18 },
            new() { CarIndex = 4, Driver = "Pierre Gasly", Team = "Alpine", RaceNumber = 10 },
            new() { CarIndex = 5, Driver = "Oliver Bearman", Team = "Haas", RaceNumber = 87 },
            new() { CarIndex = 6, Driver = "Max Verstappen", Team = "Red Bull Racing", RaceNumber = 33 },
            new() { CarIndex = 7, Driver = "Lewis Hamilton", Team = "Ferrari", RaceNumber = 44 },
            new() { CarIndex = 8, Driver = "George Russell", Team = "Mercedes", RaceNumber = 63 },
            new() { CarIndex = 9, Driver = "Oscar Piastri", Team = "McLaren", RaceNumber = 81 },
            new() { CarIndex = 10, Driver = "Isack Hadjar", Team = "Racing Bulls", RaceNumber = 6 },
            new() { CarIndex = 11, Driver = "Andrea Kimi Antonelli", Team = "Mercedes", RaceNumber = 12 },
            new() { CarIndex = 12, Driver = "Jack Doohan", Team = "Alpine", RaceNumber = 7 },
            new() { CarIndex = 13, Driver = "Gabriel Bortoleto", Team = "Sauber", RaceNumber = 5 },
            new() { CarIndex = 14, Driver = "Fernando Alonso", Team = "Aston Martin", RaceNumber = 14 },
            new() { CarIndex = 15, Driver = "Carlos Sainz", Team = "Williams", RaceNumber = 55 },
            new() { CarIndex = 16, Driver = "Esteban Ocon", Team = "Haas", RaceNumber = 31 },
            new() { CarIndex = 17, Driver = "Liam Lawson", Team = "Racing Bulls", RaceNumber = 30 },
            new() { CarIndex = 18, Driver = "Nico Hulkenberg", Team = "Sauber", RaceNumber = 27 },
            new() { CarIndex = 19, Driver = "Charles Leclerc", Team = "Ferrari", RaceNumber = 16 }
        ];
    }
}
