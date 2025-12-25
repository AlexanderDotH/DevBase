# DevBase.Test.Avalonia Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Test.Avalonia project.

## Table of Contents

- [Application Entry Point](#application-entry-point)
  - [Program](#program)
- [Application Classes](#application-classes)
  - [App](#app)
  - [MainWindow](#mainwindow)
- [XAML Views](#xaml-views)
  - [App.axaml](#appaxaml)
  - [MainWindow.axaml](#mainwindowaxaml)

## Application Entry Point

### Program

```csharp
/// <summary>
/// Main entry point for the Avalonia test application.
/// </summary>
class Program
{
    /// <summary>
    /// The main method that starts the application.
    /// Initialization code. Don't use any Avalonia, third-party APIs or any
    /// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    /// yet and stuff might break.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    [STAThread]
    public static void Main(string[] args)
    
    /// <summary>
    /// Configures and builds the Avalonia application.
    /// Avalonia configuration, don't remove; also used by visual designer.
    /// </summary>
    /// <returns>Configured AppBuilder instance.</returns>
    public static AppBuilder BuildAvaloniaApp()
}
```

## Application Classes

### App

```csharp
/// <summary>
/// The main application class for the Avalonia test application.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the application by loading XAML resources.
    /// </summary>
    public override void Initialize()
    
    /// <summary>
    /// Called when framework initialization is completed.
    /// Sets up the main window for desktop application lifetime.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
}
```

### MainWindow

```csharp
/// <summary>
/// The main window of the test application demonstrating color extraction from images.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    
    /// <summary>
    /// Handles the button click event to load and process an image.
    /// Randomly selects a PNG file from the OpenLyricsClient cache directory,
    /// extracts the dominant color using Lab color space clustering,
    /// and displays the image with its RGB color components.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The routed event arguments.</param>
    private void Button_OnClick(object? sender, RoutedEventArgs e)
}
```

## XAML Views

### App.axaml

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:themes="clr-namespace:Material.Styles.Themes;assembly=Material.Styles"
             x:Class="DevBase.Test.Avalonia.App">
    <Application.Styles>
        <!-- Applies Material Design theme with dark mode, purple primary, and lime secondary colors -->
        <themes:MaterialTheme BaseTheme="Dark" PrimaryColor="Purple" SecondaryColor="Lime" />
    </Application.Styles>
</Application>
```

### MainWindow.axaml

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d" d:DesignWidth="170" d:DesignHeight="170"
        Width="300"
        Height="170"
        x:Class="DevBase.Test.Avalonia.MainWindow"
        Title="DevBase.Test.Avalonia">
    
    <Grid>
        <!-- Load button to trigger image processing -->
        <Button Content="Load" 
                Margin="5,7,0,0"
                VerticalAlignment="Top" Click="Button_OnClick"></Button>
        
        <!-- Red color component display panel -->
        <Panel Name="Panel_RED"
               Width="50" 
               Height="50" 
               Background="Red" 
               VerticalAlignment="Top" 
               HorizontalAlignment="Left" 
               Margin="5,50,0,0"/>
        
        <!-- Green color component display panel -->
        <Panel Name="Panel_GREEN"
               Width="50" 
               Height="50" 
               Background="Green" 
               VerticalAlignment="Top" 
               HorizontalAlignment="Left" 
               Margin="60,50,0,0"/>
        
        <!-- Blue color component display panel -->
        <Panel Name="Panel_BLUE"
               Width="50" 
               Height="50" 
               Background="Blue" 
               VerticalAlignment="Top" 
               HorizontalAlignment="Left" 
               Margin="115,50,0,0"/>
        
        <!-- Combined color display panel -->
        <Panel Name="Panel_COLOR"
               Width="50" 
               Height="50" 
               Background="Pink" 
               VerticalAlignment="Top" 
               HorizontalAlignment="Left" 
               Margin="5,110,0,0"/>
        
        <!-- Image display control -->
        <Image Name="Image_DISPLAY"
               Width="50"
               Height="50"
               VerticalAlignment="Top"
               HorizontalAlignment="Left"
               Margin="60,110,0,0"/>
               
    </Grid>
</Window>
```

## Project Overview

The DevBase.Test.Avalonia project is a test application built with Avalonia UI framework that demonstrates:

1. **Color Extraction**: Uses the `LabClusterColorCalculator` from DevBase.Avalonia.Extension to extract dominant colors from images
2. **Image Processing**: Loads PNG files from a cache directory and displays them
3. **Color Visualization**: Shows the RGB components of the extracted color in separate panels
4. **Material Design**: Implements Material Design theme with dark mode styling

### Dependencies
- Avalonia UI framework
- Material.Styles for Material Design theming
- DevBase.Avalonia for color processing utilities
- DevBase.Avalonia.Extension for advanced color extraction algorithms
- DevBase.Generics for AList collection
- DevBase.IO for file operations

### Usage
1. Click the "Load" button to randomly select and process an image
2. The application displays the image and its dominant color
3. RGB components are shown in separate colored panels
4. The combined color is displayed in an additional panel
