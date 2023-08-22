using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for AddTimestamp.xaml
    /// </summary>
    public partial class AddTimestamp : Window
    {

        Station station;

        public AddTimestamp(Station station)
        {
            if (station == null)
            {
                MessageBox.Show("Keine gültige Station ausgewählt!");
                DialogResult = false;
                Close();
                return;
            }
            this.station = station;
            InitializeComponent();
        }

        private void AddStation(object sender, RoutedEventArgs e)
        {
            if(!AddStationLogic()) return;


            CloseWindow(sender, e);
        }

        public bool AddStationLogic()
        {
            if (string.IsNullOrEmpty(txb_value.Text) ||
                string.IsNullOrEmpty(txb_stateMnwMhw.Text) ||
                string.IsNullOrEmpty(txb_stateNswHsw.Text) ||
                string.IsNullOrEmpty(txb_timeseriesUnit.Text) ||
                string.IsNullOrEmpty(txb_shortname.Text) ||
                string.IsNullOrEmpty(txb_longname.Text) ||
                string.IsNullOrEmpty(txb_equidistance.Text))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus.");
                return false;
            }
            if (!double.TryParse(txb_value.Text, out _) ||
                !Int32.TryParse(txb_equidistance.Text, out _))
            {
                MessageBox.Show("Bitte geben Sie gültige Zahlenwerte in Messung und Equidistanz ein.");
                return false;
            }
            Comment? comment = null;
            if (!string.IsNullOrEmpty(txb_shortDescription.Text) || !string.IsNullOrEmpty(txb_longDescription.Text)) {
                comment = new Comment
                {
                    shortDescription = txb_shortDescription.Text,
                    longDescription = txb_longDescription.Text
                };
            }
            CurrentMeasurement measurement = new CurrentMeasurement
            {
                timestamp = DateTime.Parse(txb_timestamp.Text),
                value = Convert.ToDouble(txb_value.Text),
                stateMnwMhw = txb_stateMnwMhw.Text,
                stateNswHsw = txb_stateNswHsw.Text
            };
            GaugeZero? gaugeZero = null;
            if (!string.IsNullOrEmpty(txb_GaugeZeroUnit.Text) ||
                !string.IsNullOrEmpty(txb_GaugeZeroValidForm.Text) ||
                !string.IsNullOrEmpty(txb_GaugeZeroValue.Text))
            {
                if (double.TryParse(txb_GaugeZeroValue.Text, out _))
                {
                    gaugeZero = new GaugeZero
                    {
                        unit = txb_GaugeZeroUnit.Text,
                        validFrom = txb_GaugeZeroValidForm.Text,
                        value = Convert.ToDouble(txb_GaugeZeroValue.Text)
                    };
                }
                else
                {
                    MessageBox.Show("Bitte geben Sie gültige Zahlenwerte in Gauge Zero Messung ein.");
                    return false;
                }
            }

            Timeseries timeseries = new Timeseries
            {
                unit = txb_timeseriesUnit.Text,
                shortname = txb_shortname.Text,
                longname = txb_longname.Text,
                comment = comment,
                currentMeasurement = measurement,
                equidistance = Convert.ToInt32(txb_equidistance.Text),
                gaugeZero = gaugeZero
            };
            if (station.timeseries == null) station.timeseries = new List<Timeseries>();
            station.timeseries.Add(timeseries);
            return true;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
