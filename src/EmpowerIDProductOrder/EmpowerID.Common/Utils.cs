using System;

namespace EmpowerID.Common
{
    public static class Utils
    {
        private const string LoremIpsum = @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        public static string SequentialGuid()
        {
            return Ulid.NewUlid().ToString();
        }
        public static string GenerateRandomString()
        {
            return string.Join(Environment.NewLine, Enumerable.Repeat(LoremIpsum, Random.Shared.Next(5, 15)));
        }

        public static DateTime GetRandomDateTime(int minYear = 2000, int maxYear = 2024)
        {
            var random = new Random();
            var year = random.Next(minYear, maxYear);
            var month = random.Next(1, 12);
            var noOfDaysInMonth = DateTime.DaysInMonth(year, month);
            var day = random.Next(1, noOfDaysInMonth);

            return new DateTime(year, month, day);
        }
    }
}
