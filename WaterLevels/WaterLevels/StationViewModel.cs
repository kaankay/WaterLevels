using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterLevels
{
    public static class StationViewModel
    {
        public static List<Station>? Stations { get; set; }
        public static List<StationHistoryModel>? StationChangesHistories { get; set; }
    }
}
