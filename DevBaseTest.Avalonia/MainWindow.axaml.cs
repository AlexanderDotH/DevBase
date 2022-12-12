using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using DevBase.Generic;
using DevBase.IO;
using DevBaseColor.Image;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Color = Avalonia.Media.Color;
using Image = Avalonia.Controls.Image;

namespace DevBaseTest.Avalonia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            GenericList<AFileObject> files = AFile.GetFiles("C:\\Users\\Alex\\RiderProjects\\DevBase\\DevBaseTests\\DevBaseColorData", true, filter:"*.png");
            AFileObject file = files.Get(new Random().Next(0, files.Length - 1));
            Console.WriteLine(file.FileInfo.FullName);

            Stream s = new MemoryStream(file.BinaryData);
            IBitmap map = new Bitmap(s);

            Color c = new GroupColorCalculator().GetColorFromBitmap(map);

            Panel red = this.Find<Panel>(nameof(Panel_RED));
            Panel green = this.Find<Panel>(nameof(Panel_GREEN));
            Panel blue = this.Find<Panel>(nameof(Panel_BLUE));
            Panel color = this.Find<Panel>(nameof(Panel_COLOR));
            Image image = this.Find<Image>(nameof(Image_DISPLAY));

            image.Source = map;
            
            red.Background = new SolidColorBrush(new Color(255, c.R, 0, 0));
            green.Background = new SolidColorBrush(new Color(255, 0, c.G, 0));
            blue.Background = new SolidColorBrush(new Color(255, 0, 0, c.B));
            color.Background = new SolidColorBrush(new Color(255, c.R, c.G, c.B));
        }
    }
}