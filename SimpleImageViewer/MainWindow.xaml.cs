using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleImageViewer;

public partial class MainWindow : Window
{
    string[] imageTypes = new[] { ".jpg", ".png", ".bmp", ".gif" };
    string[]? imageFiles;
    int currentImage = 0;

    CancellationTokenSource? loadCts;

    public MainWindow()
    {
        InitializeComponent();

        string[] args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            // start loading without blocking constructor
            _ = LoadFromStartAsync(args);
        }
    }

    private Task LoadFromStartAsync(string[] args)
    {
        DirectoryInfo dir = new(args[1]);

        return LoadImagesAsync(dir);
    }

    private async Task LoadImagesAsync(DirectoryInfo dir)
    {
        loadCts?.Cancel();
        loadCts = new CancellationTokenSource();
        var token = loadCts.Token;

        try
        {
            // show loading overlay
            foundCountText.Text = "Found: 0";
            loadingOverlay.Visibility = Visibility.Visible;

            // enumerate files on background thread; support cancellation
            var files = await Task.Run(() =>
            {
                var list = new List<string>();
                foreach (var f in dir.GetFiles())
                {
                    token.ThrowIfCancellationRequested();
                    var ext = f.Extension.ToLower();
                    if (imageTypes.Contains(ext))
                    {
                        list.Add(f.FullName);
                        // optionally report progress by updating the UI periodically
                    }
                }

                list.Sort();
                return list.ToArray();
            }, token);

            if (token.IsCancellationRequested)
                return;

            imageFiles = files;
            currentImage = -1;

            // update count on UI thread
            foundCountText.Text = $"Found: {imageFiles.Length}";

            // show first image if any
            if (imageFiles.Length > 0)
            {
                MoveNextImage();
            }
        }
        catch (OperationCanceledException)
        {
            // cancelled by user; clear partial results
            imageFiles = Array.Empty<string>();
            currentImage = -1;
        }
        finally
        {
            // hide loading overlay
            loadingOverlay.Visibility = Visibility.Collapsed;
            loadCts = null;
        }
    }

    private void CancelLoad_Click(object sender, RoutedEventArgs e)
    {
        loadCts?.Cancel();
    }

    private void MoveNextImage()
    {
        if (imageFiles?.Length > 0)
        {
            if (currentImage >= imageFiles.Length - 1)
            {
                currentImage = 0;
            }
            else
            {
                currentImage += 1;
            }

            SetImageSourceSafe(imageFiles[currentImage]);
        }
    }
    private void MovePreviousImage()
    {
        if (imageFiles?.Length > 0)
        {
            if (currentImage < 1)
            {
                currentImage = imageFiles.Length - 1;
            }
            else
            {
                currentImage -= 1;
            }
            SetImageSourceSafe(imageFiles[currentImage]);
        }
    }

    private void SetImageSourceSafe(string path)
    {
        try
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();

            screenImage.Source = bmp;
        }
        catch
        {
            // ignore failed image and try next
            // try to remove or skip the file silently
        }
    }

    private async void MenuItem_LoadDirectory_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new();
        dlg.Filter = "Images |*.png;*.jpg;*.bmp;*.gif";

        bool? result = dlg.ShowDialog();

        if (result == true)
        {
            string? folderPath = Path.GetDirectoryName(dlg.FileName);

            if (folderPath is null)
            {
                return;
            }

            DirectoryInfo dir = new(folderPath);

            await LoadImagesAsync(dir);
        }
    }

    private void MenuItem_ThemeLight_Click(object sender, RoutedEventArgs e)
    {
        ApplyTheme("Themes/Light.xaml");
    }

    private void MenuItem_ThemeDark_Click(object sender, RoutedEventArgs e)
    {
        // load the updated dark theme file
        ApplyTheme("Themes/DarkUpdated.xaml");
    }

    private void ApplyTheme(string resourcePath)
    {
        try
        {
            var rd = new ResourceDictionary() { Source = new Uri(resourcePath, UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(rd);

            // If the loaded theme does not define TextBrush, ensure a high-contrast value
            if (!Application.Current.Resources.Contains("TextBrush"))
            {
                Application.Current.Resources["TextBrush"] = new SolidColorBrush(Colors.White);
            }
        }
        catch
        {
            // ignore theme load errors
        }
    }

    private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Left:
                MovePreviousImage();
                break;
            case Key.Right:
                MoveNextImage();
                break;
            case Key.Escape:
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else
                {
                    Close();
                }
                break;
            default:
                break;
        }
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
        else
        {
            WindowState = WindowState.Maximized;
        }
    }
}