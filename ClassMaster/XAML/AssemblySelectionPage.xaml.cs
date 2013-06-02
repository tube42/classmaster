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

public partial class AssemblySelectionPage : PhoneApplicationPage
{
    
    public AssemblySelectionPage()
    {         
        InitializeComponent();
        
        update_list();
    }
    
    
    private void update_list()
    {
        App app = (App)App.Current;
        list.ItemsSource = app.List.GetAssmblies();
    }   
    
    
    private void on_tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
        Object obj = list.SelectedItem;
        if (obj != null && obj is AssemblyEntry) {
            AssemblyEntry ae = (AssemblyEntry)obj;
            ae.Enabled = !ae.Enabled;
        }

        e.Handled = true;        
    }

    protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
    {
        // force list to be updated
        App app = (App)App.Current;
        app.List.Update();

        // And save it!
        app.List.Save();

        base.OnNavigatedFrom(e);
    }
    
}
}