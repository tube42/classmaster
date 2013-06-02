using System;
using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Text;

namespace ClassMaster
{

    public class NamespaceEntry
    {
        public bool is_namespace;
        public NamespaceData namespace_;
        public Type klass;

        // getters for XAML bindings :(
        private String name, type_letter, desc, display_name;

        public String Name { get { return name; } }
        public String DisplayName { get { return display_name; } }
        public String TypeLetter { get { return type_letter; } }
        public String Desc { get { return desc; } set { desc = value; }  }

        public NamespaceEntry(NamespaceData nd)
        {
            this.is_namespace = true;
            this.namespace_ = nd;
            this.klass = null;

            this.name = namespace_.name;
            this.display_name = this.name;
            this.type_letter = "N";


            // 
            int namespaces = namespace_.CountNamespaces();
            int classes = namespace_.CountClasses(true);
            int outer_classes = namespace_.CountClasses(false) ;
            int inner_classes = classes - outer_classes;

            this.desc = String.Format("{0} namespaces, {1} classes", namespaces, outer_classes);

            if(inner_classes != 0 ) 
                this.desc += String.Format(", {0} inner classes", inner_classes);            
                
        }

        public NamespaceEntry(Type t)
        {
            this.is_namespace = false;
            this.namespace_ = null;
            this.klass = t;

            this.name = Utils.GetCompleteName(klass); //  klass.Name;
            this.display_name = this.name.Replace("+", ".");

            if (t.IsEnum) type_letter = "En";
            else if (t.IsInterface) type_letter = "I";
            // else if (t.IsNested) type_letter = "Nes";
            else if (name.Contains("+")) type_letter = "I.C";
            else type_letter = "C";

            this.desc = String.Format("{0} members, {1} methods", klass.GetMembers().Length, klass.GetMembers().Length);
        }

    }


    public partial class ListPage : PhoneApplicationPage
    {

        private Stack<NamespaceData> stack;
        public ListPage()
        {
            InitializeComponent();

            
            App app = (App)App.Current;
            stack = new Stack<NamespaceData>();
            stack.Push(app.List.GetRoot());

            build();
        }

        private void build()
        {
            // set list data
            NamespaceData data = stack.Peek();
            list.ItemsSource = data.CreateEntries(false);            
            
            // build path name
            String name = null;
            foreach(NamespaceData d in stack) {
                if(d.name.Length == 0) continue; // nothing to add (first one)
                if(name == null) name = d.name;
                else name = d.name + "." + name;
            }
            
            PathName.Text = (name == null) ? " " : name;
        }
        
        
        private void on_tap(object sender, System.Windows.Input.GestureEventArgs e)
        {            
            Object obj = list.SelectedItem;
            
            if (obj != null && obj is NamespaceEntry) {
                NamespaceEntry ne = (NamespaceEntry) obj;
                if(ne.is_namespace) {
                    stack.Push(ne.namespace_);
                    build();
                } else {
                    Uri uri = UiHelper.GetPageForType(ne.klass);
                    if (uri != null) NavigationService.Navigate(uri);
                }
            }                        
            e.Handled = true;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (stack.Count <= 1) {
                base.OnBackKeyPress(e);
            } else {
                stack.Pop();
                build();
                e.Cancel = true;
            }
        }        
    }
}