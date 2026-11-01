using System.ComponentModel;
using Microsoft.Extensions.AI;

#pragma warning disable MEAI001

public static class DateTimeTools
{
    [AIFunctionName("get_current_time")]
    [Description("Get the current local time, including the UTC offset.")]
    public static string GetCurrentTime()
    {
        DateTimeOffset localNow = DateTimeOffset.Now;
        string formattedTime = localNow.ToString("HH:mm:ss zzz");
        return formattedTime;
    }

    [AIFunctionName("get_current_date")]
    [Description("Get the current local date with its weekday, formatted as ddd yyyy-MM-dd.")]
    public static string GetCurrentDate()
    {
        DateTime localNow = DateTime.Now;
        DateOnly localDate = DateOnly.FromDateTime(localNow);
        string formattedDate = localDate.ToString("ddd yyyy-MM-dd");
        return formattedDate;
    }
}
