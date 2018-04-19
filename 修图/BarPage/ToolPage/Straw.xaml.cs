using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace 修图.BarPage.ToolPage
{
    public sealed partial class Straw : Page
    {
        public Straw()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 0
            if (App.Setting.isStrawCurrent == false) Main.IsChecked = false;
            else Main.IsChecked = true;
            // 1
            if (App.Setting.isStrawCurrent == true) Current.IsChecked = false;
            else Current.IsChecked = true;
        }

        private void MainCurrent_Click(object sender, RoutedEventArgs e)
        {

            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == Main)
            {
                Main.IsChecked = false;
                App.Setting.isStrawCurrent = false;
            }
            else Main.IsChecked = true;

            // 1
            if (tb == Current)
            {
                Current.IsChecked = false;
                App.Setting.isStrawCurrent = true;
            }
            else Current.IsChecked = true;
        }

    }
}
