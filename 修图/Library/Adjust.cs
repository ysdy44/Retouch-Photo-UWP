using System;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace 修图.Library
{
    class Adjust
    {

        #region Mask：选区


        //羽化方法
        public static ICanvasImage GetFeather(ICanvasImage canvasImage, float Feather)
        {
            if (Feather < 0) Feather = 0;

            return new GaussianBlurEffect//高斯模糊
            {
                Source = GetGaussianBlur(canvasImage, App.Setting.Feather),
                BlurAmount = Feather,
            };
        }

        //边界算法
        public static ICanvasImage GetBorder(ICanvasImage canvasImage, int Border)
        {
            if (Border == 0) return canvasImage;
            else if (Border >= 90) Border = 90;
            else if (Border <= -90) Border = -90;

            return new MorphologyEffect//形态学
            {
                Source = new BorderEffect { Source = canvasImage },//边界效应
                Mode = (Border > 0) ? MorphologyEffectMode.Dilate : MorphologyEffectMode.Erode,//扩张&侵蚀
                Height = Math.Abs(Border),
                Width = Math.Abs(Border)//地区扩张
            };
        }


        #endregion


        #region Tool：工具


        //由魔棒得到区域
        public static ICanvasImage GetMagicWandr(CanvasRenderTarget SourceBitmap, int x, int y, float dragDistance)
        {
            Color clickColor = SourceBitmap.GetPixelColors(x, y, 1, 1).Single();

            return new ColorMatrixEffect
            {
                //ChromaKeyEffect：浓度帧作用
                //（替换指定的颜色和透明度）
                Source = new ChromaKeyEffect
                {
                    Source = SourceBitmap,
                    Color = clickColor,//指定的颜色
                    Tolerance = dragDistance,//容差
                    InvertAlpha = true//反转α
                },

                ColorMatrix = new Matrix5x4
                {
                    // 保持alpha
                    M44 = 1,

                    // 设置RGB =白色。
                    M51 = 1,
                    M52 = 1,
                    M53 = 1,
                }
            };
        }


        #endregion


        #region Effect：特效


        //黑白方法 
        public static ICanvasImage GetGrayscale(ICanvasImage canvasImage)
        {
            return new GrayscaleEffect { Source = canvasImage };
        }


        //反色方法 
        public static ICanvasImage GetInvert(ICanvasImage canvasImage)
        {
            return new InvertEffect { Source = canvasImage };
        }


        //曝光方法
        public static ICanvasImage GetExposure(ICanvasImage canvasImage, float exposure)
        {
            if (exposure < -2) exposure = -2;
            else if (exposure > 2) exposure = 2;

            return new ExposureEffect
            {
                Source = canvasImage,
                Exposure = exposure,
            };
        }


        //亮度方法 
        public static ICanvasImage GetBrightness(ICanvasImage canvasImage, float Brightness)
        {
            float WhiteX = Math.Min(2 - Brightness, 1);
            float WhiteY = 1f;
            float BlackX = Math.Max(1 - Brightness, 0);
            float BlackY = 0f;

            return GetBrightness(canvasImage, WhiteX, WhiteY, BlackX, BlackY);
        }
        public static ICanvasImage GetBrightness(ICanvasImage canvasImage, float WhiteX, float WhiteY, float BlackX, float BlackY)
        {
            if (WhiteX > 1) WhiteX = 1;
            else if (WhiteX < 0) WhiteX = 0;
            if (WhiteY > 1) WhiteY = 1;
            else if (WhiteY < 0) WhiteY = 0;

            if (BlackX > 1) BlackX = 1;
            else if (BlackX < 0) BlackX = 0;
            if (BlackY > 1) BlackY = 1;
            else if (BlackY < 0) BlackY = 0;

            return new BrightnessEffect
            {
                Source = canvasImage,
                WhitePoint = new Vector2(WhiteX, WhiteY),
                //亮度转移曲线的下半部分，黑点会调整图像中较暗部分的外观，介于0~1
                BlackPoint = new Vector2(BlackX, BlackY),
            };
        }


        //饱和度方法
        public static ICanvasImage GetSaturation(ICanvasImage canvasImage, float saturation)
        {
            if (saturation < 0) saturation = 0;
            else if (saturation > 2) saturation = 2;

            return new SaturationEffect
            {
                Source = canvasImage,
                Saturation = saturation,
            };
        }


        //色相方法
        public static ICanvasImage GetHueRotation(ICanvasImage canvasImage, float Angle)
        {
            if (Angle < 0f) Angle = 0f;
            else if (Angle > 6f) Angle = 6f;

            return new HueRotationEffect
            {
                Source = canvasImage,
                Angle = Angle,
            };
        }


        //对比度方法
        public static ICanvasImage GetContrast(ICanvasImage canvasImage, float Contrast)
        {
            if (Contrast < -1) Contrast = -1;
            else if (Contrast > 1) Contrast = 1;

            return new ContrastEffect
            {
                Source = canvasImage,
                Contrast = Contrast,
            };
        }


        //冷暖方法
        public static ICanvasImage GetTemperature(ICanvasImage canvasImage, float Temperature, float Tint = 0f)
        {
            if (Temperature < -1) Temperature = -1;
            else if (Temperature > 1) Temperature = 1;
            if (Tint < -1) Tint = -1;
            else if (Tint > 1) Tint = 1;

            return new TemperatureAndTintEffect
            {
                Source = canvasImage,
                Temperature = Temperature,
                Tint = Tint,
            };
        }


        //高光阴影方法
        public static ICanvasImage GetHighlightsAndShadows(ICanvasImage canvasImage, float Shadows, float Highlights, float Clarity = 0f, float MaskBlurAmount = 1.25f, bool SourceIsLinearGamma = false)
        {
            if (Shadows < -1) Shadows = -1;
            else if (Shadows > 1) Shadows = 1;
            if (Highlights < -1) Highlights = -1;
            else if (Highlights > 1) Highlights = 1;

            if (Clarity < -1) Clarity = -1;
            else if (Clarity > 1) Clarity = 1;
            if (MaskBlurAmount < 0) MaskBlurAmount = 0;
            else if (MaskBlurAmount > 10) MaskBlurAmount = 10;

            return new HighlightsAndShadowsEffect
            {
                Source = canvasImage,

                Shadows = Shadows,
                Highlights = Highlights,
                Clarity = Clarity,
                MaskBlurAmount = MaskBlurAmount,
                SourceIsLinearGamma = SourceIsLinearGamma,
            };
        }


        #endregion


        #region Effect1：特效1


        //高斯模糊方法
        public static ICanvasImage GetGaussianBlur(ICanvasImage canvasImage, float BlurAmount, EffectOptimization Optimization = EffectOptimization.Speed, EffectBorderMode BorderMode = EffectBorderMode.Soft)
        {
            if (BlurAmount < 0) BlurAmount = 0;
            else if (BlurAmount > 100) BlurAmount = 100;

            return new GaussianBlurEffect
            {
                Source = canvasImage,
                BlurAmount = BlurAmount,

                Optimization = Optimization,
                BorderMode = BorderMode,
            };
        }


        //定向模糊方法
        public static ICanvasImage GetDirectionalBlur(ICanvasImage canvasImage, float BlurAmount, float Angle, EffectOptimization Optimization, EffectBorderMode BorderMode)
        {
            if (BlurAmount < 0) BlurAmount = 0;
            else if (BlurAmount > 100) BlurAmount = 100;
            
            return new DirectionalBlurEffect
            {
                Source = canvasImage,
                BlurAmount = BlurAmount,
                Angle = Angle,
                Optimization = Optimization,
                BorderMode = BorderMode,
            };
        }


        //锐化方法
        public static ICanvasImage GetSharpen(ICanvasImage canvasImage, float Amount)
        {
            if (Amount < 0) Amount = 0;
            else if (Amount > 10) Amount = 10;

            return new SharpenEffect
            {
                Source = canvasImage,
                Amount = Amount,
            };
        }


        //阴影方法   
        public static ICanvasImage GetShadow(ICanvasImage canvasImage, Matrix3x2 m)// Virtual & Animated：虚拟 & 动画
        {
            return GetShadow(new Transform2DEffect
            {
                Source = canvasImage,
                TransformMatrix = m,
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
            }, 7, App.Model.LineColor.Color, 5, 5);
        }
        public static ICanvasImage GetShadow(ICanvasImage canvasImage, float BlurAmount, Color ShadowColor)
        {
            if (BlurAmount < 0) BlurAmount = 0;
            else if (BlurAmount > 100) BlurAmount = 100;

            return new CompositeEffect
            {
                Sources =  {new ShadowEffect
                {
                    Source = canvasImage,
                    BlurAmount = BlurAmount,
                    ShadowColor = ShadowColor,
                },
                    canvasImage
                }
            };
        }
        public static ICanvasImage GetShadow(ICanvasImage canvasImage, float BlurAmount, Color ShadowColor, float X, float Y)
        {
            if (BlurAmount < 0) BlurAmount = 0;
            else if (BlurAmount > 100) BlurAmount = 100;

            return new CompositeEffect
            {
                Sources =
                {
                          new Transform2DEffect
                        {
                            Source =   new ShadowEffect
                            {
                                Source = canvasImage,
                                BlurAmount = BlurAmount,
                                ShadowColor = ShadowColor,
                            },
                            TransformMatrix = Matrix3x2.CreateTranslation(X, Y),
                     },
                    canvasImage
                }
            };
        }
        public static ICanvasImage GetShadow(ICanvasImage canvasImage, float BlurAmount, Color ShadowColor, float X, float Y, float Opacity)
        {
            if (BlurAmount < 0) BlurAmount = 0;
            else if (BlurAmount > 100) BlurAmount = 100;
            if (Opacity < 0) Opacity = 0;
            else if (Opacity > 1) Opacity = 1;

            return new CompositeEffect
            {
                Sources =
                {
                    new OpacityEffect
                    {
                        Source =  new Transform2DEffect
                        {
                            Source =   new ShadowEffect
                            {
                                Source = canvasImage,
                                BlurAmount = BlurAmount,
                                ShadowColor = ShadowColor,
                            },
                            TransformMatrix = Matrix3x2.CreateTranslation(X, Y),
                        },
                        Opacity =Opacity
                    },
                    canvasImage
                }
            };
        }


        //消除颜色方法
        public static ICanvasImage GetChromaKey(ICanvasImage canvasImage, float Tolerance, Color Color, bool Feather, bool InvertAlpha)
        {
            if (Tolerance < 0) Tolerance = 0;
            else if (Tolerance > 1) Tolerance = 1;

            return new ChromaKeyEffect
            {
                Source = canvasImage,
                Tolerance = Tolerance,
                Color = Color,
                Feather = Feather,
                InvertAlpha = InvertAlpha,
            };
        }
     
        
        //边缘检测方法
        public static ICanvasImage GetEdgeDetection(ICanvasImage canvasImage, float Amount, float BlurAmount=0, EdgeDetectionEffectMode Mode= EdgeDetectionEffectMode.Sobel, CanvasAlphaMode AlphaMode=CanvasAlphaMode.Premultiplied, bool OverlayEdges=false)
        {
            if (Amount < 0) Amount = 0;
            else if (Amount > 1) Amount = 1;
            if (BlurAmount < 0) BlurAmount = 0;
            else if (BlurAmount > 10) BlurAmount = 10;

            return new EdgeDetectionEffect
            {
                Source = canvasImage,

                Amount = Amount,
                BlurAmount = BlurAmount,

                Mode = Mode,
                AlphaMode = AlphaMode,
                OverlayEdges = OverlayEdges,
            };
        }


        //边界方法
        public static ICanvasImage GetChromaKey(ICanvasImage canvasImage, CanvasEdgeBehavior ExtendX, CanvasEdgeBehavior ExtendY)
        {
            return new BorderEffect
            {
                Source = canvasImage,
                ExtendX = ExtendX,
                ExtendY = ExtendY,
            };
        }
        

        //浮雕方法
        public static ICanvasImage GetEmboss(ICanvasImage canvasImage, float Amount, float Angle)
        {
            if (Amount < 0) Amount = 0;
            else if (Amount > 10) Amount = 10;

            if (Angle < 0) Angle = 0;
            else if (Angle > 2 * (float)(Math.PI)) Angle = (float)(2 * Math.PI);

            return new EmbossEffect
            {
                Source = canvasImage,
                Amount = Amount,
                Angle = Angle,
            };
        }


        #endregion


        #region Effect2：特效2

        //亮度转透明度
        public static ICanvasImage GetLuminanceToAlpha(ICanvasImage canvasImage)
        {
            return new LuminanceToAlphaEffect { Source = canvasImage };
        }

        //深褐色
        public static ICanvasImage GetSepia(ICanvasImage canvasImage)
        {
            return new SepiaEffect { Source = canvasImage };
        }

        //多色调分印
        public static ICanvasImage GetPosterize(ICanvasImage canvasImage)
        {
            return new PosterizeEffect { Source = canvasImage };
        }


        //色彩方法
        public static ICanvasImage GetTint(ICanvasImage canvasImage,Color Color)
        { 
             return new TintEffect
            {
                Source = canvasImage,

                Color = Color
            };
         }
        public static ICanvasImage GetTint(ICanvasImage canvasImage, float Opacity, Color Color)
        {
            if (Opacity < 0) Opacity = 0;
            else if (Opacity > 1) Opacity = 1;

            return new CompositeEffect
            {
                Sources =
                {
                    canvasImage,
                    new OpacityEffect
                    {
                        Opacity = Opacity,
                        Source = new TintEffect
                        {
                            Source = canvasImage,
                            Color = Color
                        }
                    }
                }
            };
        }


        //装饰图案方法
        public static ICanvasImage GetVignette(ICanvasImage canvasImage, float Amount, Color Color )
        {
            if (Amount < 0) Amount = 0;
            else if (Amount > 1) Amount = 1;

            return new VignetteEffect
            {
                Source = canvasImage,
                Amount = Amount,
                Color = Color
            };
        }


        //伽马转换方法
        public static ICanvasImage GetGammaTransfer(ICanvasImage canvasImage, float AO, float RO,float GO,  float BO)
        {
            return new GammaTransferEffect
            {
                Source = canvasImage,
                AlphaOffset = AO,
                RedOffset = RO,
                GreenOffset = GO,
                BlueOffset = BO,
            };
        }
        public static ICanvasImage GetGammaTransfer(ICanvasImage canvasImage, float AA, float AE, float AO, float RA, float RE, float RO, float GA, float GE, float GO, float BA, float BE, float BO)
        {
            return new GammaTransferEffect
            {
                Source = canvasImage,

                AlphaAmplitude = AA,//振幅
                AlphaExponent = AE,//指数
                AlphaOffset = AO,//偏移

                RedAmplitude = RA,
                RedExponent = RE,
                RedOffset = RO,

                GreenAmplitude = GA,
                GreenExponent = GE,
                GreenOffset = GO,

                BlueAmplitude = BA,
                BlueExponent = BE,
                BlueOffset = BO,
            };
        }



        #endregion


        #region Effect3：特效3
         

        //柏林噪声
        public static ICanvasImage GetTurbulence( float width,float height)
        {
            return new TurbulenceEffect//柏林噪波
            {
                Octaves = 8,
                Size = new Vector2(App.Model.Width, App.Model.Height),
            };
        }


        //置换贴图
        public static ICanvasImage GetDisplacementMap(ICanvasImage canvasImage ,ICanvasImage source,float Amount, 
            EffectChannelSelect XChannelSelect= EffectChannelSelect.Red,
            EffectChannelSelect YChannelSelect= EffectChannelSelect.Green)
        {
            return new DisplacementMapEffect//取代一个输入图像的像素强度值的第二个图片。
            {
                Source = canvasImage,
                Displacement = source,
                
                XChannelSelect = XChannelSelect,
                 YChannelSelect = YChannelSelect,

                Amount = Amount
            };
        }



        //形态学方法
        public static ICanvasImage GetMorphology(ICanvasImage canvasImage, float size)
        {
            int s = (int)Math.Abs(size) / 2 + 1;
            if (s > 90) s = 90;
            return new MorphologyEffect
            {
                Mode = (size < 0) ? MorphologyEffectMode.Erode : MorphologyEffectMode.Dilate,
                Width = s,
               Height = s,
                Source = canvasImage,
            };
        }



        #endregion

        //    Adjust.
    }
}
