using System;

namespace Disnegativos.Shared.Interfaces;

public interface ITimeZoneService
{
    void SetUserTimeZone(string timeZoneId);
    DateTime ToUserTime(DateTime utcDateTime);
    DateTime ToUtc(DateTime localDateTime);
    string FormatForUser(DateTime utcDateTime, string format = "g");
}
