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
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;

namespace 修图.BarPage.ToolPage
{
    public sealed partial class Pen : Page
    {
        public Pen()
        {
            this.InitializeComponent();
        }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StrokePicker.StrokeChanged += StrokePicker_StrokeChanged;

            WidthSlider.Value = App.Setting.PenWidth;
            WidthNumberPicker.Value = (int)App.Setting.PenWidth;
            WidthSlider.ValueChanged += WidthSlider_ValueChanged;
            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;

            // 0
            if (App.Setting.PenMode == 0)
            {
                PenAdd.IsChecked = true;
                PenAddGrid.Visibility = Visibility.Visible;
            }
            else
            {
                PenAdd.IsChecked = false;
                PenAddGrid.Visibility = Visibility.Collapsed;
            }
            // 1
            if (App.Setting.PenMode == 1)
            {
                PenEdit.IsChecked = true;
                PenEditGrid.Visibility = Visibility.Visible;
            }
            else
            {
                PenEdit.IsChecked = false;
                PenEditGrid.Visibility = Visibility.Collapsed;
            }

            // 1
            if (App.Setting.PenMode == 2)
            {
                PenMode.IsChecked = true;
                PenModeGrid.Visibility = Visibility.Visible;
            }
            else
            {
                PenMode.IsChecked = false;
                PenModeGrid     .Visibility = Visibility.Collapsed;
            }


            // 0
            if (App.Setting.PenDash==0) Dash0.IsChecked = true;
            else Dash0.IsChecked = false;
            // 1
            if (App.Setting.PenDash == 1) Dash1.IsChecked = true;
            else Dash1.IsChecked = false;
            // 2
            if (App.Setting.PenDash ==2) Dash2.IsChecked = true;
            else Dash2.IsChecked = false;
            // 3
            if (App.Setting.PenDash == 3) Dash3.IsChecked = true;
            else Dash3.IsChecked = false;
        }



