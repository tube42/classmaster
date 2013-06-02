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
    public partial class SearchPage : PhoneApplicationPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }


        private void on_tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Object obj = list.SelectedItem;

            if (obj != null && obj is NamespaceEntry) {
                NamespaceEntry ne = (NamespaceEntry)obj;
                if (ne.is_namespace) {
                    /* TODO
                    stack.Push(ne.namespace_);
                    build();
                     * */
                } else {
                    Uri uri = UiHelper.GetPageForType(ne.klass);
                    if (uri != null) NavigationService.Navigate(uri);

                    /*
                    String name = Utils.UrlEncode(ne.klass.FullName);
                    if (ne.klass.IsEnum)
                        NavigationService.Navigate(new Uri("/XAML/EnumPage.xaml?Enum=" + name, UriKind.Relative));
                    else
                        NavigationService.Navigate(new Uri("/XAML/ClassPage.xaml?Class=" + name, UriKind.Relative));
                     */
                }
            }
            e.Handled = true;
        }

        private void on_text_changed(object sender, TextChangedEventArgs e)
        {
            String new_text = this.searchBox.Text;
            
            if (new_text.Length > 3) {
                App app = (App)App.Current;
                List<NamespaceEntry> nes = app.List.GetRoot().SearchForName(new_text);

                // set namespace name as description:
                foreach (NamespaceEntry ne in nes) 
                    ne.Desc = ne.klass.Namespace;               
                
                list.ItemsSource = nes;
            } else {
                list.ItemsSource = null;
            }
            

        }

    }
}