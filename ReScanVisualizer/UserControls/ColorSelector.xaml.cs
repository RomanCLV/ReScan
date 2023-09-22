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

namespace ReScanVisualizer.UserControls
{
    /// <summary>
    /// Logique d'interaction pour ColorSelector.xaml
    /// </summary>
    public partial class ColorSelector : UserControl
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorSelector),
                new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorChanged));

        public static readonly DependencyProperty ColorRedProperty =
            DependencyProperty.Register("ColorRed", typeof(byte), typeof(ColorSelector),
                new FrameworkPropertyMetadata((byte)255, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorComponentChanged));

        public static readonly DependencyProperty ColorGreenProperty =
            DependencyProperty.Register("ColorGreen", typeof(byte), typeof(ColorSelector),
                new FrameworkPropertyMetadata((byte)255, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorComponentChanged));

        public static readonly DependencyProperty ColorBlueProperty =
            DependencyProperty.Register("ColorBlue", typeof(byte), typeof(ColorSelector),
                new FrameworkPropertyMetadata((byte)255, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorComponentChanged));

        public static readonly DependencyProperty ColorAlphaProperty =
            DependencyProperty.Register("ColorAlpha", typeof(byte), typeof(ColorSelector),
                new FrameworkPropertyMetadata((byte)255, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorComponentChanged));

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public byte ColorRed
        {
            get { return (byte)GetValue(ColorRedProperty); }
            set { SetValue(ColorRedProperty, value); }
        }

        public byte ColorGreen
        {
            get { return (byte)GetValue(ColorGreenProperty); }
            set { SetValue(ColorGreenProperty, value); }
        }

        public byte ColorBlue
        {
            get { return (byte)GetValue(ColorBlueProperty); }
            set { SetValue(ColorBlueProperty, value); }
        }

        public byte ColorAlpha
        {
            get { return (byte)GetValue(ColorAlphaProperty); }
            set { SetValue(ColorAlphaProperty, value); }
        }

        public List<KeyValuePair<string, Color>> AllColors { get; private set; }

        public ColorSelector()
        {
            AllColors = GenerateColorList();
            DataContext = this;
            InitializeComponent();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            UpdateComboBoxSelection();
        }

        private List<KeyValuePair<string, Color>> GenerateColorList()
        {
            List<KeyValuePair<string, Color>> colorList = new List<KeyValuePair<string, Color>>();
            foreach (var property in typeof(Colors).GetProperties())
            {
                if (property.PropertyType == typeof(Color))
                {
                    Color color = (Color)property.GetValue(null);
                    colorList.Add(new KeyValuePair<string, Color>(property.Name, color));
                }
            }
            return colorList;
        }

        private void UpdateColor()
        {
            Color = Color.FromArgb(ColorAlpha, ColorRed, ColorGreen, ColorBlue);
            UpdateComboBoxSelection();
        }

        private void UpdateComboBoxSelection()
        {
            Color selectedColor = Color.FromRgb(ColorRed, ColorGreen, ColorBlue);
            foreach (KeyValuePair<string, Color> item in ColorComboBox.Items)
            {
                if (item.Value == selectedColor)
                {
                    ColorComboBox.SelectedItem = item;
                    return;
                }
            }
            ColorComboBox.SelectedItem = null;
        }

        private static void OnColorComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorSelector selector = (ColorSelector)d;
            selector.UpdateColor();
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorSelector selector = (ColorSelector)d;
            Color newColor = (Color)e.NewValue;
            selector.ColorRed = newColor.R;
            selector.ColorGreen = newColor.G;
            selector.ColorBlue = newColor.B;
            selector.ColorAlpha = newColor.A;
        }
    }
}
