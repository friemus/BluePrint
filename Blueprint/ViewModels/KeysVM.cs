using GalaSoft.MvvmLight.Command;
using SubCTools.Extras;
using SubCTools.Settings;
using SubCTools.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Threading.Tasks;
using SubCTools.Extras.Settings;
using SubCTools.Helpers;
using SubCTools.Messaging.Models;
using System.Windows.Controls;

namespace Blueprint.ViewModels
{

    //DO NOT USE WITH PARAMETERIZED RELAYCOMMANDS
    public class KeysAttribute : Attribute
    {

    }

  
    public class RecordingVM : SavableObject
    {
        private int IDs;
        public int IDss { get; private set; }

        [Keys]
        public RecordingVM(int ID, ISettingsService settings) : base(settings, id:ID.ToString())
        {
            IDs  = ID;
        }

        private RelayCommand startRecordingHotkeyCmd;

        [Keys]
        public RelayCommand StartRecordingHotkeyCmd => startRecordingHotkeyCmd ?? (startRecordingHotkeyCmd = new RelayCommand(() => {
            Console.WriteLine(IDs+ " " + " Hello" );
        }));

        private RelayCommand startRecordingCmd;

        public RelayCommand StartRecordingCmd => startRecordingCmd ?? (startRecordingCmd = new RelayCommand(()=>
        {
            Console.WriteLine(IDs + " " + " Helloooo");
        }));

        private RelayCommand stopRecordingCmd;

        [Keys]
        public RelayCommand StopRecordingCmd => stopRecordingCmd ?? (stopRecordingCmd = new RelayCommand(() =>
        {

            Console.WriteLine(IDs+ " " +"MOO" );

        }));
    }



    public class HotKey : ObservableObject
    {

        private PropertyInfo command;
        private string key;
        public event EventHandler<string> Pressed;

        public SavableObject Owner { get; set; }
        //public IEnumerable<string> HotKeys { get; private set; }

        public PropertyInfo Command
        {
            get => command;
            set
            {
                if (value == command) return;
                command = value;
                RaisePropertyChanged(nameof(Command));
            }
        }

        public string Key
        {
            get => key;
            set
            {
                if (key == value) return;

                OldKey = key;
                key = value;

                RaisePropertyChanged(nameof(Key));

                Pressed?.Invoke(this, Key);

            }
        }

        private string oldKey = string.Empty;
        public string OldKey
        {
            get => oldKey;
            set
            {
                if (OldKey != value)
                {
                    oldKey = value;
                    //RaisePropertyChanged(nameof(OldKey));
                }
            }
        }

        public string IDs { get; internal set; }
    }


    public class KeysVM : SubCBaseVM, INotifyPropertyChanged
    {

        public SavableObject ActiveOwner { get; private set; }

        public string Command { get; private set; }

        public IEnumerable<object> CustomAttributes { get; private set; }

        public ObservableCollection<HotKey> KeysCollection { get; } = new ObservableCollection<HotKey>() { };

        public SavableObject Owner { get; private set; }

        private int test;
        public RecordingVM IDss;

        [Savable]
        public int Test
        {
            get => test;
            set => Set(nameof(Test), ref test, value);
        }

        public KeysVM(ISettingsService settings, params SavableObject[] classes) : base(settings,"KeysVM")
        {
 
            foreach (var item in classes)
            {
                var Properties= item.GetType().GetProperties().Where(m => m.GetCustomAttributes(true).OfType<KeysAttribute>().Count() > 0);

                foreach (var property in Properties)
                {
                    
                    var log = new HotKey() { Owner = item, Command = property, Key = string.Empty, IDs = string.Empty };

                    KeysCollection.Add(log);

                    log.Pressed += KeyPress;

                    foreach (var key in KeysCollection)
                    {
                        settings.Update($@"HotKeys/{log.Key}""/{property.Name}""/{item.ID}""/{IDss}");
                    }

                }

            }
             
            LoadSettingsAsync();
        }

        public void OnTabSelected(object sender, SelectionChangedEventArgs e)
        {
            TabItem selectedTab = e.AddedItems[0] as TabItem;
          
            if (selectedTab.Name == "Tab1" | selectedTab.Name == "Tab2")
            {
                HotKey activeOwner = new HotKey()
                {
                    Owner = ActiveOwner
                };

            }

            else if (selectedTab.Name == null)
            {
                Console.WriteLine("there is no item");
            }

        }

        public void SaveKey(HotKey HotKey, RecordingVM ID)
        {

            var newKey = ($@"{HotKey.Key}");

            if (newKey != HotKey.OldKey)
            {
                settings.Remove($@"HotKeys/{HotKey.OldKey}");
            }

            //settings.Update($@"Keys/{HotKey.Key}""/{HotKey.Command.Name}""/{HotKey.Owner.ID}""/{ID}");
            settings.Update($@"HotKeys/{HotKey.Key}/Command/", $"{HotKey.Command.Name}");
            settings.Update($@"HotKeys/{HotKey.Key}/Owner/", $"{HotKey.Owner}");
            settings.Update($@"HotKeys/{HotKey.Key}/ID/", $"{ID}");

        }

        public void KeyPress(object sender, string s)
        {
            var itm = KeysCollection.FirstOrDefault(c => (((HotKey)sender).Command) == c.Command );
            var ids =  (((HotKey)sender).Owner.ID);
            SaveKey(itm, ids);
        }

        public void ExecuteKey(string key)
        {
            var command = KeysCollection.FirstOrDefault(k => k.Key == key);

            if(command != null)
            {
                (command.Command.GetValue(command.Owner) as ICommand)?.Execute(null);
            }
        }

        public override async Task LoadSettingsAsync()
        {
           
            var loaded = settings.LoadAll("HotKeys");

            foreach (var load in loaded)
            {

                var itm = KeysCollection.Where(p => p.Command.Name == load.Value);

                if (itm != null)

                {
                    foreach (var key in itm)
                    {
                        if (key.Key == load.Name && key.Owner.ID == load.Value)
                        {
                            //open settings inside load
                            //command
                            //&& key.Owner.ID == subitem.value
                            //id

                            KeysCollection.GetEnumerator();
                            //itm = KeysCollection.Data();
                             
                            var splits = load.Value.Split("/,".ToCharArray());

                            foreach (var split in splits)
                            {
                                Console.WriteLine($"<{split}>");
                            }
                        }
                    }

                }

                else
                {
                    Console.WriteLine("unparsed");
                }

                await base.LoadSettingsAsync();
            }

           
        }

    }

}

