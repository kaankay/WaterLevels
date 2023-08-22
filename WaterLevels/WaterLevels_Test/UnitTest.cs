using WaterLevels;

namespace WaterLevels_Test
{
    public class Tests
    {
        Utilities utilities = new Utilities();
        List<Station> stationList = new List<Station>();
        List<Timeseries> timeSeries = new List<Timeseries>();

        string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory));

        [SetUp]
        public void Setup()
        {
            stationList.Clear();
            timeSeries.Clear();

            stationList.Add(new Station()
            {
                uuid = "47174d8f-1b8e-4599-8a59-b580dd55bc87",
                number = "48900237",
                shortname = "EITZ",
                longname = "EITZE",
                km = 15,
                agency = "VERDEN",
                longitude = 52.90406544743417,
                latitude = 9.276769435375872,

                water = new Water
                {
                    longname = "ALLER",
                    shortname = "ALLER"
                },
            });

            stationList.Add(new Station()
            {
                uuid = "47174d8f-1b8e-4599-8a59-b580dd55bc88",
                number = "55900237",
                shortname = "EITZE 2",
                longname = "EITZE 2",
                km = 15,
                agency = "VERDEN 2",
                longitude = 52.90406544743417,
                latitude = 9.276769435375872,

                water = new Water
                {
                    longname = "ALLER 2",
                    shortname = "ALLER 2"
                },
            });

            timeSeries.Add(new Timeseries() {
                comment = new Comment()
                {
                    longDescription= "long description",
                    shortDescription = "short Description"
                },
                currentMeasurement = new CurrentMeasurement()
                {
                    stateMnwMhw = "stateMnwMhw",
                    stateNswHsw = "stateNswHsw",
                    timestamp = new DateTime(),
                    value = 10
                },
                equidistance = 10,
                gaugeZero = new GaugeZero()
                {
                    unit = "m",
                    validFrom = "10-10-2023",
                    value = 10,
                },
                longname = "longname",
                unit = "m",
                shortname = "shortname"
            });

            timeSeries.Add(new Timeseries()
            {
                comment = new Comment()
                {
                    longDescription = "long description 2",
                    shortDescription = "short Description 2"
                },
                currentMeasurement = new CurrentMeasurement()
                {
                    stateMnwMhw = "stateMnwMhw 2",
                    stateNswHsw = "stateNswHsw 2",
                    timestamp = new DateTime(),
                    value = 15
                },
                equidistance = 15,
                gaugeZero = new GaugeZero()
                {
                    unit = "m",
                    validFrom = "10-12-2023",
                    value = 10,
                },
                longname = "longname 2",
                unit = "m",
                shortname = "shortname 2"
            });
        }

        [Test]
        public void Test_ExportCSV()
        {
            string path = $"{solution_dir}/../TestFileCreated.csv";
            utilities.ExportToCSV(path, stationList, timeSeries);
            Assert.IsTrue(File.Exists(path));
            List<string> linesList = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    linesList.Add(line);
                }

            }
            Assert.AreEqual(linesList.Count, stationList.Count + 1);
            File.Delete(path);
            Assert.Pass();
        }

        [Test]
        public void Test_ExportXML()
        {
            string path = $"{solution_dir}/../TestDataCreated.xml";
            utilities.ExportToXML(path,stationList,timeSeries);
            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(utilities.ValidateXML(path));
            File.Delete(path);
            Assert.Pass();
        }

        [Test]
        public void Test_ImportCSV()
        {
            string path = $"{solution_dir}/../ExportTestFile.csv";
            List<Station> csvImportStationList = new List<Station>();
            List<Timeseries> csvImportTimeseriesList = new List<Timeseries>();
            

            utilities.ImportFromCSV(path, csvImportStationList, csvImportTimeseriesList);
            Assert.AreEqual(csvImportStationList.Count, 3);
            Assert.AreEqual(csvImportStationList.Find(c => c.shortname == "EITZE").shortname, "EITZE");
            Assert.AreEqual(csvImportStationList.Find(c => c.shortname == "RETHEM").shortname, "RETHEM");
        }

        [Test]
        public void Test_ImportXML()
        {
            string path = $"{solution_dir}/../ExportTestFile.xml";
            List<StationExportModel> csvImportStationList = new List<StationExportModel>();


            utilities.ImportFromXML(path, out csvImportStationList);
            Assert.AreEqual(csvImportStationList.Count, 3);
            Assert.AreEqual(csvImportStationList.Find(c => c.shortname == "EITZE").shortname, "EITZE");
            Assert.AreEqual(csvImportStationList.Find(c => c.shortname == "RETHEM").shortname, "RETHEM");
        }

        [Test]
        public void Test_SearchResultExist()
        {
            string searchTeach = "ALLER";
            List<Station> filteredStationList = new List<Station>();
            List<Timeseries> filteredTimeSeriesList = new List<Timeseries>();


            utilities.Search(searchTeach, stationList, timeSeries, out filteredStationList, out filteredTimeSeriesList);
            Assert.AreEqual(filteredStationList.Count, 2);
            Assert.AreEqual(filteredTimeSeriesList.Count, 0);
            Assert.AreEqual(filteredStationList.Find(c => c.water.shortname == searchTeach).water.shortname, searchTeach);
        }

        [Test]
        public void Test_SearchResultNotExist()
        {
            string searchTeach = "ALLER234234";
            List<Station> filteredStationList = new List<Station>();
            List<Timeseries> filteredTimeSeriesList = new List<Timeseries>();


            utilities.Search(searchTeach, stationList, timeSeries, out filteredStationList, out filteredTimeSeriesList);
            Assert.AreEqual(filteredStationList.Count, 0);
            Assert.AreEqual(filteredTimeSeriesList.Count, 0);
            Assert.AreEqual(filteredStationList.Find(c => c.water.shortname == searchTeach)?.water.shortname, null);
        }

        [Test]
        public void Test_Sort()
        {
            Station[] sortedStationArray = new Station[stationList.Count];


            utilities.Sort(stationList, out sortedStationArray);
            string expectedNumberFirst = "48900237";
            string expectedNumberLast = "55900237";
            Assert.AreEqual(sortedStationArray[0].number, expectedNumberFirst);
            Assert.AreEqual(sortedStationArray[1].number, expectedNumberLast);
        }

        [Test] 
        [Apartment(System.Threading.ApartmentState.STA)]
        public void Test_CheckTextBoxes()
        {
            bool check_value;
            EditDataWindow editDataWindow = new EditDataWindow(stationList.First());
            editDataWindow.txtblock_Number.Text = "55400327";
            editDataWindow.txtblock_Agency.Text = "CELLE";
            editDataWindow.txtblock_Km.Text = "15";
            editDataWindow.txtblock_Lat.Text = "9.1";
            editDataWindow.txtblock_Lng.Text = "10.2";
            editDataWindow.txtblock_Shortname.Text = "CELLE";
            editDataWindow.txtblock_Longname.Text = "CELLE";

            check_value = editDataWindow.CheckTextBoxes();
            Assert.AreEqual(true, check_value); 
            editDataWindow.txtblock_Shortname.Text = "";
            check_value = editDataWindow.CheckTextBoxes();
            Assert.AreEqual(false, check_value); 
        }
    }
}