using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace 修图.BarPage.ToolPage
{
    public sealed partial class Rectangle : Page
    {
        public Rectangle()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;

            Slider.Value = App.Setting.Radius;
            Icon.RadiusX = Icon.RadiusY = App.Setting.Radius / 10;
        }


        //Rectangle：圆角 
        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Radius = (float)e.NewValue;
            NumberPicker.Value = (int)e.NewValue;

            Icon.RadiusX = Icon.RadiusY = e.NewValue / 10;
        }
        private void NumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Radius = Value;
            Slider.Value = Value;

            Icon.RadiusX = Icon.RadiusY = Value / 10;
        }




        //Rectangle：中心 
        private void CenterCheck_Loaded(object sender, RoutedEventArgs e)
        {
            CenterCheck.IsChecked = App.Setting.isCenter == true ? true : false;
            Fore.Visibility = App.Setting.isCenter == true ? Visibility.Visible : Visibility.Collapsed;

            CenterCheck.Checked += CenterCheck_Checked;
            CenterCheck.Unchecked += CenterCheck_Unchecked;
        }
        private void CenterCheck_Checked(object sender, RoutedEventArgs e)
        {
            App.Setting.isCenter = true;
            Fore.Visibility = Visibility.Visible;
        }
        private void CenterCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Setting.isCenter = false;
            Fore.Visibility = Visibility.Collapsed;
        }

        

        //Rectangle：正方形 
        private void SquareCheck_Loaded(object sender, RoutedEventArgs e)
        {
            SquareCheck.IsChecked = App.Setting.isSquare == true ? true : false;
            Icon.Height = App.Setting.isSquare == true ? 22 : 18;

            SquareCheck.Checked += SquareCheck_Checked;
            SquareCheck.Unchecked += SquareCheck_Unchecked;
        }
        private void SquareCheck_Checked(object sender, RoutedEventArgs e)
        {
            App.Setting.isSquare = true;
            Icon.Height = 22;
        }
        private void SquareCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Setting.isSquare = false;
            Icon.Height = 18;
        }

    
    }
}
