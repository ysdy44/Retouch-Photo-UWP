using System;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Model;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Text;

namespace 修图.BarPage.OtherPage
{ 
    public sealed partial class Transform3D : Page
    {
        public Transform3D()
        {
            this.InitializeComponent();
        }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.Setting.Transform3DRect = App.GetCurrentBounds();//判断图像的矩形边界

            // 0
            if (App.Setting.Transform3DMode ==  HorizontalAlignment.Left)
            {
                Transform3DMove.IsChecked = false;
                Transform3DMoveGrid.Visibility = Visibility.Visible;
            }
            else
            {
                Transform3DMove.IsChecked = true;
                Transform3DMoveGrid.Visibility = Visibility.Collapsed;
            }
            // 1
            if (App.Setting.Transform3DMode == HorizontalAlignment.Center)
            {
                Transform3DScale.IsChecked = false;
                Transform3DScaleGrid.Visibility = Visibility.Visible;
            }
            else
            {
                Transform3DScale.IsChecked = true;
                Transform3DScaleGrid.Visibility = Visibility.Collapsed;
            }

            // 2
            if (App.Setting.Transform3DMode == HorizontalAlignment.Right)
            {
                Transform3DRotate.IsChecked = false;
                Transform3DRotateGrid.Visibility = Visibility.Visible;
            }
            else
            {
                Transform3DRotate.IsChecked = true;
                Transform3DRotateGrid.Visibility = Visibility.Collapsed;
            }



            App.Setting.Transform3DMove.X = App.Setting.Transform3DMove.Y = App.Setting.Transform3DMove.Z = 0;
            App.Setting.Transform3DScale.X = App.Setting.Transform3DScale.Y = App.Setting.Transform3DScale.Z = 1;
            App.Setting.Transform3DRotate.X = App.Setting.Transform3DRotate.Y = App.Setting.Transform3DRotate.Z = 0;

            //Move：移动
            MoveXSlider.ValueChanged += MoveXSlider_ValueChanged;
            MoveYSlider.ValueChanged += MoveYSlider_ValueChanged;
            MoveZSlider.ValueChanged += MoveZSlider_ValueChanged;
            MoveXNumberPicker.ValueChange += MoveXNumberPicker_ValueChange;
            MoveYNumberPicker.ValueChange += MoveYNumberPicker_ValueChange;
            MoveZNumberPicker.ValueChange += MoveZNumberPicker_ValueChange;

            //Scale：尺寸
            ScaleXSlider.ValueChanged += ScaleXSlider_ValueChanged;
            ScaleYSlider.ValueChanged += ScaleYSlider_ValueChanged;
            ScaleZSlider.ValueChanged += ScaleZSlider_ValueChanged;
            ScaleXNumberPicker.ValueChange += ScaleXNumberPicker_ValueChange;
            ScaleYNumberPicker.ValueChange += ScaleYNumberPicker_ValueChange;
            ScaleZNumberPicker.ValueChange += ScaleZNumberPicker_ValueChange;

            //Rotate：尺寸
            RotateXSlider.ValueChanged += RotateXSlider_ValueChanged;
            RotateYSlider.ValueChanged += RotateYSlider_ValueChanged;
            RotateZSlider.ValueChanged += RotateZSlider_ValueChanged;
            RotateXNumberPicker.ValueChange += RotateXNumberPicker_ValueChange;
            RotateYNumberPicker.ValueChange += RotateYNumberPicker_ValueChange;
            RotateZNumberPicker.ValueChange += RotateZNumberPicker_ValueChange;
         }



