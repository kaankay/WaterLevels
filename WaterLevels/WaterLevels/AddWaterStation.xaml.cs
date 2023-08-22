using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace WaterLevels
{
    // <summary>
    // Interaction logic for AddWaterStation.xaml
    // </summary>
    public partial class AddWaterStation : Window
    {
        private MainWindow Main;
        public AddWaterStation(MainWindow main)
        {
            Main = main;
            InitializeComponent();
            
        } 

        private void AddStation(object sender, RoutedEventArgs e)
        {
            if (!AddStationLogic()) return;

            CloseWindow(sender, e);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public bool AddStationLogic()
        {
            if (string.IsNullOrEmpty(txb_uuid.Text) ||
                string.IsNullOrEmpty(txb_number.Text) ||
                string.IsNullOrEmpty(txb_shortname.Text) ||
                string.IsNullOrEmpty(txb_longname.Text) ||
                string.IsNullOrEmpty(txb_km.Text) ||
                string.IsNullOrEmpty(txb_agency.Text) ||
                string.IsNullOrEmpty(txb_longitude.Text) ||
                string.IsNullOrEmpty(txb_latitude.Text) ||
                string.IsNullOrEmpty(txb_waterLongname.Text) ||
                string.IsNullOrEmpty(txb_waterShortname.Text))
            {
                MessageBox.Show("Bitte füllen Sie alle felder aus.");
                return false;
            }
            else if (!double.TryParse(txb_km.Text, out _) ||
                      !double.TryParse(txb_latitude.Text, out _) ||
                      !double.TryParse(txb_longitude.Text, out _) ||
                      !Int32.TryParse(txb_number.Text, out _))
            {
                MessageBox.Show("Bitte tragen sie nur Zahlen in die Felder KM, Longitude und Latitude ein.");
                return false;
            }
            Station station = new Station
            {
                uuid = txb_uuid.Text,
                number = txb_number.Text,
                shortname = txb_shortname.Text,
                longname = txb_longname.Text,
                km = Convert.ToDouble(txb_km.Text),
                agency = txb_agency.Text,
                longitude = Convert.ToDouble(txb_longitude.Text),
                latitude = Convert.ToDouble(txb_latitude.Text),

                water = new Water
                {
                    longname = txb_waterLongname.Text,
                    shortname = txb_waterShortname.Text

                },
                timeseries = new List<Timeseries>()
            };

            Main.stationList.Add(station);
            return true;
        }

    }
}
