using System.Diagnostics;

namespace FlightWatch.Infrastructure.Extensions;

public static class ActivityExtensions
{
    public static Activity? RecordException(this Activity? activity, Exception exception)
    {
        if (activity == null)
            return null;

        var tags = new ActivityTagsCollection
        {
            ["exception.type"] = exception.GetType().FullName,
            ["exception.message"] = exception.Message,
            ["exception.stacktrace"] = exception.StackTrace
        };

        activity.AddEvent(new ActivityEvent("exception", default, tags));

        return activity;
    }
}

