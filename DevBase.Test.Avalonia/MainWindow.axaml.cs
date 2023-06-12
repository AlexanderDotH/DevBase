using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Image;
using DevBase.Generics;
using DevBase.IO;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Color = Avalonia.Media.Color;
using Image = Avalonia.Controls.Image;

namespace DevBase.Test.Avalonia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                AList<AFileObject> files = AFile.GetFiles($"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\OpenLyricsClient\\Cache", true, filter:"*.png");
                AFileObject file = files.Get(new Random().Next(0, files.Length - 1));
                Console.WriteLine(file.FileInfo.FullName);

                Stream s = new MemoryStream(file.BinaryData);
                IBitmap map = new Bitmap(s);

                Color c = new ClusterColorCalculator().GetColorFromBitmap(map);

                Panel red = this.Find<Panel>(nameof(DevBase.Test.Avalonia.MainWindow.Panel_RED));
                Panel green = this.Find<Panel>(nameof(DevBase.Test.Avalonia.MainWindow.Panel_GREEN));
                Panel blue = this.Find<Panel>(nameof(DevBase.Test.Avalonia.MainWindow.Panel_BLUE));
                Panel color = this.Find<Panel>(nameof(DevBase.Test.Avalonia.MainWindow.Panel_COLOR));
                Image image = this.Find<Image>(nameof(DevBase.Test.Avalonia.MainWindow.Image_DISPLAY));

                image.Source = map;
            
                red.Background = new SolidColorBrush(new Color(255, c.R, 0, 0));
                green.Background = new SolidColorBrush(new Color(255, 0, c.G, 0));
                blue.Background = new SolidColorBrush(new Color(255, 0, 0, c.B));
                color.Background = new SolidColorBrush(new Color(255, c.R, c.G, c.B));
            }
            catch (System.Exception exception)
            {
            }
        }
    }
}