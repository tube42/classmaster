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
    public class ClassItemEntry
    {
        public String Title { get; set; }
        public String Text { get; set; }

        public ClassItemEntry(String title, String text) 
        { 
            Title = title == null ? "???" : title; 
            Text = text == null ? "???" : text; 
        }
    }

    public class HierarchyItemEntry
    {
        public Type Type { get; set; }
        public String ClassName { get; set; }
        public String Namespace { get; set; }

        public HierarchyItemEntry(Type type)
        {
            Type = type;
            ClassName = Utils.GetCompleteName(type);
            Namespace = type.Namespace;
        }
    }

    // --------------------------------
    public class ClassContentEntry 
    {
        public enum Type { FUNCTION, MEMBER, EVENT, PROPERTY };
        public Type Typ { get; private set; }
        public String Name { get; private set; }
        public String DescTop { get; private set; }
        public String DescBottom { get; private set; }
        public Object Obj { get; private set; }

        public String TypeLetter { get; private set; }

        public ClassContentEntry(MemberInfo member)
        {
            Obj = member;
            Typ = Type.MEMBER;
            Name = String.Format("{0}", member.Name);
            TypeLetter = "m_";
            DescTop = get_simple_name( member.DeclaringType); 
        }

        public ClassContentEntry(MethodInfo method)
        {
            Obj = method;
            Typ = Type.FUNCTION;
            
            TypeLetter = "f()";
            Name = get_simple_name( method.Name);

            // access:
            String tmp = "";
            if (method.IsPublic) tmp += "public ";
            if (method.IsPrivate) tmp += "private ";
            if (method.IsStatic ) tmp += "static ";
            if (method.IsFinal) tmp += "final ";
            if (method.IsVirtual) tmp += "virtual ";
            if (method.IsAbstract) tmp += "abstract";

            // type
            tmp += get_simple_name(method.ReturnType);
            DescTop = tmp;
            
            // parameters
            DescBottom = get_parameters_text(method.GetParameters());
        }

        public ClassContentEntry(ConstructorInfo method)
        {
            Obj = method;
            Typ = Type.FUNCTION;            
            TypeLetter = ".ctor";
            Name = get_simple_name(method.DeclaringType.Name);

            // access:
            String tmp = "";
            if (method.IsPublic) tmp += "public ";
            if (method.IsPrivate) tmp += "private ";
            if (method.IsStatic) tmp += "static ";
            if (method.IsFinal) tmp += "final ";
            if (method.IsVirtual) tmp += "virtual ";
            if (method.IsAbstract) tmp += "abstract";

            // type
            DescTop = tmp;

            // parameters
            DescBottom = get_parameters_text(method.GetParameters());
        }




        public ClassContentEntry(PropertyInfo prop)
        {
            Obj = prop;
            Typ = Type.PROPERTY;
            TypeLetter = "Pr";
            Name = get_simple_name(prop.Name);

            // access:
            
            DescTop = String.Format("{0}", get_simple_name(prop.PropertyType));

            String tmp = "";
            if(prop.CanRead) tmp += "get ";
            if(prop.CanWrite) tmp += "set ";
            DescBottom = tmp;
        }

        public ClassContentEntry(EventInfo event_)
        {
            Obj = event_;
            Typ = Type.EVENT;
            TypeLetter = "Ev";
            Name = String.Format("{0}", event_.Name);

            DescTop = get_simple_name(event_.DeclaringType);
        }

        // ---------------------------------------
        private String get_simple_name(Object obj)
        {
            String fullname = obj.ToString();
            String [] sliced = fullname.Split(' ', '.' /*, '+' */ );
            if (sliced.Length == 0) return "?";

            String ret = sliced[sliced.Length - 1];
            // ret = ret.Replace("]", "[]");
            ret = ret.Replace("'1", "<>");
            ret = ret.Replace("'2", "<,>");
            ret = ret.Replace("'3", "<,,>");
            ret = ret.Replace("'4", "<,,,>");
            ret = ret.Replace("'5", "<,,,,>");
            ret = ret.Replace("'6", "<,,,,,>");
            return ret;
        }
        private String get_parameters_text(ParameterInfo[] pi)
        {
            String ret = "" ;
            for (int i = 0; i < pi.Length; i++) 
            {
                if (i != 0) ret = ret + ", ";
                ret = ret + get_simple_name(pi[i].ParameterType.Name) + " " + pi[i].Name;
                
            }
            return "(" + ret + ")";
        }
    }



    // -------------------------------------------------
public partial class ClassPage : PhoneApplicationPage
{
    private Type type = null;
    

    public ClassPage()
    {
        InitializeComponent();        
    }
    
    protected override void OnNavigatedTo(NavigationEventArgs args)
    {
        base.OnNavigatedTo(args);
        
        if (NavigationContext.QueryString.ContainsKey("Class")) {
            String name = NavigationContext.QueryString["Class"];
            Type t = UiHelper.LoadTypeFromName(name);

            if(t != null) {
                type = t;
                Update();
            } else if(type == null) {
                MessageBox.Show("Unable to load class " + name + "!", "Error", MessageBoxButton.OK);
                NavigationService.GoBack();
            }                           
        }
        
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
        ClassList.ItemsSource = classitems;

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


        // get inner classes
        List<HierarchyItemEntry> ics = new List<HierarchyItemEntry>();
        Type[] icss = type.GetNestedTypes(BindingFlags.Public);
        for (int i = 0; i < icss.Length; i++)
            ics.Add(new HierarchyItemEntry(icss[i]));
        InnerClassList.ItemsSource = ics;



        // get properties
        List<ClassContentEntry> ps = new List<ClassContentEntry>();
        foreach (PropertyInfo p in type.GetProperties())             
            ps.Add(new ClassContentEntry(p));
        propertyList.ItemsSource = ps;

        // constructors
        List<ClassContentEntry> cil = new List<ClassContentEntry>();        
        foreach(ConstructorInfo ci in type.GetConstructors() )
            cil.Add(new ClassContentEntry(ci));
        constructorList.ItemsSource = cil;

        // get functions
        List<ClassContentEntry> ms = new List<ClassContentEntry>();
        foreach (MethodInfo mi in type.GetMethods()) 
            if(! mi.IsSpecialName)
                ms.Add(new ClassContentEntry(mi));
        functionList.ItemsSource = ms;

        // get members
        List<ClassContentEntry> m2s = new List<ClassContentEntry>();
        foreach (MemberInfo mi in type.GetMembers()) 
            if( (mi.MemberType & (MemberTypes.Field | MemberTypes.NestedType)) != 0)
                m2s.Add(new ClassContentEntry(mi));
        memberList.ItemsSource = m2s;

        // get events
        List<ClassContentEntry> es = new List<ClassContentEntry>();
        foreach (EventInfo ei in type.GetEvents()) es.Add(new ClassContentEntry(ei));
        eventList.ItemsSource = es;

    }
    
    // ----------------------------------------------------
    public void on_inner_class(Object sender, EventArgs args)
    {
        show_class(InnerClassList.SelectedItem);
    }

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

            if (uri != null)
                NavigationService.Navigate(uri);
            else
                // let user know ?
                MessageBox.Show("Class is not available (is the assembly excluded?)");
        }
    }
}

}