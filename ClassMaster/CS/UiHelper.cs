using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ClassMaster
{
    public static class UiHelper
    {

        public static Type LoadTypeFromName(String name)
        {
            App app = (App)App.Current;
            return app.List.GetRoot().Find(name);
        }

        public static Uri GetPageForType(Type t)
        {
            // lets see if we can load it oureselves:
            String name = t.FullName;

            Type t2 = LoadTypeFromName(name);
            if (t2 != null) {
                name = Utils.UrlEncode(name);
                if(t.IsEnum)
                    return new Uri("/XAML/EnumPage.xaml?Enum=" + name, UriKind.Relative);
                else
                    return new Uri("/XAML/ClassPage.xaml?Class=" + name, UriKind.Relative);
            } else {
                return null;                
            }
        }
    }
}
