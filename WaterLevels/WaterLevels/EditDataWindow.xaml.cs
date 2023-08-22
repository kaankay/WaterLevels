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

namespace WaterLevels
{
    /// <summary>
    /// Interaction logic for EditDataWindow.xaml
    /// </summary>
    public partial class EditDataWindow : Window
    {
        public Station SelectedStation;
        public EditDataWindow(Station selectedItem)
        {
            InitializeComponent();
            var list = StationViewModel.Stations;
            dg_Stations.ItemsSource = list;
            SelectedStation = selectedItem;
            dg_Stations.SelectedItem = selectedItem;

        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dg_Stations.SelectedItem != null)
                {
                    var selectedStation = (Station)dg_Stations.SelectedItem;
                    var previousStation = StationViewModel.Stations!.Where(a => a.uuid == selectedStation.uuid).SingleOrDefault();
                    if (previousStation != null && selectedStation != null)
                    {
                        if (StationViewModel.StationChangesHistories == null)
                        {
                            StationViewModel.StationChangesHistories = new List<StationHistoryModel>();
                        }
                        StationViewModel.StationChangesHistories.Add(new StationHistoryModel()
                        {
                            Uuid = selectedStation.uuid!,
                            Number = selectedStation.number!,
                            Shortname = selectedStation.shortname!,
                            Longname = selectedStation.longname!,
                            Agency = selectedStation.agency!,
                            km = selectedStation.km,
                            Latitude = selectedStation.latitude,
                            Longitude = selectedStation.longitude,
                            UpdatedDateTime = DateTime.Now
                        });

                        previousStation.number = txtblock_Number.Text;
                        previousStation.shortname = txtblock_Shortname.Text;
                        previousStation.longname = txtblock_Longname.Text;
                        previousStation.km = Convert.ToDouble(txtblock_Km.Text);
                        previousStation.agency = txtblock_Agency.Text;
                        previousStation.latitude = Convert.ToDouble(txtblock_Lat.Text);
                        previousStation.longitude = Convert.ToDouble(txtblock_Lng.Text);

                        MessageBox.Show("Station Edited Successfully", "Edit Successfull", MessageBoxButton.OK, MessageBoxImage.Information);
                        dg_Stations.Items.Refresh();
                        dg_StationsHistory.ItemsSource = StationViewModel.StationChangesHistories != null ? StationViewModel.StationChangesHistories.Where(a => a.Uuid == selectedStation.uuid).ToList() : new List<StationHistoryModel>();
                        dg_StationsHistory.Items.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btn_Revert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dg_Stations.SelectedItem != null && dg_StationsHistory.SelectedItem != null)
                {
                    var selectedStation = (Station)dg_Stations.SelectedItem;
                    var selectedStationHistory = (StationHistoryModel)dg_StationsHistory.SelectedItem;
                    var selectedItemStation = StationViewModel.Stations!.Where(a => a.uuid == selectedStation.uuid).SingleOrDefault();
                    
                    selectedItemStation!.number = selectedStationHistory.Number;
                    selectedItemStation.shortname = selectedStationHistory.Shortname;
                    selectedItemStation.longname = selectedStationHistory.Longname;
                    selectedItemStation.km = selectedStationHistory.km;
                    selectedItemStation.agency = selectedStationHistory.Agency;
                    selectedItemStation.latitude = selectedStationHistory.Latitude;
                    selectedItemStation.longitude = selectedStationHistory.Longitude;

                    MessageBox.Show("Station Status Reverted Successfully", "Revert Successfull", MessageBoxButton.OK, MessageBoxImage.Information);
                    dg_Stations.Items.Refresh();
                    dg_StationsHistory.Items.Refresh();
                    txtblock_Number.Text = selectedStationHistory.Number;
                    txtblock_Lat.Text = selectedStationHistory.Latitude.ToString();
                    txtblock_Lng.Text = selectedStationHistory.Longitude.ToString();
                    txtblock_Km.Text = selectedStationHistory.km.ToString();
                    txtblock_Agency.Text = selectedStationHistory.Agency;
                    txtblock_Shortname.Text = selectedStationHistory.Shortname;
                    txtblock_Longname.Text = selectedStationHistory.Longname;
                }
                else
                {
                    MessageBox.Show("Please select history status to revert changes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dg_Stations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedStation = (Station)dg_Stations.SelectedItem;
                if (selectedStation != null)
                {
                    dg_StationsHistory.ItemsSource = StationViewModel.StationChangesHistories != null ? StationViewModel.StationChangesHistories.Where(a => a.Uuid == selectedStation.uuid).ToList() : new List<StationHistoryModel>();
                    txtblock_Number.Text = selectedStation.number;
                    txtblock_Lat.Text = selectedStation.latitude.ToString();
                    txtblock_Lng.Text = selectedStation.longitude.ToString();
                    txtblock_Km.Text = selectedStation.km.ToString();
                    txtblock_Agency.Text = selectedStation.agency;
                    txtblock_Shortname.Text = selectedStation.shortname;
                    txtblock_Longname.Text = selectedStation.longname;
                }
                else
                {
                    txtblock_Number.Text = String.Empty;
                    txtblock_Lat.Text = String.Empty;
                    txtblock_Lng.Text = String.Empty;
                    txtblock_Km.Text = String.Empty;
                    txtblock_Agency.Text = String.Empty;
                    txtblock_Shortname.Text = String.Empty;
                    txtblock_Longname.Text = String.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CheckTextBoxes() //check if all textboxes are filled
        {
            if (txtblock_Agency.Text != "" && txtblock_Km.Text != "" && txtblock_Lat.Text != "" && txtblock_Lng.Text != "" && txtblock_Longname.Text != ""
                && txtblock_Number.Text != "" && txtblock_Shortname.Text != "")
            {
                return true;
            }

            else return false;

        }
    }
}
