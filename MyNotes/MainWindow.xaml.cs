﻿using System;
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
        private bool deleting = false;

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
                    if (!(c is CheckBox) || !(c as CheckBox).IsChecked.Value)
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

            pin.IsChecked = this.Topmost = element.Attribute("topmost", this.Topmost);
            pin.Visibility = pin.IsChecked.Value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!deleting && !(App.Current as App).AllowClose)
                e.Cancel = true;
            base.OnClosing(e);
        }

        void textbox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MinHeight = (sender as TextBox).Margin.Top + e.NewSize.Height + 10;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            element.SetAttributeValue("deleted", true);
            this.deleting = true;
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
        private void Pin_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            element.SetAttributeValue("topmost", true);
        }
        private void Pin_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            element.SetAttributeValue("topmost", false);
        }
    }
}
