using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterLevels
{
    public class StationHistoryModel
    {
        public string? Uuid { get; set; }
        public string? Number { get; set; }
        public string? Shortname { get; set; }
        public string? Longname { get; set; }
        public double km { get; set; }
        public string? Agency { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
}
