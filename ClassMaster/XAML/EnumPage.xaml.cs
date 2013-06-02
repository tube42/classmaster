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
using System.Windows.Navigation;
using System.Reflection;

namespace ClassMaster
{
    public partial class EnumPage : PhoneApplicationPage
    {
        private Type type = null;
        public EnumPage()
        {
            InitializeComponent();
        }




    protected override void OnNavigatedTo(NavigationEventArgs args)
    {
        base.OnNavigatedTo(args);
        
        if (NavigationContext.QueryString.ContainsKey("Enum")) {
            String name = NavigationContext.QueryString["Enum"];
            Type t = UiHelper.LoadTypeFromName(name);

            if(t != null) {
                type = t;
                Update();
            } else if(type == null) {
                MessageBox.Show("Unable to load enum " + name + "!", "Error", MessageBoxButton.OK);
                NavigationService.GoBack();
            }                           
        }        
    }

    private Type LoadTypeFromName(String name)
    {
        App app = (App)App.Current;
        return app.List.GetRoot().Find(name);
    }
    
    
    private void Update()
    {
        if (type == null) return;

        // set pivot title
        PageTitle.Title = type.Name;


        // class description:
        List<ClassItemEntry> classitems = new List<ClassItemEntry>();
        classitems.Add(new ClassItemEntry("Name", Utils.GetCompleteName(type)));
        classitems.Add(new ClassItemEntry("Namespace", type.Namespace));
        classitems.Add(new ClassItemEntry("Module", type.Module.ToString()));
        if(type.BaseType != null) 
            classitems.Add(new ClassItemEntry("Parent", type.BaseType.ToString()));
        classitems.Add(new ClassItemEntry("Attributes", type.Attributes.ToString()));

        if (type.IsGenericParameter) {
            classitems.Add(new ClassItemEntry("Declaring type", type.DeclaringType.ToString()));
            classitems.Add(new ClassItemEntry("Declaring method", type.DeclaringMethod.ToString()));
        }
        classitems.Add(new ClassItemEntry("Member type", type.MemberType.ToString()));        
        classitems.Add(new ClassItemEntry("Qualified name", type.AssemblyQualifiedName.ToString()));
        EnumList.ItemsSource = classitems;

        // hierarchy:
        List<HierarchyItemEntry> hr = new List<HierarchyItemEntry>();
        Type[] ints = type.GetInterfaces();
        for(int i = 0; i < ints.Length; i++)
            hr.Add(new HierarchyItemEntry(ints[i] ));
        for(Type tmp = type; tmp.BaseType != null; tmp = tmp.BaseType)             
            if(tmp != type)
                hr.Add( new HierarchyItemEntry(tmp) );
        hr.Add(new HierarchyItemEntry(typeof(System.Object)));
        hr.Reverse(); // from top to bottom?
        HierarchyList.ItemsSource = hr;

        // get members
        List<ClassContentEntry> m2s = new List<ClassContentEntry>();
        foreach (MemberInfo mi in type.GetMembers()) 
            if( (mi.MemberType & (MemberTypes.Field | MemberTypes.NestedType)) != 0)
                m2s.Add(new ClassContentEntry(mi));
        ValueList.ItemsSource = m2s;


    }
    
    // ----------------------------------------------------
    
    public void on_hierarchy(Object sender, EventArgs args)
    {
        show_class(HierarchyList.SelectedItem);
    }

    private void show_class(Object obj)
    {
        if (obj != null && obj is HierarchyItemEntry) {
            HierarchyItemEntry hie = (HierarchyItemEntry)obj;
            if (hie.Type == type) return; // tapping on myself
            Uri uri = UiHelper.GetPageForType(hie.Type);

            if(uri != null)
                NavigationService.Navigate( uri);
            else
                // let user know ?
                MessageBox.Show("Class or enum is not available (is the assembly excluded?)");            
        }
    }
}


}