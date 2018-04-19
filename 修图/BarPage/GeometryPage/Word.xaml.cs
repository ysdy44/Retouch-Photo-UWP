using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Text;
namespace 修图.BarPage.GeometryPage
{
    public sealed partial class Word : Page
    {
        public Word()
        {
            this.InitializeComponent();
        }



        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Setting.WordFontStyle = FontStyle.Normal;
        }
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            App.Setting.WordFontStyle = FontStyle.Oblique;
        }
        private void ToggleButton_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleButton.Unchecked += ToggleButton_Unchecked;
            ToggleButton.Checked += ToggleButton_Checked;
        }






        private void FontPicker_FontChange(Object sender, String FontFamily)
        {
            App.Setting.WordFontFamily = FontFamily;
            TextBox.FontFamily =new FontFamily(FontFamily) ;
        }




        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Setting.WordText = TextBox.Text;

         }

        private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            App.Setting.WordText = sender.Text;
          }

    
    }
}
