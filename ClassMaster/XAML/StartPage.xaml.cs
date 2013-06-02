using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace ClassMaster
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
                        
        }
        
        private void on_select(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/XAML/AssemblySelectionPage.xaml", UriKind.Relative));
        }

        private void on_browse(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/XAML/BrowsePage.xaml", UriKind.Relative));
        }
        
        private void on_search(object sender, RoutedEventArgs e)
        {            
            NavigationService.Navigate(new Uri("/XAML/SearchPage.xaml", UriKind.Relative));
        }
        
    }
}