using Nancy.Json;
using Nancy.ModelBinding.DefaultBodyDeserializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Net.Http.Json;
using System.Net.Http;
using Microsoft.Win32;
using System.Globalization;
using System.Reflection;
using System.Xml.Schema;

namespace WaterLevels
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Utilities utilities = new Utilities();

        public List<Station> stationList = new List<Station>();

        List<StationExportModel> stationExportModels = new List<StationExportModel>();


        public List<Station> stationID = new List<Station>();

        public List<Station> mapStations = new List<Station>();

        public List<Timeseries> timeseriesList = new List<Timeseries>();


        public double MapWidth = 400;
        public double MapHeight = 630;

        //Lat-long coordinates for Germany (Source: https://latitudelongitude.org/de/)
        public double MinLatitude = 47.40724;
        public double MaxLatitude = 54.9079;
        public double MinLongitude = 5.98815;
        public double MaxLongitude = 14.98853;

        public double scaledLatitude;
        public double scaledLongitude;

        public MainWindow()
        {
            InitializeComponent();

            MapStationList();

            ScaleFactors();

            DrawStation();
        }

        // Calculation of Scale Factor => Scale Factor = Dimensions of the new shape / Dimensions of the original shape
        //Source: https://www.cuemath.com/geometry/scale-factor/#
        public void ScaleFactors()
        {
            scaledLatitude = MapHeight / (MaxLatitude - MinLatitude);
            scaledLongitude = MapWidth / (MaxLongitude - MinLongitude);
        }

        public void DrawStation()
        {

            foreach (Station station in mapStations)
            {
                string stringKm = Convert.ToString(station.km);


                //This puts pinpoints on the map, to show stations
                Ellipse pinpoint = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red,
                    ToolTip = $"uuid: {station.uuid}\nnumber: {station.number}\nshortname: {station.shortname}\nlongname: {station.longname}\nkm: {stringKm}\nagency: {station.agency}\nwater: {station.water?.shortname}"

                };

                string? shortnameLower = station.shortname?.ToLower();

                //This shows the names of the stations.
                TextBlock statName = new TextBlock
                {
                    Text = shortnameLower,
                    Foreground = Brushes.Black,
                    FontSize = 10,
                    ToolTip = $"uuid: {station.uuid}\nnumber: {station.number}\nshortname: {station.shortname}\nlongname: {station.longname}\nkm: {stringKm}\nagency: {station.agency}\nwater: {station.water?.shortname}"
                };

                double distanceToLeft = (station.longitude - MinLongitude) * scaledLongitude;
                double distanceToTop = (MaxLatitude - station.latitude) * scaledLatitude;

                Canvas.SetLeft(pinpoint, distanceToLeft);
                Canvas.SetTop(pinpoint, distanceToTop);
                Canvas.SetLeft(statName, distanceToLeft + 15);
                Canvas.SetTop(statName, distanceToTop - 15);

                pinpoint.MouseLeftButtonDown += (sender, e) =>
                {
                    SelectedStation(station);

                };

                myCanvas.Children.Add(pinpoint);
                myCanvas.Children.Add(statName);

            }
        }

        public dynamic GetAPI(string url)
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = httpClient.Send(request);
            using var reader = new StreamReader(response.Content.ReadAsStream());
            var responseBody = reader.ReadToEnd();

            //https://stackoverflow.com/questions/49385536/serialize-datetime-property-isodatetimeconverter-and-serverprotocolviolation-err
            var serializeSettings = new JsonSerializerSettings();
            serializeSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyyMMddTHHmmssZ" });

            var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody, serializeSettings)!;

            return jsonObject;
        }

        public void MapStationList()
        {
            string url = "https://www.pegelonline.wsv.de/webservices/rest-api/v2/stations.json?limit=40&offset=0";

            var jsonObjects = GetAPI(url);


            foreach (var jsonObject in jsonObjects)
            {
                Station station = new Station
                {
                    uuid = jsonObject["uuid"],
                    number = jsonObject["number"],
                    shortname = jsonObject["shortname"],
                    longname = jsonObject["longname"],
                    km = Convert.ToDouble(jsonObject["km"]),
                    agency = jsonObject["agency"].ToString(),
                    longitude = Convert.ToDouble(jsonObject["longitude"]),
                    latitude = Convert.ToDouble(jsonObject["latitude"]),

                    water = new Water
                    {
                        longname = jsonObject["water"]["longname"],
                        shortname = jsonObject["water"]["shortname"]

                    }
                };
                mapStations.Add(station);
            }
        }

        public void API_GetStationList()
        {
            string url = "https://www.pegelonline.wsv.de/webservices/rest-api/v2/stations.json?limit=40&offset=0";

            var jsonObjects = GetAPI(url);


            foreach (var jsonObject in jsonObjects)
            {
                Station station = new Station
                {
                    uuid = jsonObject["uuid"],
                    shortname = jsonObject["shortname"]
                };
                stationID.Add(station);
            }
        }

        public void getWaterData(string stationIDNummer)
        {
            try
            {

                //https://www.pegelonline.wsv.de/webservices/rest-api/v2/stations/593647aa-9fea-43ec-a7d6-6476a76ae868.json?includeTimeseries=true&includeCurrentMeasurement=true&includeCharacteristicValues=true

                var url = "https://www.pegelonline.wsv.de/webservices/rest-api/v2/stations/" + stationIDNummer + ".json?includeTimeseries=true&includeCurrentMeasurement=true&includeCharacteristicValues=true";

                var jsonObj = GetAPI(url);

                var input = jsonObj["timeseries"][0]["currentMeasurement"]["timestamp"];

                var gaugeZeroInput = jsonObj["timeseries"][0]["gaugeZero"];

                Station waterProperties = new Station
                {
                    uuid = jsonObj["uuid"],
                    number = jsonObj["number"],
                    shortname = jsonObj["shortname"],
                    longname = jsonObj["longname"],
                    km = Convert.ToDouble(jsonObj["km"]),
                    agency = jsonObj["agency"].ToString(),
                    longitude = Convert.ToDouble(jsonObj["longitude"]),
                    latitude = Convert.ToDouble(jsonObj["latitude"]),

                    water = new Water
                    {
                        longname = jsonObj["water"]["longname"],
                        shortname = jsonObj["water"]["shortname"]

                    },

                    timeseries = new List<Timeseries>()


                };


                //Some Stations have no input about gaugeZero. 
                Timeseries timeseriesObj;
                if (gaugeZeroInput != null)
                {
                    timeseriesObj = new Timeseries
                    {
                        shortname = jsonObj["timeseries"][0]["shortname"],
                        longname = jsonObj["timeseries"][0]["longname"],
                        unit = jsonObj["timeseries"][0]["unit"],
                        equidistance = Convert.ToInt32(jsonObj["timeseries"][0]["equidistance"]),
                        currentMeasurement = new CurrentMeasurement
                        {
                            timestamp = DateTime.Parse((input).ToString()),
                            value = Convert.ToDouble(jsonObj["timeseries"][0]["currentMeasurement"]["value"]),
                            stateMnwMhw = jsonObj["timeseries"][0]["currentMeasurement"]["stateMnwMhw"],
                            stateNswHsw = jsonObj["timeseries"][0]["currentMeasurement"]["stateNswHsw"],

                        },
                        gaugeZero = new GaugeZero
                        {
                            unit = jsonObj["timeseries"][0]["gaugeZero"]["unit"],
                            value = Convert.ToDouble(jsonObj["timeseries"][0]["gaugeZero"]["value"]),
                            validFrom = jsonObj["timeseries"][0]["gaugeZero"]["validFrom"]
                        }
                    };

                }
                else
                {
                    timeseriesObj = new Timeseries
                    {
                        shortname = jsonObj["timeseries"][0]["shortname"],
                        longname = jsonObj["timeseries"][0]["longname"],
                        unit = jsonObj["timeseries"][0]["unit"],
                        equidistance = Convert.ToInt32(jsonObj["timeseries"][0]["equidistance"]),
                        currentMeasurement = new CurrentMeasurement
                        {
                            timestamp = DateTime.Parse((input).ToString()),
                            value = Convert.ToDouble(jsonObj["timeseries"][0]["currentMeasurement"]["value"]),
                            stateMnwMhw = jsonObj["timeseries"][0]["currentMeasurement"]["stateMnwMhw"],
                            stateNswHsw = jsonObj["timeseries"][0]["currentMeasurement"]["stateNswHsw"],

                        },
                        gaugeZero = new GaugeZero
                        {
                            unit = "unknown",
                            value = 0.0,
                            validFrom = "unknown"
                        }
                    };
                }
                waterProperties.timeseries.Add(timeseriesObj);
                stationList.Add(waterProperties);
                //timeseriesList.Add(timeseriesObj); 
                updateDG_WaterData(waterProperties);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectedStation(Station stat)
        {
            try
            {
                string stationIDNummer = "";

                string? stationName = stat.shortname;

                API_GetStationList();

                for (int i = 0; i < 40; i++)
                {
                    if (stationName == stationID[i].shortname)
                    {
                        stationIDNummer = stationID[i].uuid!;
                        break;
                    }
                }

                getWaterData(stationIDNummer);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bt_add_Click(object sender, RoutedEventArgs e)
        {
            if (dg_waterData.SelectedItem != null)
            {
                if (dg_waterData.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Bitte wählen sie nur eine Station aus.");
                    return;
                }

                var selectedItemIndex = dg_waterData.Items.IndexOf(dg_waterData.SelectedItem);
                Station selectedItem = stationList.ElementAt(selectedItemIndex);

                stationList.Remove(selectedItem);
                AddTimestamp timestamp = new AddTimestamp(selectedItem!);
                timestamp.Owner = this;
                timestamp.ShowDialog();
                stationList.Add(selectedItem!);
                updateDG_WaterData(selectedItem);
            }
            else
            {
                AddWaterStation newStation = new AddWaterStation(this);
                newStation.Owner = this;
                newStation.ShowDialog();
                updateDG_WaterData();
            }
        }

        private void bt_edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItemIndex = dg_waterData.Items.IndexOf(dg_waterData.SelectedItem);
                if (selectedItemIndex == -1)
                {
                    MessageBox.Show("Please select station to Edit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                StationViewModel.Stations = stationList;

                Station selectedItem = StationViewModel.Stations.ElementAt(selectedItemIndex);


                EditDataWindow editDataWindow = new EditDataWindow(selectedItem);
                editDataWindow.ShowDialog();

                // if the window has a result, refresh the listbox
                //if (editDataWindow.DialogResult.HasValue && editDataWindow.DialogResult.Value)
                stationList = StationViewModel.Stations;
                updateDG_WaterData();
                //dg_waterData.ItemsSource = null;
                //dg_waterData.Items.Clear();
                //dg_waterData.Items.Refresh();
                //dg_waterData.ItemsSource = stationList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bt_delete_Click(object sender, RoutedEventArgs e)
        {

            if (dg_timeSeries.SelectedItems != null && dg_timeSeries.SelectedItems!.Count != 0)
            {
                if (dg_timeSeries.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Bitte wählen sie nur eine Messung aus.");
                    return;
                }

                var selectedItemIndex = dg_timeSeries.Items.IndexOf(dg_timeSeries.SelectedItem);
                Timeseries selectedItem = timeseriesList.ElementAt(selectedItemIndex);
                foreach (Station current in stationList)
                {
                    if (current.timeseries!.Equals(timeseriesList))
                    {
                        dg_timeSeries.SelectedItems.Clear();
                        current.timeseries.RemoveAt(selectedItemIndex);
                        updateDG_WaterData(current);
                        return;
                    }
                }
                MessageBox.Show("Fehler beim entfernen eines Elements. Bitte aktualisieren sie die Messwerte durch Klicken auf andere Stationen.");
            }
            else if (dg_waterData.SelectedItem != null && dg_waterData.SelectedItems!.Count != 0)
            {
                if (dg_waterData.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Bitte wählen sie nur eine Station aus.");
                    return;
                }

                var selectedItemIndex = dg_waterData.Items.IndexOf(dg_waterData.SelectedItem); 
                Station selectedItem = stationList.ElementAt(selectedItemIndex);
                foreach (Station current in stationList)
                {
                    if (current.uuid!.Equals(selectedItem.uuid))
                    {
                        stationList.Remove(current);
                        break;
                    }
                }
                if(stationList.Count > 0)
                    updateDG_WaterData(stationList.ElementAt(0));
                else 
                { 
                    updateDG_WaterData();
                    dg_timeSeries.Items.Clear();
                    timeseriesList.Clear();
                }
            }
            else
                MessageBox.Show("Bitte wählen sie ein Element aus!");
        }

        private void bt_filter_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (String.IsNullOrEmpty(txtblock_Search.Text))
                {
                    dg_waterData.Items.Clear();
                    foreach (Station current in stationList)
                    {
                        dg_waterData.Items.Add(current);
                    }

                    dg_timeSeries.Items.Clear();
                    foreach (Timeseries current in timeseriesList)
                    {
                        dg_timeSeries.Items.Add(current);
                    }

                    return;
                }

                string searchText = txtblock_Search.Text; ;
                List<Station> filteredList = new List<Station>();
                List<Timeseries> filteredTimeSeriesList = new List<Timeseries>();

                /*var*/ filteredTimeSeriesList = timeseriesList.Where(a => a.equidistance.ToString().Contains(searchText) || a.unit!.Contains(searchText) || a.longname!.Contains(searchText) || a.shortname!.Contains(searchText) || a.currentMeasurement!.value.ToString().Contains(searchText) || a.currentMeasurement.timestamp.ToString().Contains(searchText) || a.currentMeasurement!.stateNswHsw!.Contains(searchText) || a.currentMeasurement!.stateMnwMhw!.Contains(searchText) || a.gaugeZero!.unit!.Contains(searchText) || a.gaugeZero!.validFrom!.Contains(searchText) || a.gaugeZero.value.ToString().Contains(searchText)).ToList();

                dg_waterData.Items.Clear();
                foreach (Station current in filteredList)
                {
                    dg_waterData.Items.Add(current);
                }

                dg_timeSeries.Items.Clear();
                foreach (Timeseries current in filteredTimeSeriesList)
                {
                    dg_timeSeries.Items.Add(current);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void bt_export_csv_Click(object sender, RoutedEventArgs e)
        {
            if (stationList == null || stationList.Count <= 0)
            {
                MessageBox.Show("Stations List is empty. Nothing to export", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string dateNowStamp = DateTimeOffset.Now.ToString("yyy_MM_dd-HH_mm_ss_fff");
            SaveFileDialog fileDailouge = new SaveFileDialog();
            fileDailouge.Filter = "CSV|*.csv";
            fileDailouge.FileName = dateNowStamp;
            fileDailouge.ShowDialog();

            if (fileDailouge.FileName != dateNowStamp)
            {
                utilities.ExportToCSV(fileDailouge.FileName, stationList, timeseriesList);
            }
            else MessageBox.Show("Export Declined.");
        }

        private void bt_export_xml_Click(object sender, RoutedEventArgs e)
        {
            if (stationList == null || stationList.Count <= 0)
            {
                MessageBox.Show("Stations List is empty. Nothing to export", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string dateNowStamp = DateTimeOffset.Now.ToString("yyy_MM_dd-HH_mm_ss_fff");
            SaveFileDialog fileDailoug = new SaveFileDialog();
            fileDailoug.Filter = "XMl|*.xml";
            fileDailoug.FileName = dateNowStamp;
            fileDailoug.ShowDialog();

            if (fileDailoug.FileName != dateNowStamp)
            {
                utilities.ExportToXML(fileDailoug.FileName, stationList, timeseriesList);
            }
            else MessageBox.Show("Export Declined.");
        }


        private void bt_import_csv_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDailouge = new OpenFileDialog();
            fileDailouge.Filter = "CSV|*.csv";
            fileDailouge.ShowDialog();
            if (fileDailouge.FileName == null && fileDailouge.FileName == "")
            {
                MessageBox.Show("No File selected to import stations", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                utilities.ImportFromCSV(fileDailouge.FileName!, stationList, timeseriesList);

                if (stationList.Count > 0)
                {
                    dg_waterData.Items.Clear();
                    dg_timeSeries.Items.Clear();
                    for (int i = 0; i < stationList.Count; i++)
                    {
                        dg_waterData.Items.Add(stationList[i]);
                        dg_timeSeries.Items.Add(timeseriesList[i]);
                    }
                    MessageBox.Show("Import from CSV-File was successful.");
                }
            }

        }

        private void bt_import_xml_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDailouge = new OpenFileDialog();
            fileDailouge.Filter = "XML|*.xml";
            fileDailouge.ShowDialog();
            if (fileDailouge.FileName == null && fileDailouge.FileName == "")
            {
                MessageBox.Show("No File selected to import stations", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {

                utilities.ImportFromXML(fileDailouge.FileName!, out stationExportModels);

                if (stationExportModels.Count > 0)
                {
                    foreach (StationExportModel station in stationExportModels)
                    {
                        stationList.Add(new Station()
                        {
                            agency = station.agency,
                            km = station.km,
                            latitude = station.latitude,
                            longitude = station.longitude,
                            longname = station.longname,
                            number = station.number,
                            shortname = station.shortname,
                            uuid = station.uuid,
                            water = station.water,
                        });
                        dg_waterData.Items.Add(station);

                        timeseriesList.Add(station.timeseries!);
                        dg_timeSeries.Items.Add(station.timeseries);
                    }
                    MessageBox.Show("Import from XML-File was successful.");
                }

            }

        }

        private bool updateSelection = false;
        private void DataGridClick(object sender, RoutedEventArgs e)
        {                
            if (dg_waterData.SelectedItem == null) return;
            if (dg_waterData.SelectedItems.Count > 1) return;
            if (updateSelection) return;

            var selectedItemIndex = dg_waterData.Items.IndexOf(dg_waterData.SelectedItem);
            Station selectedItem = stationList.ElementAt(selectedItemIndex);

            updateSelection = true;
            updateDG_WaterData(selectedItem);
            dg_waterData.SelectedIndex = selectedItemIndex;
            updateSelection = false;
        }

        public void updateDG_WaterData(Station? station)
        {
            if(station == null) return;

            dg_waterData.Items.Clear();
            foreach (Station current in stationList)
            {
                dg_waterData.Items.Add(current);
            }
            timeseriesList = station!.timeseries!;
            dg_timeSeries.Items.Clear();
            foreach (Timeseries current in timeseriesList)
            {
                dg_timeSeries.Items.Add(current);
            }

        }

        public void updateDG_WaterData()
        {
            dg_waterData.Items.Clear();
            foreach (Station current in stationList)
            {
                dg_waterData.Items.Add(current);
            }
        }

        private void bt_Sort_Click(object sender, RoutedEventArgs e)
        {
            Station[] sortedArray = new Station[stationList.Count];

            utilities.Sort(stationList, out sortedArray);

            dg_waterData.Items.Clear();
            for (int i = 0; i < sortedArray.Length; i++)
            {
                dg_waterData.Items.Add(sortedArray[i]);
            }
        }
    }
}

