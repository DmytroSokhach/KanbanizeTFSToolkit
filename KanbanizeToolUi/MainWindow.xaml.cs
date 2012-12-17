using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KanbanizeToolUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand MenuRoutedCommand = new RoutedCommand();

        // TODO: finish these WPF routed commands
        private void ExecutedMainMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            if (null != item)
            {
                switch ((string)e.Parameter)
                {
                    case ("File_Exit"):
                        Application.Current.Shutdown();
                        break;
                    case ("Options_Edit"):
                        EditOptions();
                        break;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            //ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming)

            /*CommandBinding customCommandBinding = new CommandBinding(MenuRoutedCommand, ExecutedMainMenuCommand, CanExecuteCustomCommand);*/

           /* KeyGesture CloseCmdKeyGesture = new KeyGesture(Key.X, ModifierKeys.Control);
            MenuRoutedCommand.InputGestures.Add(CloseCmdKeyGesture);
            CloseCmdKeyGesture = new KeyGesture(Key.O, ModifierKeys.Control);
            MenuRoutedCommand.InputGestures.Add(CloseCmdKeyGesture);*/
        }



        private void EditOptions()
        {
            var result = new SettingsEditorWindow().ShowDialog();

        }


        public void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Control target = e.Source as Control;

            if (target == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = true;
        }
    }
}
