using Microsoft.Win32;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WaterLevels
{
    public class Utilities
    {
        public void ExportToCSV(string filePath, List<Station> stationList, List<Timeseries> TimeseriesList)
        {
            try
            {

                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    //Define field names for CSV file Header
                    sw.WriteLine("id; number; shortname; longname; km; agency; latitude; longitude; water longname; water shortname; short desc; long desc; unit; equidistance(min); timestamp; value; statemnwmhw; statenswhsw; unit of level zero(m); Height(decimal); valid from ");

                    //Filling fields with each entry of List of Stations 

                    for (int i = 0; i < stationList.Count; i++)
                    {
                        var model = stationList[i];
                        var timeSeriesModel = TimeseriesList[i];
                        string row = model.uuid + ";" + model.number + ";" + model.shortname + ";" + model.longname + ";" + model.km + ";" + model.agency + ";" + model.latitude + ";" + model.longitude + ";" + model.water.longname + ";" + model.water.shortname + ";" + timeSeriesModel.shortname + ";" + timeSeriesModel.longname + ";" + timeSeriesModel.unit + ";" + timeSeriesModel.equidistance + ";" + timeSeriesModel.currentMeasurement.timestamp + ";" + timeSeriesModel.currentMeasurement.value + ";" + timeSeriesModel.currentMeasurement.stateMnwMhw + ";" + timeSeriesModel.currentMeasurement.stateNswHsw + ";" + timeSeriesModel.gaugeZero.unit + ";" + timeSeriesModel.gaugeZero.value + ";" + timeSeriesModel.gaugeZero.validFrom;
                        sw.WriteLine(row);
                    }

                    MessageBox.Show("Export as CSV-File was successful.");
                    sw.Close();
                }
            }
            //Exeption message will be returned when error occured while generating csv.
            catch (Exception ex)
            {
                MessageBox.Show("Error occured while generating csv: " + ex.Message);
            }
        }

        public void ExportToXML(string filePath, List<Station> stationList, List<Timeseries> TimeseriesList)
        {
            TextWriter stationsWriter = null;
            try
            {
                List<StationExportModel> exportList = new List<StationExportModel>();
                for (int i = 0; i < stationList.Count; i++)
                {
                    exportList.Add(new StationExportModel()
                    {
                        agency = stationList[i].agency,
                        km = stationList[i].km,
                        latitude = stationList[i].latitude,
                        longitude = stationList[i].longitude,
                        longname = stationList[i].longname,
                        number = stationList[i].number,
                        shortname = stationList[i].shortname,
                        uuid = stationList[i].uuid,
                        water = stationList[i].water,
                        timeseries = TimeseriesList[i]
                    });
                }
                XmlSerializer stationsSerializer = new XmlSerializer(typeof(List<StationExportModel>));
                stationsWriter = new StreamWriter(filePath);
                stationsSerializer.Serialize(stationsWriter, exportList);

                MessageBox.Show("Stations Exported to XML Successfully.", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }

            //Exeption Message will be returned if error occured while creating the xml file.
            catch (Exception ex)
            {
                MessageBox.Show("Error durign export as XML-File: " + ex.Message);
            }

            finally
            {
                stationsWriter.Close();
            }
        }

        public void ImportFromCSV(string filePath, List<Station> stationList, List<Timeseries> TimeseriesList)
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var dataRow = reader.ReadLine();
                    var values = dataRow.Split(';');

                    if (values[0] != "id")
                    {
                        Station waterProperties = new Station
                        {
                            uuid = values[0],
                            number = values[1],
                            shortname = values[2],
                            longname = values[3],
                            km = Convert.ToDouble(values[4]),
                            agency = values[5],
                            longitude = Convert.ToDouble(values[6]),
                            latitude = Convert.ToDouble(values[7]),

                            water = new Water
                            {
                                longname = values[8],
                                shortname = values[9]
                            },
                        };
                        stationList.Add(waterProperties);

                        Timeseries timeseries = new Timeseries()
                        {
                            shortname = values[10],
                            longname = values[11],
                            unit = values[12],
                            equidistance = Convert.ToInt32(values[13]),
                            currentMeasurement = new CurrentMeasurement()
                            {
                                timestamp = Convert.ToDateTime(values[14]),
                                value = Convert.ToDouble(values[15]),
                                stateMnwMhw = values[16],
                                stateNswHsw = values[17],
                            },
                            gaugeZero = new GaugeZero()
                            {
                                unit = values[18],
                                value = Convert.ToDouble(values[19]),
                                validFrom = values[20]
                            }
                        };
                        TimeseriesList.Add(timeseries);
                    }
                }
            }

        }

        public void ImportFromXML(string filePath, out List<StationExportModel> stationExportModels)
        {
            try
            {
                //XmlReaderSettings stationsSettings = new XmlReaderSettings();

                //stationsSettings.Schemas.Add("", @"../../../XmlValidationSchema.xsd");
                //stationsSettings.ValidationType = ValidationType.Schema;
                //stationsSettings.ValidationEventHandler += StationsSettingsValidationEventHandler;

                XmlSchemaSet schema = new XmlSchemaSet();
                schema.Add("", @"../../../XmlValidationSchema.xsd");

                XmlReader xmlReader = XmlReader.Create(filePath);

                XDocument doc = XDocument.Load(xmlReader);
                doc.Validate(schema, StationsSettingsValidationEventHandler);
                xmlReader.Close();

                XmlSerializer serializer = new XmlSerializer(typeof(StationExportModel[]));
                using (TextReader reader = new StringReader(System.IO.File.ReadAllText(filePath)))
                {
                    var result = (StationExportModel[])serializer.Deserialize(reader);
                    stationExportModels = result.ToList();
                    
                }
                
            }
            catch (Exception ex)
            {
                stationExportModels = new List<StationExportModel>();
                MessageBox.Show("Import XML Error occured " + ex.Message, "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        static void StationsSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.Write("WARNING: ");
                Console.WriteLine(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
            }
        }

        public bool ValidateXML(string filePath)
        {

            XmlSchemaSet schema = new XmlSchemaSet();
            if (File.Exists("..\\..\\XMLValidation\\stations.xsd"))
            {
                schema.Add("", "..\\..\\XMLValidation\\stations.xsd");
            }
            string testXSD = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory)) + "stations.xsd";
            if (File.Exists(testXSD))
            {
                schema.Add("", testXSD);
            }
            XmlReader xmlReader = XmlReader.Create(filePath);

            try
            {
                XDocument doc = XDocument.Load(xmlReader);
                doc.Validate(schema, StationsSettingsValidationEventHandler);
                xmlReader.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validation Error");
                return false;
            }

        }


        public void Search(string searchText, List<Station> stationList, List<Timeseries> TimeseriesList, out List<Station> filteredList, out List<Timeseries> filteredTimeSeriesList)
        {
            filteredList = stationList.Where(a => a.uuid!.Contains(searchText) || a.water!.longname!.Contains(searchText) || a.water!.shortname!.Contains(searchText) || a.km.ToString().Contains(searchText) || a.longitude.ToString().Contains(searchText) || a.agency!.Contains(searchText) || a.latitude.ToString().Contains(searchText) || a.longname!.Contains(searchText) || a.number!.Contains(searchText) || a.shortname!.Contains(searchText)).ToList();

            filteredTimeSeriesList = TimeseriesList.Where(a => a.equidistance.ToString().Contains(searchText) || a.unit!.Contains(searchText) || a.longname!.Contains(searchText) || a.shortname!.Contains(searchText) || a.currentMeasurement!.value.ToString().Contains(searchText) || a.currentMeasurement.timestamp.ToString().Contains(searchText) || a.currentMeasurement!.stateNswHsw!.Contains(searchText) || a.currentMeasurement!.stateMnwMhw!.Contains(searchText) || a.gaugeZero!.unit!.Contains(searchText) || a.gaugeZero!.validFrom!.Contains(searchText) || a.gaugeZero.value.ToString().Contains(searchText)).ToList();

        }

        public void Sort(List<Station> stationList, out Station[] sortedArray)
        {
            sortedArray = stationList.ToArray();
            Array.Sort(sortedArray);
        }

    }
}
