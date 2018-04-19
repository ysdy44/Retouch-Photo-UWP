using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace 修图.BarPage.ToolPage
{
    public sealed partial class Ellipse : Page
    {
        public Ellipse()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
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
            Icon.Height = App.Setting.isSquare == true ? 22 : 16;

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
            Icon.Height = 16;
        }




    }
}