        private void Pen_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == PenAdd)
            {
                PenAdd.IsChecked = true;
                PenAddGrid.Visibility = Visibility.Visible;

                App.Setting.PenMode = 0;
            }
            else
            {
                PenAdd.IsChecked = false;
                PenAddGrid.Visibility = Visibility.Collapsed;
            }

            // 1
            if (tb == PenEdit)
            {
                PenEdit.IsChecked = true;
                PenEditGrid.Visibility = Visibility.Visible;

                App.Setting.PenMode = 1;
            }
            else
            {
                PenEdit.IsChecked = false;
                PenEditGrid.Visibility = Visibility.Collapsed;
            }

            // 2
            if (tb == PenMode)
            {
                PenMode.IsChecked = true;
                PenModeGrid.Visibility = Visibility.Visible;

                App.Setting.PenMode = 2;
            }
            else
            {
                PenMode.IsChecked = false;
                PenModeGrid.Visibility = Visibility.Collapsed;
            }
        }



        #endregion


        #region Pen：钢笔


        private void Dash_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == Dash0)
            {
                Dash0.IsChecked = false;
                App.Setting.PenDash = 0;
            }
            else Dash0.IsChecked = true;
            // 1
            if (tb == Dash1)
            {
                Dash1.IsChecked = false;
                App.Setting.PenDash = 1;
            }
            else Dash1.IsChecked = true;

            // 2
            if (tb == Dash2)
            {
                Dash2.IsChecked = false;
                App.Setting.PenDash = 2;
            }
            else Dash2.IsChecked = true;

            // 3
            if (tb == Dash3)
            {
                Dash3.IsChecked = false;
                App.Setting.PenDash = 3;
            }
            else Dash3.IsChecked = true;

            App.Model.Refresh++;//画布刷新
        }




        private void StrokePicker_StrokeChanged(CanvasStrokeStyle StrokeStyle)
        {
            App.Setting.PenStrokeStyle = StrokeStyle;

            App.Model.Refresh++;//画布刷新
        }


        private void ColorButton_ColorChanged(Windows.UI.Color Color)
        {
            App.Model.Refresh++;//画布刷新
         }





        //Width：宽度 
        private void WidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.PenWidth = (float)e.NewValue;
            WidthNumberPicker.Value = (int)e.NewValue;
        }
        private void WidthNumberPicker_ValueChange(object sender, int Value)
        {
             App.Setting.PenWidth = Value;
            WidthSlider.Value = Value;
          }


        #endregion


        #region Edit：编辑


        private void AddButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //上一个点是否确认
            var isChecked = false;

            //要清增加的点集合
            List<int> list = new List<int> { };

            for (int i = 0; i < App.Setting.PenVectorList.Count; i++)
            {
                if (App.Setting.PenVectorList[i].isChecked == true&& isChecked==true)
                {
                    list.Add(i);//增加点集合
                }
                //旧点
                isChecked = App.Setting.PenVectorList[i].isChecked;
            }

            for (int i = 0; i < list.Count; i++)
            {
                //计算索引与向量
                var index = list[i] + i;
                Vector2 vect = App.Setting.PenVectorList[index - 1].Vect + App.Setting.PenVectorList[index ].Vect;

                //new一个pen并添加
                修图.Model.Pen pen = new 修图.Model.Pen(vect/2);
                pen.isChecked = true;
                App.Setting.PenVectorList.Insert(index, pen);
            }

            App.Model.Refresh++;//画布刷新
        }

        private void SharpButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
             foreach (var pen in App.Setting.PenVectorList)
            {
                if (pen.isChecked==true)
                {
                    pen.Left = pen.Right = pen.Vect;
                }
            }

            App.Model.Refresh++;//画布刷新
        }

        private void SmoothButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
             for (int i = 1; i < App.Setting.PenVectorList.Count-1; i++)
            {
                if (App.Setting.PenVectorList[i].isChecked == true)
                { 
                    //Space为前一个和后一个点的差距
                    Vector2 Space = App.Setting.PenVectorList[i + 1].Vect - App.Setting.PenVectorList[i - 1].Vect;
                    Vector2 Spa = Space / 6;

                    //平滑左右点
                    App.Setting.PenVectorList[i].Left = App.Setting.PenVectorList[i].Vect + Spa;
                    App.Setting.PenVectorList[i].Right = App.Setting.PenVectorList[i].Vect - Spa;
                }
            }

            App.Model.Refresh++;//画布刷新
        }

        private void ClearButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //要清空的点集合
            List<int> list = new List<int> { };

             for (int i = App.Setting.PenVectorList.Count-1; i>=0; i--)
            {
                if (App.Setting.PenVectorList[i].isChecked == true)
                {
                    list.Add(i);//添加到点集合
                }
            }
            foreach (var item in list)//根据点集合删除点
            {
                App.Setting.PenVectorList.RemoveAt(item);
            }

            App.Model.Refresh++;//画布刷新
        }


        #endregion


        #region to：输出


        private void toMask_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.Setting.PenVectorList.Count > 1)
            {
                CanvasPathBuilder VirtualBuilder = new CanvasPathBuilder(App.Model.VirtualControl);
            VirtualBuilder.BeginFigure(App.Setting.PenVectorList[0].Vect);

            CanvasPathBuilder AnimatedBuilder = new CanvasPathBuilder(App.Model.AnimatedControl);
            AnimatedBuilder.BeginFigure(App.Setting.PenVectorList[0].Vect);

            for (int i = 0; i < App.Setting.PenVectorList.Count - 1; i++)//0 to 9
            {
                Vector2 vl = App.Setting.PenVectorList[i].Left;
                Vector2 v1r =App.Setting.PenVectorList[i + 1].Right;
                Vector2 v1 = App.Setting.PenVectorList[i + 1].Vect;

                VirtualBuilder.AddCubicBezier(vl , v1r, v1 );
                AnimatedBuilder.AddCubicBezier(vl, v1r, v1);
            }
            VirtualBuilder.EndFigure(CanvasFigureLoop.Closed);
            AnimatedBuilder.EndFigure(CanvasFigureLoop.Closed);

            //几何图形
            CanvasGeometry VirtualGeometry = CanvasGeometry.CreatePath(VirtualBuilder);
            CanvasGeometry AnimatedGeometry = CanvasGeometry.CreatePath(AnimatedBuilder);

            //几何
            App.Mask(VirtualGeometry, AnimatedGeometry, App.Model.MaskMode); //改变选区与套索

            App.Model.isReStroke = true;//重新设置描边
            App.Judge();//判断选区，改变是否动画与选区矩形

            }

            flyout.Hide();
        }



        private void toPoints_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.Setting.PenVectorList.Count > 1)
            {
                float R = App.Setting.PenWidth;
                using (var ds = App.Model.Layers[App.Model.Index].CanvasRenderTarget.CreateDrawingSession())
                {
                    for (int i = 0; i < App.Setting.PenVectorList.Count; i++)//0 to 9
                    {
                        Vector2 v = App.Setting.PenVectorList[i].Vect;

                        ds.FillEllipse(v, R, R, App.Model.PenColor);
                    }
                }
                App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }

            flyout.Hide();
        }

        private void toFill_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.Setting.PenVectorList.Count > 1)
            {
                CanvasPathBuilder VirtualBuilder = new CanvasPathBuilder(App.Model.VirtualControl);
                VirtualBuilder.BeginFigure(App.Setting.PenVectorList[0].Vect);

                for (int i = 0; i < App.Setting.PenVectorList.Count - 1; i++)//0 to 9
                {
                    Vector2 vl = App.Setting.PenVectorList[i].Left;
                    Vector2 v1r = App.Setting.PenVectorList[i + 1].Right;
                    Vector2 v1 = App.Setting.PenVectorList[i + 1].Vect;

                    VirtualBuilder.AddCubicBezier(vl, v1r, v1);
                }
                VirtualBuilder.EndFigure(CanvasFigureLoop.Closed);

                using (var ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
                {
                    ds.FillGeometry(CanvasGeometry.CreatePath(VirtualBuilder), App.Model.PenColor);
                }
                App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }

            flyout.Hide();
        }


        private void toStroke_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.Setting.PenVectorList.Count > 1)
            {
                CanvasPathBuilder VirtualBuilder = new CanvasPathBuilder(App.Model.VirtualControl);
                VirtualBuilder.BeginFigure(App.Setting.PenVectorList[0].Vect);

                for (int i = 0; i < App.Setting.PenVectorList.Count - 1; i++)//0 to 9
                {
                    Vector2 vl = App.Setting.PenVectorList[i].Left;
                    Vector2 v1r = App.Setting.PenVectorList[i + 1].Right;
                    Vector2 v1 = App.Setting.PenVectorList[i + 1].Vect;

                    VirtualBuilder.AddCubicBezier(vl, v1r, v1);
                }
                VirtualBuilder.EndFigure(CanvasFigureLoop.Closed);

                using (var ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
                {
                    ds.DrawGeometry(CanvasGeometry.CreatePath(VirtualBuilder), App.Model.PenColor, App.Setting.PenWidth, App.Setting.PenStrokeStyle);
                }
                App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }

            flyout.Hide();
        }


        private void toClear_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Setting.PenVectorList.Clear();
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新

            flyout.Hide();
        }


        #endregion


    }
}
