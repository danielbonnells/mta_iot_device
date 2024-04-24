using System;
using System.Collections.Generic;

namespace ESP32_MTA_Feed.Models;

public partial class Route
{
    public Route(string stopName)
    {
        StopName = stopName;
    }

    public Route(Route original)
    {
        StopName = original.StopName;
        RouteId = original.RouteId;
        StopId = original.StopId;
        Direction = original.Direction;
        ArrivalTimes = original.ArrivalTimes != null ? new List<DateTime>(original.ArrivalTimes) : null;
    }
    public string StopName { get; set; }

    public string? RouteId { get; set; }

    public string? StopId { get; set; }
    public string? Direction { get; set; }
    public List<DateTime>? ArrivalTimes { get; set; }

}
