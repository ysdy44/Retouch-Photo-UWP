# Retouch-Photo-UWP
Simple and free photo retouching tools,can transform,adjustment, shear,filling images,support layer,the selecti on. If you think that the PS is too complex might as well try.
[form windows10 app-store](https://www.microsoft.com/store/productId/9N2SVF2769GH)   

![](Screenshot/logo.png)  

（Maintenance has been stopped）

## 1.new project
new project> width:1000 height:1000>OK
  

## 2.Tools
Rrtouch-Photo provide dozens of tools: hand,curves,straw,Magic Wand,,Rectangular Marquee,Elliptical Marquee,Polygonal Marquee,Free Hand,,Selection Paint,Selection Eraser,Smudge,Mixer,,paint,pencil,pen,eraser......<br> 
[ToolControl](%E4%BF%AE%E5%9B%BE/Control/ToolControl.xaml)   <br> 
[ToolPage](https://github.com/ysdy44/Retouch-Photo-UWP/tree/master/%E4%BF%AE%E5%9B%BE/BarPage/ToolPage)  
![](Screenshot/001.jpg)  


## 3.Canvas
When you choose a different Tool, barPage will navigate to the corresponding Page。<br> 
At this point, you can perform operations on the MainCanvasControl<br> 
[MainCanvasControl](%E4%BF%AE%E5%9B%BE/Control/MainCanvasControl.xaml)
![](Screenshot/002.jpg)  
![](Screenshot/003.jpg)  


## 4.Edit
Edit Lasso selected mask by :Cut,Copy,Paste,Clear,Extract,Copy Merged<br> 
Edit Lasso by :Select All,Deselect,Select Pixel,Inverse,Feather,Transform <br> 
[MaskButton](%E4%BF%AE%E5%9B%BE/Control/MaskButton.xaml)
![](Screenshot/004.jpg)  

## 5.Effect
Add special effects to the image with parameters<br> 
[Effect](https://github.com/ysdy44/Retouch-Photo-UWP/tree/master/%E4%BF%AE%E5%9B%BE/BarPage/EffectPage)<br>
Gray,Invert,Exposure,Brightness,Saturation,HueRotatom,Contrast.Temperature,Highights and Shadow<br> 
[Effect1](https://github.com/ysdy44/Retouch-Photo-UWP/tree/master/%E4%BF%AE%E5%9B%BE/BarPage/EffectPage1)<br>
DirectionBlur,GaussianBlur.Sharp.en.Shadow.Chromakey.EdgeDetection,Border,Emboss,Lighting<br> 
[Effect2](https://github.com/ysdy44/Retouch-Photo-UWP/tree/master/%E4%BF%AE%E5%9B%BE/BarPage/EffectPage2)<br>
LuminanceToAlpha,Fog,Sepia,Posterize,Colouring,Tint,DiscreteTransfer,Vignette,GammaTransfer<br> 
[Effect3](https://github.com/ysdy44/Retouch-Photo-UWP/tree/master/%E4%BF%AE%E5%9B%BE/BarPage/EffectPage3)<br>
Glass,Pinch and Punch,Morphology<br> 
[EffectBuuton](%E4%BF%AE%E5%9B%BE/Control/EffectBuuton.xaml)
![](Screenshot/005.jpg)  


## 6.Others
Other operations:Crop,Stretch,Rotate,Gradient,Fade
Text,Grid,Fill,Transform<br> 
[OtherButton](%E4%BF%AE%E5%9B%BE/Control/OtherButton.xaml)
![](Screenshot/006.jpg)  

## 7.Layer
Add,Delete,Merge,Duplicate Layers<br> 
Change the Name of the layer, Opacity, BlendMode<br> 
Normal,Multiply,Screen,Dissolve,,Darken,Lighten,DarkerColor,LighterColor,,ColorBurn,ColorDodge,LinearBurn,LinearDodge,,Overlay ,HardLight,VividLight,LinearLight,PinLight,,HardMix,Difference,Exclusion,,Hue,Saturation,Color,,Luminosity,Subtract,Division<br> 
[LayerControl](%E4%BF%AE%E5%9B%BE/Control/LayerControl.xaml)
![](Screenshot/007.jpg)  

## 8。Adjustment Mode
Use the parameter slider on the right to change the effect of the picture<br> 
[AdjustControl](%E4%BF%AE%E5%9B%BE/Control/AdjustControl.xaml)<br>
You can also choose ready-made filters<br> 
[FilterControl](%E4%BF%AE%E5%9B%BE/Control/FilterControl.xaml)
![](Screenshot/008.jpg)  

## 9.Paint Mode
It is now a professional drawing software<br>
[ColorPicker](%E4%BF%AE%E5%9B%BE/Picker/ColorPicker.xaml)<br>
[BrushControl](%E4%BF%AE%E5%9B%BE/Control/BrushControl.xaml)
![](Screenshot/009.jpg)

## 10.Geometry
You can freely draw any shape<br>
[GeometryControl](%E4%BF%AE%E5%9B%BE/Control/GeometryControl.xaml)
![](Screenshot/010.jpg)
