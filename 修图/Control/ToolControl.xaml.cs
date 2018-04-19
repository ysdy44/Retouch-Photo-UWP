using System;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
using 修图.Model;
using Windows.UI.Xaml;

namespace 修图.Control
{
    public sealed partial class ToolControl : UserControl
    {


        #region DependencyProperty：依赖属性


        //工具
        public int Tool
        {
            get { return (int)GetValue(ToolProperty); }
            set { SetValue(ToolProperty, value); }
        }

        public static readonly DependencyProperty ToolProperty =
            DependencyProperty.Register("Tool", typeof(int), typeof(ToolControl), new PropertyMetadata(0, new PropertyChangedCallback(ToolOnChang)));

        private static void ToolOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ToolControl Con = (ToolControl)sender;

            int OldTool =(int)e.OldValue; 
            int NewTool = (int)e.NewValue;

            if (NewTool >= 0 && NewTool < 16)
            {
                if (OldTool >= 0 && OldTool < 16)
                {
                    //Undo：撤销
                    Undo undo = new Undo();
                    undo.ToolInstantiation(OldTool);
                    App.UndoAdd(undo);

                     Con.ListView.SelectedIndex = NewTool;
                }
            }
        }

        #endregion

        public ToolControl()
        {
            this.InitializeComponent();
         }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        App.Model.Tool= ListView.SelectedIndex;
        }
 
     }
}
