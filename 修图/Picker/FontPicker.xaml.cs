using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace 修图.Picker
{
    public sealed partial class FontPicker : UserControl
    {
        private string _Font = "Segoe UI";
        public string Font
        {
            get => _Font;
            set
            {
                for (int i = 0; i < FontNameList.Count; i++)
                {
                    if (FontNameList[i].Name == value)
                    {
                        CmbFont.SelectedIndex = i;
                        _Font = value;
                        break;
                    }
                }
            }
        }

        List<FontInof> FontNameList = new List<FontInof>();
        List<int> FontSizeList = new List<int>();

        //用户控件，向外传值
        public delegate void FontChangeHandler(object sender, string FontFamily);
        public event FontChangeHandler FontChange = null;

        #region DependencyProperty：依赖属性
        //注册依赖属性
        public static readonly DependencyProperty FontHeaderProperty = DependencyProperty.Register("FontHeaderProperty", typeof(string), typeof(FontPicker), new PropertyMetadata("Font"));

        public string FontHeader
        {
            get { return (string)GetValue(FontHeaderProperty); }
            set { SetValue(FontHeaderProperty, value); }
        }

        //注册依赖属性
        public static readonly DependencyProperty PreviewProperty = DependencyProperty.Register("PreviewProperty", typeof(string), typeof(FontPicker), new PropertyMetadata("Preview"));

        public string Preview
        {
            get { return (string)GetValue(PreviewProperty); }
            set { SetValue(PreviewProperty, value); }
        }


        #endregion

        public FontPicker()
        {
            this.InitializeComponent();
            string language = "zh-hans";
            string[] fontList = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();

            List<FontInof> ChinaFont = new List<FontInof>();
            foreach (var name in fontList)
            {
                if (new FontEnZhName().IsTuBiao(name) == false)
                {
                    var res = new FontEnZhName().IsChineseFont(name);
                    var info = new FontInof() { Name = name };
                    if ((res != null) && (language == "zh-hans"))
                    {
                        info.ShowName = res;
                        info.ShowString = "中文字体";
                        ChinaFont.Add(info);
                    }
                    else
                    {
                        info.ShowName = name;
                        info.ShowString = "AaBbCc";
                        FontNameList.Add(info);
                    }
                }
            }
            FontNameList.AddRange(ChinaFont);

            for (int i = 8; i <= 72; i++)
                FontSizeList.Add(i);
            CmbFont.ItemsSource = FontNameList;
            for (int i = 0; i < FontNameList.Count; i++)
            {
                if (FontNameList[i].Name == Font)
                {
                    CmbFont.SelectedIndex = i;
                    break;
                }
            }

            CmbFont.SelectionChanged += CmbFont_SelectionChanged;
        }
  
    private void CmbFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbFont.SelectedIndex == -1)
            return;
        Font = FontNameList[CmbFont.SelectedIndex].Name;
        FontChange?.Invoke(this, Font); //用户控件：Value改变
    }
}

    #region Class：类

public class FontInof
{
    public string Name { get; set; }

    public string ShowName { get; set; }

    public FontFamily Family => new FontFamily(Name);

    public string ShowString { get; set; } = string.Empty;
}

public class FontEnZhName
{
    private static List<EnZhName> list = new List<EnZhName>()
        {
            new EnZhName() { En = "PMingLiU", Zh = "新细明体" },
            new EnZhName() { En = "MingLiU", Zh = "细明体" },
            new EnZhName() { En = "DFKai-SB", Zh = "标楷体" },
            new EnZhName() { En = "SimHei", Zh = "黑体" },
            new EnZhName() { En = "SimSun", Zh = "宋体" },
            new EnZhName() { En = "NSimSun", Zh = "新宋体" },
            new EnZhName() { En = "FangSong", Zh = "仿宋" },
            new EnZhName() { En = "KaiTi", Zh = "楷体" },
            new EnZhName() { En = "Microsoft JhengHei", Zh = "微软正黑体" },
            new EnZhName() { En = "Microsoft JhengHei UI", Zh = "微软正黑体 UI" },
            new EnZhName() { En = "Microsoft YaHei", Zh = "微软雅黑" },
            new EnZhName() { En = "Microsoft YaHei UI", Zh = "微软雅黑 UI" },
            new EnZhName() { En = "KaiTi", Zh = "楷体" },
            new EnZhName() { En = "KaiTi", Zh = "楷体" },
            new EnZhName() { En = "KaiTi", Zh = "楷体" },
            new EnZhName() { En = "LiSu", Zh = "隶书" },
            new EnZhName() { En = "YouYuan", Zh = "幼圆" },
            new EnZhName() { En = "STXihei", Zh = "华文细黑" },
            new EnZhName() { En = "STKaiti", Zh = "华文楷体" },
            new EnZhName() { En = "STSong", Zh = "华文宋体" },
            new EnZhName() { En = "STZhongsong", Zh = "华文中宋" },
            new EnZhName() { En = "STFangsong", Zh = "华文仿宋" },
            new EnZhName() { En = "FZShuTi", Zh = "方正舒体" },
            new EnZhName() { En = "FZYaoti", Zh = "方正姚体" },
            new EnZhName() { En = "STCaiyun", Zh = "华文彩云" },
            new EnZhName() { En = "STHupo", Zh = "华文琥珀" },
            new EnZhName() { En = "STLiti", Zh = "华文隶书" },
            new EnZhName() { En = "STXingkai", Zh = "华文行楷" },
            new EnZhName() { En = "STXinwei", Zh = "华文新魏" },
        };

    public List<string> ZiFuList = new List<string>()
        {
            "Bookshelf Symbol 7",
            "HoloLens MDL2 Assets",
            "Marlett",
            "MS Reference Specialty",
            "MT Extra",
            "Segoe MDL2 Assets",
            "Symbol",
            "Webdings",
            "Wingdings",
            "Wingdings 2",
            "Wingdings 3"
        };
    /// <summary>
    /// 是否是中文字体
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string IsChineseFont(string name)
    {
        foreach (var temp in list)
        {
            if (string.Equals(name.ToLower(), temp.En.ToLower()))
            {
                return temp.Zh;
            }
        }
        return null;
    }
    /// <summary>
    /// 是否是图标
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsTuBiao(string name)
    {
        foreach (var temp in ZiFuList)
        {
            if (string.Equals(name.ToLower(), temp.ToLower()))
            {
                return true;
            }
        }
        return false;
    }

}

public class EnZhName
{
    public string En;
    public string Zh;
}


    #endregion 

}
  