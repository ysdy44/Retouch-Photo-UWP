using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;


namespace 修图.BarPage
{
    public sealed partial class MaskMode : UserControl
    {

        #region DependencyProperty：依赖属性

        //刷新
        public int Mode
        {
            get { return (int)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(int), typeof(MaskMode), new PropertyMetadata(0, new PropertyChangedCallback(ModeOnChang)));

        private static void ModeOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MaskMode Con = (MaskMode)sender;
            int mode = (int)e.NewValue;

            // 0
            if (mode == 0) Con.MaskMode0.IsChecked = false;
            else Con.MaskMode0.IsChecked = true;
            // 1
            if (mode == 1) Con.MaskMode1.IsChecked = false;
            else Con.MaskMode1.IsChecked = true;
            // 2
            if (mode == 2) Con.MaskMode2.IsChecked = false;
            else Con.MaskMode2.IsChecked = true;
            // 3
            if (mode == 3) Con.MaskMode3.IsChecked = false;
            else Con.MaskMode3.IsChecked = true;
        }



         


        #endregion

        public MaskMode()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;

        }


        #region Global：全局

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 0
            if (Mode == 0) MaskMode0.IsChecked = false;
            else MaskMode0.IsChecked = true;
            // 1
            if (Mode == 1) MaskMode1.IsChecked = false;
            else MaskMode1.IsChecked = true;
            // 2
            if (Mode == 2) MaskMode2.IsChecked = false;
            else MaskMode2.IsChecked = true;
            // 3
            if (Mode == 3) MaskMode3.IsChecked = false;
            else MaskMode3.IsChecked = true;
        }

        //选区模式
        private void MaskMode_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == MaskMode0)
            {
                MaskMode0.IsChecked = false;
                Mode = 0;
            }
            else MaskMode0.IsChecked = true;

            // 1
            if (tb == MaskMode1)
            {
                MaskMode1.IsChecked = false;
                Mode = 1;
            }
            else MaskMode1.IsChecked = true;

            // 2
            if (tb == MaskMode2)
            {
                MaskMode2.IsChecked = false;
                Mode = 2;
            }
            else MaskMode2.IsChecked = true;

            // 3
            if (tb == MaskMode3)
            {
                MaskMode3.IsChecked = false;
                Mode = 3;
            }
            else MaskMode3.IsChecked = true;
        }


        #endregion

    }
}
