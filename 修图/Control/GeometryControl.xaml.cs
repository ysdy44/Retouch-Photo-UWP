using System;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
using 修图.Model;
using Windows.UI.Xaml;


namespace 修图.Control
{
    public sealed partial class GeometryControl : UserControl
    {
        #region DependencyProperty：依赖属性


        //工具
        public int Tool
        {
            get { return (int)GetValue(ToolProperty); }
            set { SetValue(ToolProperty, value); }
        }

        public static readonly DependencyProperty ToolProperty =
            DependencyProperty.Register("Tool", typeof(int), typeof(GeometryControl), new PropertyMetadata(0, new PropertyChangedCallback(ToolOnChang)));

        private static void ToolOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GeometryControl Con = (GeometryControl)sender;

            int OldTool = (int)e.OldValue;
            int NewTool = (int)e.NewValue;

            if (NewTool >= 500 && NewTool < 600)
            {
                App.Tip(Con.GetTip(NewTool));//全局提示
                Con.ListView.SelectedIndex = NewTool - 500;

                if (OldTool >= 500 && OldTool < 600)
                {
                    //Undo：撤销
                    Undo undo = new Undo();
                    undo.ToolInstantiation(OldTool);
                    App.UndoAdd(undo);
                }
            }
        }

        #endregion

        public GeometryControl()
        {
            this.InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Model.Tool = ListView.SelectedIndex + 500;
        }


        private string GetTip(int Tool)
        {
            switch (Tool)
            {
                //Tool：工具
                //  case 500: return App.resourceLoader.GetString("/Tool/Hand_");
                case 500: return "Line";

                default: return string.Empty;
            }
        }
    }
}
