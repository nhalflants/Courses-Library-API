using System;

namespace CourseLibrary.API.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset) 
        {
            var dateToCalculateTo = DateTime.UtcNow;
            var age = dateToCalculateTo.Year - dateTimeOffset.Year;

            if (dateToCalculateTo < dateTimeOffset.AddYears(age))
                age--;

            return age; 
        }
    }
}