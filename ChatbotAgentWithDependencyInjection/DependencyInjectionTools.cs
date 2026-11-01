using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable MEAI001

public static class DependencyInjectionTools
{
    [AIFunctionName("get_current_time")]
    [Description("Get the current local time, including the UTC offset.")]
    public static string GetCurrentTime(IServiceProvider services)
    {
        TimeProvider timeProvider = services.GetRequiredService<TimeProvider>();
        DateTimeOffset localNow = timeProvider.GetLocalNow();
        return localNow.ToString("HH:mm:ss zzz");
    }

    [AIFunctionName("get_current_date")]
    [Description("Get the current local date with its weekday, formatted as ddd yyyy-MM-dd.")]
    public static string GetCurrentDate(IServiceProvider services)
    {
        TimeProvider timeProvider = services.GetRequiredService<TimeProvider>();
        DateTimeOffset localNow = timeProvider.GetLocalNow();
        DateOnly localDate = DateOnly.FromDateTime(localNow.DateTime);
        return localDate.ToString("ddd yyyy-MM-dd");
    }
}