using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace KanbanizeToolUi
{
    /// <summary>
    /// Interaction logic for SettingsEditorWindow.xaml
    /// </summary>
    public partial class SettingsEditorWindow : Window
    {
        public SettingsEditorWindow()
        {
            InitializeComponent();

            //propertyGrid.AdvancedOptionsMenu.ItemsSource = KanbanizeTool.Config.Utils.GetUserSettingsAsEnumerable();
            //var settings = KanbanizeTool.Config.Utils.GetUserSettingsAsEnumerable();
            //propertyGrid.SelectedObject = KanbanizeTool.Properties.Settings.Default;
            //SetUpPropertyGrid(propertyGrid, KanbanizeTool.Properties.Settings.Default);
            propertyGrid.SelectedObject = KanbanizeTool.Properties.Settings.Default;
            //SetUpPropertyGrid(propertyGrid, new KanbanizeTool.Properties.Settings());
        }

                   

        private void SetUpPropertyGrid(PropertyGrid pg, object selectedObject)
        {
            pg.AutoGenerateProperties = false;

            //var config = ConfigurationManager.OpenExeConfiguration("KanbanizeTool.exe");
            //ConfigurationManager.OpenMappedExeConfiguration()
            
            // iterate through selectedObject´s properties
            // and create a PropertyAttribute
            PropertyInfo[] properties = typeof(KanbanizeTool.Properties.Settings).GetProperties();
            var userProperties = properties.Where(property => Attribute.GetCustomAttribute(property, typeof(UserScopedSettingAttribute)) as UserScopedSettingAttribute != null);
            
            foreach (PropertyInfo pi in userProperties)
            {
                pg.PropertyDefinitions.Add(new PropertyDefinition {Name = pi.Name});
            }
            pg.SelectedObject = selectedObject;

            


            /*var list = new List<string>();
            list.Add("aaa");
            list.Add("bbb");
            foreach (var item in list)
            {
                pg.PropertyDefinitions.Add(new PropertyDefinition(){Name = item});
            }
            pg.SelectedObject = list;*/

            /*foreach (PropertyInfo propertyInfo in selectedObject)
            {
                pg.PropertyDefinitions.Add(new PropertyDefinition{Name = propertyInfo.Name});
            }
            pg.SelectedObject = selectedObject;*/

            //ptPdialog.PropertyControl.SelectedObject = list;
        }

        private bool IsBrowsable(PropertyInfo pi)
        {
            if (pi != null)
            {
                object[] attributes = pi.GetCustomAttributes(typeof(BrowsableAttribute), true);
                return (attributes.Length > 0) && ((BrowsableAttribute)attributes[0]).Browsable;
            }
            return false;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            DoSaveBindedBoxesHack();
            var settings = propertyGrid.SelectedObject as KanbanizeTool.Properties.Settings;
            if (settings != null)
            {
                settings.Save();
                Close();
            }
        }

        private void DoSaveBindedBoxesHack()
        {
            //var focusedItem = FocusManager.GetFocusedElement(this); // Test only
            FocusManager.SetFocusedElement(this, null);
        }
    }

    public class Person
    {
        [Browsable(true)]
        public string Name { get; set; }
        [Browsable(true)]
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
}
