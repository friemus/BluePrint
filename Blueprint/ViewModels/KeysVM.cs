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

namespace Blueprint.ViewModels
{

    //DO NOT USE WITH PARAMETERIZED RELAYCOMMANDS
    public class KeysAttribute : Attribute
    {

    }

    public class RecordingVM
    {
        private int IDs;
        public int ID { get; private set; }
        public RecordingVM(int ID)
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

        public object Owner { get; set; }

        //public static string Keys { get; internal set; } 

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

                key = value;

                RaisePropertyChanged(nameof(Key));

                Pressed?.Invoke(this, Key);

            }
        }

    }


    public class KeysVM : SubCBaseVM, INotifyPropertyChanged
    {

        public string Command { get; private set; }

        public IEnumerable<object> CustomAttributes { get; private set; }

        public ObservableCollection<HotKey> KeysCollection { get; } = new ObservableCollection<HotKey>() { };

        public object Owner { get; private set; }

        private int test;

        [Savable]
        public int Test
        {
            get => test;
            set => Set(nameof(Test), ref test, value);
        }

        public KeysVM(ISettingsService settings, params object[] classes) : base(settings,"KeysVM")
        {
 
            foreach (var item in classes)
            {
                //char c = 'j';
                var Properties= item.GetType().GetProperties().Where(m => m.GetCustomAttributes(true).OfType<KeysAttribute>().Count() > 0);

                foreach (var property in Properties)
                {
                    
                    var log = new HotKey() { Owner = item, Command = property, Key = string.Empty};

                    //item.Key += (e) => Key?.Invoke(e);

                    KeysCollection.Add(log);

                    log.Pressed += KeyPress;

                    //foreach (var key in KeysCollection)
                    //{
                    //    settings.Update($@"Keys:/{log.Key}",  $"{property.Name}");
                    //}
                    
                }
              
            }
             
            LoadSettingsAsync();
        }

        public void SaveKey(string Command, string Key)
        {
            settings.Update($@"Keys:/{Key}", $"{Command}");
        }

        public void KeyPress(object sender, string s)
        {
            var itm = KeysCollection.FirstOrDefault(c => (((HotKey)sender).Command) == c.Command );

            SaveKey(itm.Command.Name, s);
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
           
            var loaded = settings.LoadAll("Keys");

            foreach (var load in loaded)
            {
                var itm = KeysCollection.FirstOrDefault(p => p.Command.Name == load.Value);

                if (itm != null)

                {
                    itm.Key = load.Name;
                    //var commandName = load.Value.Split(":".ToCharArray()).Last();

                    //Test = Int32.Parse(load.Value);

                    //settings?.Open("Settings.xml");

                    //settings.Update($@"{load.Name}", $"{Test}");

                    Console.WriteLine("load" + " " + load.Name + " " + load.Value);

                }


                //settings.Update($@"{load.Name}", $"{Test}");

                //settings?.Open("Settings.xml");

                await base.LoadSettingsAsync();
            }

           
        }

    }

}

