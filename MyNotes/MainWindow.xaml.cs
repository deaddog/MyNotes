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

            var barButtons = from UIElement e in topbar.Children where e is Button select e as Button;
            foreach (var b in barButtons)
                b.Visibility = System.Windows.Visibility.Hidden;

            Action update = () =>
            {
                var visi = topbar.IsMouseOver ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                foreach (var b in barButtons)
                    b.Visibility = visi;
            };
            this.MouseMove += (s, e) => update();
            this.MouseLeave += (s, e) => update();
            this.MouseLeftButtonDown += (s, e) => DragMove();
        }

        public MainWindow(XElement element)
            : this()
        {
            this.element = element;

            this.textbox.Text = element.Element("text", this.textbox.Text);
        }

        void textbox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MinHeight = (sender as TextBox).Margin.Top + e.NewSize.Height + 10;
        }
    }
}
