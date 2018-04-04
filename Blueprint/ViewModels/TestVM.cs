using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Blueprint.ViewModels
{

    public class TestVM
    {
        [Keys]
        public void PlayVideo(string Value)
        {
            MessageBox.Show("Recording started !!!");
        }

        [Keys]
        public void StopVideo(string Value)
        {
            MessageBox.Show("Recording Stopped!!");
        }

        [Keys]
        public void ScreenShot(string Value)
        {
            MessageBox.Show("Saving Screen!!");
        }


        private RelayCommand<string> openMediaCmd;

        [Keys]
        public RelayCommand<string> OpenMediaCmd => openMediaCmd ?? (openMediaCmd = new RelayCommand<string>(p =>
        {
            //p = path to media
            Console.WriteLine(p);
        }));
    }
}
