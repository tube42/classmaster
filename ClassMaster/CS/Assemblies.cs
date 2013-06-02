using System;

using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

using Microsoft.Phone;
using Microsoft.Phone.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Reflection;




namespace ClassMaster {
public class AssemblyEntry : INotifyPropertyChanged
{
    
    // public static Brush BrushDisabled = new SolidColorBrush(Colors.Black);
    // public static Brush BrushEnabled = new SolidColorBrush(Colors.Red);
    
    public static Brush BrushDisabled = (Brush)App.Current.Resources["PhoneBackgroundBrush"];
    public static Brush BrushEnabled = (Brush) App.Current.Resources["PhoneAccentBrush"];
    
    
    
    private bool enabled;
    public event PropertyChangedEventHandler PropertyChanged;
    
    
    public String Name { get; private set; }
    public bool Enabled
    {
        get { return enabled; }
        set 
        {
            if (enabled != value) {
                enabled = value;
                
                if (PropertyChanged != null) {                        
                    PropertyChanged(this, new PropertyChangedEventArgs("Enabled") );
                    PropertyChanged(this, new PropertyChangedEventArgs("Color"));
                }
            }
        }
    }
    
    
    public AssemblyEntry(String name, bool enabled)
    {
        Name = name;
        this.enabled = enabled;
    }
    
    public Brush Color { get { return enabled ? BrushEnabled : BrushDisabled; } }        
}


// a name space and its contents
public class NamespaceData
{
    public String name;
    public List<NamespaceData> namespaces;
    public List<Type> classes;

    public NamespaceData(String name)
    {
        this.name = name;
        this.namespaces = new List<NamespaceData>();
        this.classes = new List<Type>();        
    }
    
    public List<NamespaceEntry> CreateEntries(bool include_inner_classess)
    {
        List<NamespaceEntry> list = new List<NamespaceEntry>();
        foreach(NamespaceData nd in namespaces) 
            list.Add( new NamespaceEntry(nd));
        
        
        foreach(Type t in classes) 
            if(include_inner_classess || !t.IsNested)
                list.Add( new NamespaceEntry(t));

        return list;
    }

    // ------------------------------------------
    public int CountNamespaces()
    {
        return namespaces.Count;
    }

    public int CountClasses(bool include_inner_classess)
    {
        if (include_inner_classess) return classes.Count;
        else {
            int ret = 0;
            foreach (Type t in classes)
                if (!t.IsNested)
                    ret++;
            return ret;
        }
    }

    // ------------------------------------------
    
    private NamespaceData FindNamespaceFromClassName(String [] names, int offset, int count)
    {
        if (offset >= count) return this;
        foreach(NamespaceData nd in namespaces) {
            if (nd.name == names[offset])
                return nd.FindNamespaceFromClassName(names, offset + 1, count);
        }

        NamespaceData n2 = new NamespaceData(names[offset]);
        namespaces.Add(n2);
        return n2.FindNamespaceFromClassName(names, offset + 1, count);
    }
    
    public NamespaceData FindNamespaceFromClassName(String name)
    {  
        String [] ss = name.Split(' ', '.' /* , '+' */ );
        return FindNamespaceFromClassName(ss, 0, ss.Length-1);
    }
    
    
    // ------------------------------------

    private Type Find(String [] names, int offset, int count)
    {
        if(count < 0 || offset > count) return null;
        
        String me = names[offset];
        
        if (offset >= count) {
            foreach (Type t in classes) {
                String name = Utils.GetCompleteName(t);
                if (name == me)
                    return t;
            }
        } else {            
            foreach(NamespaceData nd in namespaces)
                  if (nd.name == me)
                      return nd.Find(names, offset + 1, count);
        }
            
        return null; // not found
    }
    
    public Type Find(String name)
    {
        String [] ss = name.Split(' ', '.' /*, '+' */);
        return Find(ss, 0, ss.Length-1);
    }

    // ------------------------------------------
    private void SearchForName(List<NamespaceEntry> result, String name)
    {
        foreach (Type t in classes)
            if (!t.IsNested && t.Name.ToLower().Contains(name))
                result.Add(new NamespaceEntry(t));

        foreach (NamespaceData nd in namespaces)
            nd.SearchForName(result, name);
    }

    public List<NamespaceEntry> SearchForName(String name)
    {
        List<NamespaceEntry> ret = new List<NamespaceEntry>();
        if (name.Length > 2) SearchForName(ret, name.ToLower());
        return ret;
    }


