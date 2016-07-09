using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace narmail.Controls
{
    public sealed partial class TabControl : UserControl
    {
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(TabControl), null);

        public string Glyph
        {
            get { return GetValue(GlyphProperty) as string; }
            set { SetValue(GlyphProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(TabControl), null);

        public string Label
        {
            get { return GetValue(LabelProperty) as string; }
            set { SetValue(LabelProperty, value); }
        }


        public TabControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
