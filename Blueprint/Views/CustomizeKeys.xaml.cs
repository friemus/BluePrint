using Blueprint.ViewModels;
using SubCTools.Extras.Settings;
using SubCTools.Settings;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Blueprint.Views
{
    /// <summary>
    /// Interaction logic for CustomizeKeys.xaml
    /// </summary>
    public partial class CustomizeKeys : UserControl
    {
        public SavableObject ActiveOwner { get; private set; }

        public CustomizeKeys()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            var settings = new SubCXDoc("Settings.xml");

            var tabVM1 = new RecordingVM(1, settings);

            var tabVM2 = new RecordingVM(2, settings);

            keysVM = new KeysVM(settings, tabVM1, tabVM2);

            tab1.DataContext = tabVM1;
            tab2.DataContext = tabVM2;
            listbx.DataContext = keysVM;
            DataContext = keysVM;
            //var RecordingVM = new RecordingVM ("ID");

            //this.DataContext = RecordingVM;


        }

        public IEnumerable<KeyValuePair<string, string>> KeysDictionary { get; private set; }

        private KeysVM keysVM;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //KeyDown += KeysVM.KeyPressed;
        }
        public void KeyPressed(string key)
        {
            keysVM.ExecuteKey(key);
        }

        private void OnTabSelected(object sender, SelectionChangedEventArgs e)
        {
            //TabItem selectedTab = e.AddedItems[0] as TabItem;

            //if (selectedTab.Name == "tab1" | selectedTab.Name == "tab2")
            //{
            //    HotKey activeOwner = new HotKey()
            //    {
            //        Owner = ActiveOwner
            //    };
            //}

            //else if (selectedTab.Name == null)
            //{
            //    Console.WriteLine("there is no TabItem");
            //}


        }



    }
}