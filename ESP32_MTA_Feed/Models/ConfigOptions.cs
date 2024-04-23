using System;
using System.Collections.Generic;

namespace ESP32_MTA_Feed.Models;

public partial class ConfigOptions
{
    public ConfigOptions(){}
   public List<Route> Routes { get; set; } = new List<Route>();
    public void Add(Route route){
        this.Routes.Add(route);
    }

}
