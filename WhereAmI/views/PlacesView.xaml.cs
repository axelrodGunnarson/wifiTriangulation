﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhereAmI.models;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Collections;

namespace WhereAmI.views
{
    /// <summary>
    /// Interaction logic for PlacesView.xaml
    /// </summary>
    public partial class PlacesView : UserControl
    {
        Place selectedPlace;
        public PlacesView()
        {
            InitializeComponent();
            //Data binding after loading data
            App.loadedDataHandlers += (delegate()
            {
                // After the data is loaded call the DbSet<T>.Local property  
                // to use the DbSet<T> as a binding source. 
                placesViewData.ItemsSource = placesViewData.ItemsSource = DataManager.Instance.context.Places.Local;
            });
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            //placesViewData.ItemsSource = DataManager.Instance.places; 
            //To update the list Actions-Place.InActions when Actions is modified
            this.listActions.computeAvailablePlaces();
        }

        private void btnChangePlace_Click(object sender, RoutedEventArgs e)
        {
            string name=selectedPlace.Name;
            // Instantiate the dialog box
            EditDialogBox dlg = new EditDialogBox();

            // Configure the dialog box
            dlg.Owner = Window.GetWindow(this) as MainWindow;
            dlg.DataContext = selectedPlace;

            // Open the dialog box modally 
            dlg.ShowDialog();

            //var ctx = DataManager.Instance.context;
            if (dlg.DialogResult == false)
            {
                selectedPlace.Name = name;
            }
            DataManager.Instance.safeSave();
        }

        private void btnUpdateWifis_Click(object sender, RoutedEventArgs e)
        {
            // Display message box
            MessageBoxResult result = MessageBox.Show(
                Properties.Resources.UpdateWifiMsg,
                Properties.Resources.UpdateWifiTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                selectedPlace.Snapshot = Place.serializationSnapshot(DataManager.Instance.wifis.ToList());
                DataManager.Instance.safeSave();
            }
        }

        private void btnDeletePlace_Click(object sender, RoutedEventArgs e)
        {
            // Display message box
            MessageBoxResult result = MessageBox.Show(
                Properties.Resources.DeletePlace,
                Properties.Resources.DeletePlaceTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                //places.Remove(placesViewData.SelectedItem as Place);
                var ctx = DataManager.Instance.context;
                ctx.Places.Remove(selectedPlace);
                DataManager.Instance.safeSave();

                //To hide UI buttons
                placeViewDetail.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void btnResetStatPlace_Click(object sender, RoutedEventArgs e)
        {
            // Display message box
            MessageBoxResult result = MessageBox.Show(
                Properties.Resources.ResetCntPlace,
                Properties.Resources.ResetCntPlaceTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                selectedPlace.Cnt = 0;
                DataManager.Instance.safeSave();
            }
        }

        private void placesViewData_Selected(object sender, RoutedEventArgs e)
        {
            placeViewDetail.Visibility = System.Windows.Visibility.Visible;
            selectedPlace = placesViewData.SelectedItem as Place;
            this.listActions.selectedPlace = selectedPlace;
            this.listActions.computeAvailablePlaces();
        }
    }
}
