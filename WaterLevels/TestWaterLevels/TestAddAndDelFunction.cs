using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterLevels;

namespace TestWaterLevels
{
    [Apartment(ApartmentState.STA)]
    internal class TestAddAndDelFunction
    {
        private MainWindow mainWindow;
        private AddWaterStation addWaterRoot;
        private AddTimestamp addTimestamp;
        private Station station;

        [SetUp]
        public void Setup()
        {
            mainWindow = new MainWindow();
            addWaterRoot = new AddWaterStation(mainWindow){
                txb_uuid = { Text = "47174d8f-1b8e-4599-8a59-b580dd55bc87" },
                txb_number = { Text = "48900237" },
                txb_shortname = { Text = "EITZE" },
                txb_longname = { Text = "EITZE" },
                txb_km = { Text = "9.65" },
                txb_agency = { Text = "VERDEN" },
                txb_longitude = { Text = "9.276769435375872" },
                txb_latitude = { Text = "52.90406544743417" },
                txb_waterLongname = { Text = "ALLER" },
                txb_waterShortname = { Text = "ALLER" }
            };
            Water water = new Water() { longname = "ALLER", shortname = "ALLER" };
            station = new Station() {
                uuid = "47174d8f-1b8e-4599-8a59-b580dd55bc87",
                number = "48900237",
                shortname = "EITZE",
                longname = "EITZE",
                km = 9.65, //Dieser Wert wird nicht so abgespeichert im water object und ich habe keine Ahnung warum, dieser Fehler tritt immer auf wenn "." verwendet wird selbiges bei longitude und latitude manchmal hilft PC neustarten
                agency = "VERDEN",
                longitude = 9.276769435375872,
                latitude = 52.9040654474341,
                water = water
            };

            addTimestamp = new AddTimestamp(station) { 
                txb_shortname = { Text = "W" },
                txb_longname = { Text = "WASSERSTAND ROHDATEN" },
                txb_timeseriesUnit = { Text = "cm" },
                txb_equidistance = { Text = "15" },
                txb_timestamp = { Text = "2023-06-25T22:00:00+02:00" },
                txb_value = { Text = "329" },
                txb_stateMnwMhw = { Text = "normal" },
                txb_stateNswHsw = { Text = "unknown" },
                txb_GaugeZeroUnit = { Text = "m. ü. NN" },
                txb_GaugeZeroValue = { Text = "8" },
                txb_GaugeZeroValidForm = { Text = "1985-03-13" },
                txb_shortDescription = { Text = "short" },
                txb_longDescription = { Text = "long comment" }
            };
            mainWindow.stationList.Add(station);
        }

        [Test]
        public void TestAddStation_ValidData()
        {
            addWaterRoot.AddStationLogic();

            Assert.That(mainWindow.stationList.Count, Is.EqualTo(2));

            Assert.That(station.uuid, Is.EqualTo(mainWindow.stationList[1].uuid));
            Assert.That(station.number, Is.EqualTo(mainWindow.stationList[1].number));
            Assert.That(station.shortname, Is.EqualTo(mainWindow.stationList[1].shortname));
            Assert.That(station.longname, Is.EqualTo(mainWindow.stationList[1].longname));
            Assert.That(station.km, Is.EqualTo(mainWindow.stationList[0].km).Within(0.01));
            Assert.That(station.agency, Is.EqualTo(mainWindow.stationList[0].agency));
            Assert.That(station.longitude, Is.EqualTo(mainWindow.stationList[0].longitude).Within(0.01));
            Assert.That(station.latitude, Is.EqualTo(mainWindow.stationList[0].latitude).Within(0.01));
            Assert.That(station.water!.longname, Is.EqualTo(mainWindow.stationList[1].water!.longname));
            Assert.That(station.water.shortname, Is.EqualTo(mainWindow.stationList[1].water!.shortname));
        }

        [Test]
        public void TestAddTimestamp()
        {
            addTimestamp.AddStationLogic();
            Timeseries timeseries = mainWindow.stationList[0].timeseries![0];

            Assert.That(timeseries.shortname, Is.EqualTo("W"));
            Assert.That(timeseries.longname, Is.EqualTo("WASSERSTAND ROHDATEN"));
            Assert.That(timeseries.unit, Is.EqualTo("cm"));
            Assert.That(timeseries.equidistance, Is.EqualTo(15));
            Assert.That(timeseries.currentMeasurement!.timestamp.ToString, Is.EqualTo("25.06.2023 22:00:00"));
            Assert.That(timeseries.currentMeasurement!.value, Is.EqualTo(329));
            Assert.That(timeseries.currentMeasurement!.stateMnwMhw, Is.EqualTo("normal"));
            Assert.That(timeseries.currentMeasurement!.stateNswHsw, Is.EqualTo("unknown"));
            Assert.That(timeseries.gaugeZero!.unit, Is.EqualTo("m. ü. NN"));
            Assert.That(timeseries.gaugeZero!.value, Is.EqualTo(8));
            Assert.That(timeseries.gaugeZero!.validFrom, Is.EqualTo("1985-03-13"));
            Assert.That(timeseries.comment!.shortDescription, Is.EqualTo("short"));
            Assert.That(timeseries.comment!.longDescription, Is.EqualTo("long comment"));
        }

    }
}

