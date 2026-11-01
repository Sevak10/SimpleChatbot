using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType]
public class DateTimeTools
{
    private readonly TimeProvider _timeProvider;

    public DateTimeTools(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    [McpServerTool(Name = "get_current_time")]
    [Description("Get the current local time, including the UTC offset.")]
    public string GetCurrentTime()
    {
        DateTimeOffset localNow = _timeProvider.GetLocalNow();
        string formattedTime = localNow.ToString("HH:mm:ss zzz");
        return formattedTime;
    }

    [McpServerTool(Name = "get_current_date")]
    [Description("Get the current local date with its weekday, formatted as ddd yyyy-MM-dd.")]
    public string GetCurrentDate()
    {
        DateTimeOffset localNow = _timeProvider.GetLocalNow();
        DateOnly localDate = DateOnly.FromDateTime(localNow.DateTime);
        string formattedDate = localDate.ToString("ddd yyyy-MM-dd");
        return formattedDate;
    }
}