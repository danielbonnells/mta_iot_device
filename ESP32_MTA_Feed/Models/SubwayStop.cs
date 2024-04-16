using System;
using System.Collections.Generic;

namespace ESP32_MTA_Feed.Models;

public partial class SubwayStop
{
    public string? StopId { get; set; }

    public string? StopName { get; set; }

    public string? StopLat { get; set; }

    public string? StopLon { get; set; }

    public string? LocationType { get; set; }

    public string? ParentStation { get; set; }
}
