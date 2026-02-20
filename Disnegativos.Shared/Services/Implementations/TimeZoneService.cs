using System;
using System.Globalization;
using Disnegativos.Shared.Interfaces;

namespace Disnegativos.Shared.Services.Implementations;

public class TimeZoneService : ITimeZoneService
{
    private TimeZoneInfo _userTimeZone = TimeZoneInfo.Utc;

    public void SetUserTimeZone(string timeZoneId)
    {
        try
        {
            _userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            _userTimeZone = TimeZoneInfo.Utc;
        }
    }

    public DateTime ToUserTime(DateTime utcDateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _userTimeZone);
    }

    public DateTime ToUtc(DateTime localDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, _userTimeZone);
    }

    public string FormatForUser(DateTime utcDateTime, string format = "g")
    {
        return ToUserTime(utcDateTime).ToString(format, CultureInfo.CurrentCulture);
    }
}
