using System;
using System.Collections.Generic;

namespace ESP32_MTA_Feed.Models;

public partial class Route(string stopName)
{
    public string StopName { get; set; } = stopName;

    public string? RouteId { get; set; }

    public string? StopId { get; set; }
    public string? Direction { get; set; }
    public List<DateTime>? ArrivalTimes { get; set; }
}