    // ----------------------------------
    private int CompareNamespaceData(NamespaceData n1, NamespaceData n2)
    {
        return String.Compare(n1.name, n2.name);
    }
    private int CompareType(Type t1, Type t2)
    {
        return String.Compare(t1.Name, t2.Name);
    }
    public void Sort()
    {
        foreach (NamespaceData nd in namespaces)
            nd.Sort();

        namespaces.Sort(CompareNamespaceData);
        classes.Sort(CompareType);
    }
}

public class AssemblyList
{
    private List<AssemblyEntry> entries = null;
    private NamespaceData root = null;
    
    public void Update()
    {
        // this forces root to be updated next time
        root = null;
    }
    
    public List<AssemblyEntry> GetAssmblies()
    {
        if (entries == null) create_assembly_entries();
        return entries;
    }
    
    public NamespaceData GetRoot()
    {
        if(root == null) create_root();
        return root;        
    }
    // --------------------------------------------
    private void create_assembly_entries()
    {
        
        entries = new List<AssemblyEntry>();        
        entries.Add(new AssemblyEntry("Microsoft.Phone", true));
        entries.Add(new AssemblyEntry("Microsoft.Phone.Interop", true));
        entries.Add(new AssemblyEntry("Microsoft.Phone.Controls", true));
        entries.Add(new AssemblyEntry("Microsoft.Phone.Controls.Map", false));
        entries.Add(new AssemblyEntry("Microsoft.Devices.Sensors", false));
        entries.Add(new AssemblyEntry("mscorlib", true));
        entries.Add(new AssemblyEntry("mscorlib.Extensions", true));
        entries.Add(new AssemblyEntry("system", true));
        entries.Add(new AssemblyEntry("System.Core", true));
        entries.Add(new AssemblyEntry("System.Device", false));
        entries.Add(new AssemblyEntry("System.Net", false));
        entries.Add(new AssemblyEntry("System.Windows", true));
        entries.Add(new AssemblyEntry("System.Xml", false));
#if false
        entries.Add(new AssemblyEntry("Microsoft.Xna.FrameWork", false));
        entries.Add(new AssemblyEntry("Microsoft.Xna.FrameWork.Game", false));
        entries.Add(new AssemblyEntry("Microsoft.Xna.FrameWork.GameServices", false));
        entries.Add(new AssemblyEntry("Microsoft.Xna.FrameWork.Graphics", false));
        entries.Add(new AssemblyEntry("Microsoft.Xna.FrameWork.Inout", false));

        entries.Add(new AssemblyEntry("Microsoft.Advertising.Mobile", false));
#endif
    }
    
    private void create_root()
    {
        // 1. load assemblies
        List<AssemblyEntry> entries = GetAssmblies();
        List<Assembly> loaded = new List<Assembly>();
        
        foreach(AssemblyEntry ae in entries) {
            try {
                if(ae.Enabled)
                    loaded.Add( Assembly.Load(ae.Name));
            } catch { }
        }
        
        // 2. load classes
        List<Type> classes = new List<Type>();                
        foreach(Assembly asm in loaded) {
            Type [] types = asm.GetTypes();
            foreach (Type t in types) {
                if (!t.IsNotPublic) {
                    if (t.IsClass || t.IsInterface || t.IsEnum)
                        classes.Add(t);
                }
            }
        }
        
        // 3. create namespace root
        root = new NamespaceData("");
        
        foreach(Type type in classes) {
            String name = type.FullName;            
            NamespaceData pos = root.FindNamespaceFromClassName(name);
            pos.classes.Add(type);

        }

        // 4. and sort it!
        root.Sort();

    }

    // -------------------------------------------------------------------
    public void Save()
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        List<AssemblyEntry> list = GetAssmblies();
        
        foreach (AssemblyEntry a in list) {
            String name = "Settings.AssemblyList." + a.Name;
            settings[name] = a.Enabled;
        }
        settings.Save();
    }
    
    public void Load()
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        List<AssemblyEntry> list = GetAssmblies();
        
        foreach (AssemblyEntry a in list) {
            String name = "Settings.AssemblyList." + a.Name;
            bool enabled = true;
            if (settings.TryGetValue<bool>(name, out enabled))
                a.Enabled = enabled;
        }
    }
    
    
    
    
}
}
