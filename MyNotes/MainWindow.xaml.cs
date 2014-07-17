using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MyNotes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XElement element;

        public MainWindow()
        {
            InitializeComponent();
            textbox.Focus();

            foreach (UIElement c in topbar.Children)
                c.Visibility = System.Windows.Visibility.Hidden;

            Action update = () =>
            {
                var visibility = topbar.IsMouseOver ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                foreach (UIElement c in topbar.Children)
                    c.Visibility = visibility;
            };
            this.MouseMove += (s, e) => update();
            this.MouseLeave += (s, e) => update();
            this.MouseLeftButtonDown += (s, e) => DragMove();
        }

        public MainWindow(XElement element)
            : this()
        {
            this.element = element;

            var bounds = element.Element("bounds");
            if (bounds != null)
            {
                this.Left = bounds.Element("left", this.Left);
                this.Top = bounds.Element("top", this.Top);
                this.Width = bounds.Element("width", this.Width);
                this.Height = bounds.Element("height", this.Height);
            }

            this.Topmost = element.Attribute("topmost", this.Topmost);
            this.textbox.Text = element.Element("text", this.textbox.Text);

            this.textbox.TextChanged += (s, e) => element.SetElementValue("text", textbox.Text);
            this.SizeChanged += (s, e) =>
            {
                var b = element.Element("bounds", true);
                b.SetElementValue("width", Width);
                b.SetElementValue("height", Height);
            };
            this.LocationChanged += (s, e) =>
            {
                var b = element.Element("bounds", true);
                b.SetElementValue("left", this.Left);
                b.SetElementValue("top", this.Top);
            };
        }

        void textbox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MinHeight = (sender as TextBox).Margin.Top + e.NewSize.Height + 10;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            element.SetAttributeValue("deleted", true);
            this.Close();
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            var ele = new XElement("note");
            element.Parent.Add(ele);

            MainWindow mw = new MainWindow(ele);
            mw.Width = this.Width;
            mw.Height = this.Height;
            mw.Show();
        }
    }
}
