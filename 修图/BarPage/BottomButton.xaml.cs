using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace 修图.BarPage
{
    public sealed partial class BottomButton : UserControl
    {
        public BottomButton()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.Model.BottomVisibility == Visibility.Visible)
            {
                await FontGrid.Rotate(value: 0.0f,
                         centerX: 10f,
                         centerY: 10f,
                         duration: 0, delay: 0).StartAsync();

                App.Model.BottomVisibility = Visibility.Collapsed;
            }
            else
            {
                await FontGrid.Rotate(value: 180.0f,
                         centerX: 10f,
                         centerY: 10f,
                         duration: 0, delay: 0).StartAsync();

                App.Model.BottomVisibility = Visibility.Visible;
            }
        }

     
    }
}
