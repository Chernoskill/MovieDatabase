#if WINDOWS
using Windows.Storage.Pickers;
using WinRT.Interop;
#endif

namespace MovieDatabase.Services
{
    public class FileService
    {
        public async Task SaveFileAsync(string fileName, string contents)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            await File.WriteAllTextAsync(filePath, contents);
        }

        public async Task<string?> LoadFileAsync(string fileName)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            if (!File.Exists(filePath))
            {
                return null;
            }

            return await File.ReadAllTextAsync(filePath);
        }

        public async Task ExportFileAsync(string directoryPath, string fileName, string contents)
        {
            string filePath = Path.Combine(directoryPath, fileName);
            await File.WriteAllTextAsync(filePath, contents);
        }

        public string GetDefaultExportDirectory()
        {
#if ANDROID
            return global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryDocuments)?.AbsolutePath
                ?? FileSystem.AppDataDirectory;
#elif WINDOWS
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#else
            return FileSystem.AppDataDirectory;
#endif
        }

        public async Task<string?> PickImportFilePathAsync()
        {
#if WINDOWS
            FileOpenPicker picker = new()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".json");

            object? platformWindow = Microsoft.Maui.Controls.Application.Current?.Windows[0].Handler?.PlatformView;

            if (platformWindow != null)
            {
                nint hwnd = WindowNative.GetWindowHandle(platformWindow);
                InitializeWithWindow.Initialize(picker, hwnd);
            }

            Windows.Storage.StorageFile? file = await picker.PickSingleFileAsync();
            return file?.Path;
#else
            FileResult? result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a movie database JSON file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/json" } }
                })
            });

            return result?.FullPath;
#endif
        }
    }
}
