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
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Brushes;
using System.Numerics;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI;

namespace 修图.Picker
{
    public sealed partial class StrokePicker : UserControl
    {
        int Mode;


        //Delegate
        public delegate void StrokeChangedHandler(CanvasStrokeStyle StrokeStyle);
        public event StrokeChangedHandler StrokeChanged;

        #region DependencyProperty：依赖属性

        public CanvasStrokeStyle StrokeStyle
        {
            get { return (CanvasStrokeStyle)GetValue(StrokeStyleProperty); }
            set { SetValue(StrokeStyleProperty, value); }
        }
          public static readonly DependencyProperty StrokeStyleProperty =
            DependencyProperty.Register("StrokeStyle", typeof(CanvasStrokeStyle), typeof(StrokePicker), new PropertyMetadata(new CanvasStrokeStyle() , new PropertyChangedCallback(StrokeStyleOnChang)));

        private static void StrokeStyleOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            StrokePicker Con = (StrokePicker)sender;
        }

        #endregion


        public StrokePicker()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }

        private void Stroke()
        {
            StrokeChanged?.Invoke(StrokeStyle);
        }


        #region Global：全局


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 0
            if (Mode == 0)
            {
                DashButton.IsChecked = true;
                DashGrid.Visibility = Visibility.Visible;
            }
            else
            {
                DashButton.IsChecked = false;
                DashGrid.Visibility = Visibility.Collapsed;
            }
            // 1
            if (Mode == 1)
            {
                CapButton.IsChecked = true;
                CapGrid.Visibility = Visibility.Visible;
            }
            else
            {
                CapButton.IsChecked = false;
                CapGrid.Visibility = Visibility.Collapsed;
            }
            // 2
            if (Mode == 2)
            {
                JoinButton.IsChecked = true;
                JoinGrid.Visibility = Visibility.Visible;
            }
            else
            {
                JoinButton.IsChecked = false;
                JoinGrid.Visibility = Visibility.Collapsed;
            }
            // 3
            if (Mode == 3)
            {
                ArrayButton.IsChecked = true;
                ArrayGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ArrayButton.IsChecked = false;
                ArrayGrid.Visibility = Visibility.Collapsed;
            }
        }


         private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == DashButton)
            {
                DashButton.IsChecked = true;
                DashGrid.Visibility = Visibility.Visible;
                Mode = 0;
            }
            else
            {
                DashButton.IsChecked = false;
                DashGrid.Visibility = Visibility.Collapsed;
            }
            // 1
            if (tb == CapButton)
            {
                CapButton.IsChecked = true;
                CapGrid.Visibility = Visibility.Visible;
                Mode = 1;
            }
            else
            {
                CapButton.IsChecked = false;
                CapGrid.Visibility = Visibility.Collapsed;
            }

            // 2
            if (tb == JoinButton)
            {
                JoinButton.IsChecked = true;
                JoinGrid.Visibility = Visibility.Visible;
                Mode = 2;
            }
            else
            {
                JoinButton.IsChecked = false;
                JoinGrid.Visibility = Visibility.Collapsed;
            }

            // 3
            if (tb == ArrayButton)
            {
                ArrayButton.IsChecked = true;
                ArrayGrid.Visibility = Visibility.Visible;
                Mode = 3;
            }
            else
            {
                ArrayButton.IsChecked = false;
                ArrayGrid.Visibility = Visibility.Collapsed;
            }
        }

         
         #endregion

        #region Dash：破折


        private void DashGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // 0
            if (StrokeStyle.DashStyle == CanvasDashStyle.Solid) Dash0.IsChecked = false;
            else Dash0.IsChecked = true;
            // 1
            if (StrokeStyle.DashStyle == CanvasDashStyle.Dash) Dash1.IsChecked = false;
            else Dash1.IsChecked = true;
            // 2
            if (StrokeStyle.DashStyle == CanvasDashStyle.Dot) Dash2.IsChecked = false;
            else Dash2.IsChecked = true;
            // 3
            if (StrokeStyle.DashStyle == CanvasDashStyle.DashDot) Dash3.IsChecked = false;
            else Dash3.IsChecked = true;
        }

        private void Dash_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == Dash0)
            {
                Dash0.IsChecked = false;
                StrokeStyle.DashStyle = CanvasDashStyle.Solid;
            }
            else Dash0.IsChecked = true;
            // 1
            if (tb == Dash1)
            {
                Dash1.IsChecked = false;
                StrokeStyle.DashStyle = CanvasDashStyle.Dash;
            }
            else Dash1.IsChecked = true;

            // 2
            if (tb == Dash2)
            {
                Dash2.IsChecked = false;
                StrokeStyle.DashStyle = CanvasDashStyle.Dot;
            }
            else Dash2.IsChecked = true;

            // 3
            if (tb == Dash3)
            {
                Dash3.IsChecked = false;
                StrokeStyle.DashStyle = CanvasDashStyle.DashDot;
            }
            else Dash3.IsChecked = true;

            Stroke();
        }


        #endregion

        #region Cap：线帽

        private void CapGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // 0
            if (StrokeStyle.DashCap == CanvasCapStyle.Flat) Cap0.IsChecked = false;
            else Cap0.IsChecked = true;
            // 1
            if (StrokeStyle.DashCap == CanvasCapStyle.Square) Cap1.IsChecked = false;
            else Cap1.IsChecked = true;
            // 2
            if (StrokeStyle.DashCap == CanvasCapStyle.Round) Cap2.IsChecked = false;
            else Cap2.IsChecked = true;
            // 3
            if (StrokeStyle.DashCap == CanvasCapStyle.Triangle) Cap3.IsChecked = false;
            else Cap3.IsChecked = true;
        }

        private void Cap_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == Cap0)
            {
                Cap0.IsChecked = false;
                StrokeStyle.DashCap = CanvasCapStyle.Flat;
            }
            else Cap0.IsChecked = true;
            // 1
            if (tb == Cap1)
            {
                Cap1.IsChecked = false;
                StrokeStyle.DashCap = CanvasCapStyle.Square;
            }
            else Cap1.IsChecked = true;

            // 2
            if (tb == Cap2)
            {
                Cap2.IsChecked = false;
                StrokeStyle.DashCap = CanvasCapStyle.Round;
            }
            else Cap2.IsChecked = true;

            // 3
            if (tb == Cap3)
            {
                Cap3.IsChecked = false;
                StrokeStyle.DashCap = CanvasCapStyle.Triangle;
            }
            else Cap3.IsChecked = true;

            Stroke();
        }

        #endregion

        #region Join：关节

        private void JoinGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // 0
            if (StrokeStyle.LineJoin == CanvasLineJoin.Miter) Join0.IsChecked = false;
            else Join0.IsChecked = true;
            // 1
            if (StrokeStyle.LineJoin == CanvasLineJoin.Bevel) Join1.IsChecked = false;
            else Join1.IsChecked = true;
            // 2
            if (StrokeStyle.LineJoin == CanvasLineJoin.Round) Join2.IsChecked = false;
            else Join2.IsChecked = true;
            // 3
            if (StrokeStyle.LineJoin == CanvasLineJoin.MiterOrBevel) Join3.IsChecked = false;
            else Join3.IsChecked = true;
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == Join0)
            {
                Join0.IsChecked = false;
                StrokeStyle.LineJoin = CanvasLineJoin.Miter;
            }
            else Join0.IsChecked = true;
            // 1
            if (tb == Join1)
            {
                Join1.IsChecked = false;
                StrokeStyle.LineJoin = CanvasLineJoin.Bevel;
            }
            else Join1.IsChecked = true;

            // 2
            if (tb == Join2)
            {
                Join2.IsChecked = false;
                StrokeStyle.LineJoin = CanvasLineJoin.Round;
            }
            else Join2.IsChecked = true;

            // 3
            if (tb == Join3)
            {
                Join3.IsChecked = false;
                StrokeStyle.LineJoin = CanvasLineJoin.MiterOrBevel;
            }
            else Join3.IsChecked = true;

            Stroke();
        }

        #endregion

        #region Array：间隔

        #endregion
         
    }
}