        private void Transform3D_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == Transform3DMove)
            {
                Transform3DMove.IsChecked = false;
                Transform3DMoveGrid.Visibility = Visibility.Visible;

                App.Setting.Transform3DMode = HorizontalAlignment.Left;
            }
            else
            {
                Transform3DMove.IsChecked = true;
                Transform3DMoveGrid.Visibility = Visibility.Collapsed;
            }

            // 1
            if (tb == Transform3DScale)
            {
                Transform3DScale.IsChecked = false;
                Transform3DScaleGrid.Visibility = Visibility.Visible;

                App.Setting.Transform3DMode = HorizontalAlignment.Right;
            }
            else
            {
                Transform3DScale.IsChecked = true;
                Transform3DScaleGrid.Visibility = Visibility.Collapsed;
            }

            // 2
            if (tb == Transform3DRotate)
            {
                Transform3DRotate.IsChecked = false;
                Transform3DRotateGrid.Visibility = Visibility.Visible;

                App.Setting.Transform3DMode = HorizontalAlignment.Right;
            }
            else
            {
                Transform3DRotate.IsChecked = true;
                Transform3DRotateGrid.Visibility = Visibility.Collapsed;
            }
        }


        #endregion


        #region Move：移动


        private void MoveXSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Transform3DMove.X = (float)e.NewValue ;
             MoveXNumberPicker.Value=(int)e.NewValue;
            Render();
        }
        private void MoveYSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Transform3DMove.Y = (float)e.NewValue;
            MoveYNumberPicker.Value = (int)e.NewValue;
            Render();
        }
        private void MoveZSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Transform3DMove.Z = (float)e.NewValue ;
            MoveZNumberPicker.Value = (int)e.NewValue;
            Render();
        }




        private void MoveXNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Transform3DMove.X = Value;
            MoveXSlider.Value = Value;
            Render();
        }
        private void MoveYNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Transform3DMove.Y = Value;
            MoveYSlider.Value = Value;
            Render();
        }
        private void MoveZNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Transform3DMove.Z = Value;
            MoveZSlider.Value = Value;
            Render();
        }


        #endregion

        #region Scale：缩放


        private void ScaleXSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Transform3DScale.X = (float)(e.NewValue / 100);
            ScaleXNumberPicker.Value = (int)e.NewValue;
            Render();
        }
        private void ScaleYSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Transform3DScale.Y = (float)(e.NewValue / 100);
            ScaleYNumberPicker.Value = (int)e.NewValue;
            Render();
        }
        private void ScaleZSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Transform3DScale.Z = (float)(e.NewValue / 100);
            ScaleZNumberPicker.Value = (int)e.NewValue;
            Render();
        }




        private void ScaleXNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Transform3DScale.X = Value / 100f;
            ScaleXSlider.Value = Value;
            Render();
        }
        private void ScaleYNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Transform3DScale.Y = Value / 100f;
            ScaleYSlider.Value = Value;
            Render();
        }
        private void ScaleZNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Transform3DScale.Z = Value / 100f;
            ScaleZSlider.Value = Value;
            Render();
        }


        #endregion

        #region Rotate：旋转


        private void RotateXSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Transform3DRotate.X = (float)(e.NewValue/180*Math.PI);
            RotateXNumberPicker.Value = (int)e.NewValue;
            Render();
        }
        private void RotateYSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Transform3DRotate.Y = (float)(e.NewValue / 180 * Math.PI);
            RotateYNumberPicker.Value = (int)e.NewValue;
            Render();
        }
        private void RotateZSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Transform3DRotate.Z = (float)(e.NewValue / 180 * Math.PI);
            RotateZNumberPicker.Value = (int)e.NewValue;
            Render();
        }




        private void RotateXNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Transform3DRotate.X = (float)(Value / 180d * Math.PI);
            RotateXSlider.Value = Value;
            Render();
        }
        private void RotateYNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Transform3DRotate.Y = (float)(Value / 180d* Math.PI);
            RotateYSlider.Value = Value;
            Render();
        }
        private void RotateZNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Transform3DRotate.Z = (float)(Value / 180d * Math.PI);
            RotateZSlider.Value = Value;
            Render();
        }


        #endregion


        public static void Render()
        {
            App.Model.SecondCanvasImage = new Transform3DEffect
            {
                Source =  App.Model.SecondSourceRenderTarget,
                TransformMatrix =App.Setting.Transform3DMatrix,
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
            };

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新 
        }      
    }
}
