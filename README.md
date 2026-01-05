SimpleImageViewer

A minimal WPF image viewer targeting .NET 10. Displays images in a borderless, centered window with simple navigation and a small context menu.

Project analysis

- UI: `MainWindow.xaml` defines a borderless window with the following event handlers wired in XAML:
  - `Window_KeyDown`, `Window_MouseDown`, `Window_MouseDoubleClick`
  - A `ContextMenu` with `Load Directory...` and `Exit` items
  - Primary display element: `Image` named `screenImage`

- Logic: `MainWindow.xaml.cs` implements loading images from a directory, next/previous navigation, and window controls.

Code issues and recommended fixes

1. Fix array initializer and add LINQ using

- Problem: `string[] imageTypes = [".jpg", ".png", ".bmp", ".gif"];` is invalid C#.
- Fix: `string[] imageTypes = new[] { ".jpg", ".png", ".bmp", ".gif" };` and add `using System.Linq;` at the top of `MainWindow.xaml.cs`.

2. Initialize and index logic

- Current behavior sets `currentImage = imageFiles.Length;` before calling `MoveNextImage()`. This relies on wrap logic and is error-prone.
- Recommendation: set `currentImage = -1;` in `LoadImages` and then call `MoveNextImage()` so the first image shown is index 0.

3. Robust image loading

- Use `BitmapImage.BeginInit()`/`EndInit()` with `CacheOption = BitmapCacheOption.OnLoad` then call `Freeze()` to release file handles immediately:

```csharp
var bmp = new BitmapImage();
bmp.BeginInit();
bmp.UriSource = new Uri(path, UriKind.Absolute);
bmp.CacheOption = BitmapCacheOption.OnLoad;
bmp.EndInit();
bmp.Freeze();
screenImage.Source = bmp;
```

- Consider `DecodePixelWidth`/`DecodePixelHeight` to limit memory for large images.
- Wrap image creation in try/catch to skip corrupted files.

4. Directory selection UX

- The code uses `OpenFileDialog` to select an image file and then loads other images from that file's folder. Consider using `System.Windows.Forms.FolderBrowserDialog` if you want to choose a directory directly.

5. Null-safety and bounds checks

- Ensure `imageFiles` is not null before accessing and check array lengths when updating `currentImage`.

Build and run

Prerequisites:
- .NET 10 SDK
- Visual Studio 2026 or any compatible IDE

From the repository root:

```
dotnet run --project SimpleImageViewer
```

Usage

- Right-click to open the context menu: `Load Directory...` (select an image file; the app loads the containing folder) or `Exit`.
- Left/Right arrow keys navigate images.
- Escape closes the app or exits maximized state.
- Double-click toggles maximized state. Click-and-drag moves the window.

Theme

- The project supports Light and Dark UI themes via WPF resource dictionaries placed in a `Themes/` folder (for example, `Themes/Light.xaml`, `Themes/Dark.xaml`).
- Runtime switch example (add to `MainWindow.xaml.cs`):

```csharp
var themeUri = new Uri("/Themes/Dark.xaml", UriKind.Relative);
var rd = (ResourceDictionary)Application.LoadComponent(themeUri);
Application.Current.Resources.MergedDictionaries.Clear();
Application.Current.Resources.MergedDictionaries.Add(rd);
```

Project structure

- `SimpleImageViewer/SimpleImageViewer.csproj`
- `SimpleImageViewer/MainWindow.xaml` — UI markup
- `SimpleImageViewer/MainWindow.xaml.cs` — code-behind (image loading, navigation, window controls)
- `SimpleImageViewer/App.xaml` and `SimpleImageViewer/App.xaml.cs` — application entry and resources
- `SimpleImageViewer/AssemblyInfo.cs`
- `Themes/` — recommended location for theme resource dictionaries (create if not present)
- `CONTRIBUTING.md` — contribution guidelines

Contributing

See `CONTRIBUTING.md` for contribution workflow, coding guidelines, and PR expectations.

License

Add a `LICENSE` file (for example, MIT) and specify the license here.
