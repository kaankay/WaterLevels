using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterLevels
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Comment
    {
        private string? _shortDescription;
        private string? _longDescription;

        public string? shortDescription { get => _shortDescription; set => _shortDescription = value; }
        public string? longDescription { get => _longDescription; set => _longDescription = value; }
    }

    public class CurrentMeasurement
    {
        private DateTime _timestamp;
        private double _value;
        private string? _stateMnwMhw;
        private string? _stateNswHsw;


        public DateTime timestamp { get => _timestamp; set => _timestamp = value; }
        public double value { get => _value; set => _value = value; }
        public string? stateMnwMhw { get => _stateMnwMhw; set => _stateMnwMhw = value; }
        public string? stateNswHsw { get => _stateNswHsw; set => _stateNswHsw = value; }
    }

    public class GaugeZero
    {
        private string? _unit;
        private double _value;
        private string? _validFrom;

        public string? unit { get => _unit; set => _unit = value; }
        public double value { get => _value; set => _value = value; }
        public string? validFrom { get => _validFrom; set => _validFrom = value; }
    }

    public class Station : IComparable
    {
        private string? _uuid;
        private string? _number;
        private string? _shortname;
        private string? _longname;
        private double _km;
        private string? _agency;
        private double _longitude;
        private double _latitude;
        private Water? _water;
        List<Timeseries>? _timeseries;



        public string? uuid { get => _uuid; set => _uuid = value; }
        public string? number { get => _number; set => _number = value; }
        public string? shortname { get => _shortname; set => _shortname = value; }
        public string? longname { get => _longname; set => _longname = value; }
        public double km { get => _km; set => _km = value; }
        public string? agency { get => _agency; set => _agency = value; }
        public double longitude { get => _longitude; set => _longitude = value; }
        public double latitude { get => _latitude; set => _latitude = value; }
        public Water? water { get => _water; set => _water = value; }
        public List<Timeseries>? timeseries { get => _timeseries; set => _timeseries = value; }

        public int CompareTo(object incomingobject)
        {
            Station? incomingStation = incomingobject as Station;

            return this.number!.CompareTo(incomingStation!.number);
        }

    }

    public class Timeseries
    {
        private string? _shortname;
        private string? _longname;
        private string? _unit;
        private int _equidistance;
        private CurrentMeasurement? _currentMeasurement;
        private GaugeZero? _gaugeZero;
        private Comment? _comment;

        public Timeseries()
        {

        }

        public string? shortname { get => _shortname; set => _shortname = value; }
        public string? longname { get => _longname; set => _longname = value; }
        public string? unit { get => _unit; set => _unit = value; }
        public int equidistance { get => _equidistance; set => _equidistance = value; }
        public CurrentMeasurement? currentMeasurement { get => _currentMeasurement; set => _currentMeasurement = value; }
        public GaugeZero? gaugeZero { get => _gaugeZero; set => _gaugeZero = value; }
        public Comment? comment { get => _comment; set => _comment = value; }
    }

    public class Water
    {
        private string? _shortname;
        private string? _longname;

        public string? shortname { get => _shortname; set => _shortname = value; }
        public string? longname { get => _longname; set => _longname = value; }
    }

    public class StationExportModel
    {
        private string? _uuid;
        private string? _number;
        private string? _shortname;
        private string? _longname;
        private double _km;
        private string? _agency;
        private double _longitude;
        private double _latitude;
        private Water? _water;
        private Timeseries? _timeseries;



        public string? uuid { get => _uuid; set => _uuid = value; }
        public string? number { get => _number; set => _number = value; }
        public string? shortname { get => _shortname; set => _shortname = value; }
        public string? longname { get => _longname; set => _longname = value; }
        public double km { get => _km; set => _km = value; }
        public string? agency { get => _agency; set => _agency = value; }
        public double longitude { get => _longitude; set => _longitude = value; }
        public double latitude { get => _latitude; set => _latitude = value; }
        public Water? water { get => _water; set => _water = value; }
        public Timeseries? timeseries { get => _timeseries; set => _timeseries = value; }
    }
}
