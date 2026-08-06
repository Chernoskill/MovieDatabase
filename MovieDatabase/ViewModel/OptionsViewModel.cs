using CommunityToolkit.Maui.Storage;
using MovieDatabase.Localization;
using MovieDatabase.Services;

namespace MovieDatabase.ViewModel
{
    public partial class OptionsViewModel : BaseViewModel
    {
        readonly TagService tagService;
        readonly DataService dataService;
        readonly FileService fileService;
        readonly LocalizationResourceManager localization;

        public ObservableCollection<TagListItem> Tags { get; } = new();

        public string AppVersion => AppInfo.Current.VersionString;

        [ObservableProperty]
        public partial string? NewTagName { get; set; }

        [ObservableProperty]
        public partial int NewTagColorIndex { get; set; }

        [ObservableProperty]
        public partial AppLanguage SelectedLanguage { get; set; }

        public OptionsViewModel(TagService tagService, DataService dataService, FileService fileService, LocalizationResourceManager localization)
        {
            this.tagService = tagService;
            this.dataService = dataService;
            this.fileService = fileService;
            this.localization = localization;
            SelectedLanguage = localization.CurrentLanguage;
        }

        partial void OnSelectedLanguageChanged(AppLanguage value)
        {
            localization.SetLanguage(value);
        }

        public async Task LoadAsync()
        {
            List<Tag> tags = await tagService.GetTags();

            Tags.Clear();

            foreach (Tag tag in tags)
            {
                Tags.Add(new TagListItem(tag, SaveTagAsync, DeleteTagAsync));
            }
        }

        [RelayCommand]
        async Task AddTagAsync()
        {
            if (string.IsNullOrWhiteSpace(NewTagName))
            {
                return;
            }

            Tag tag = await tagService.AddTagAsync(NewTagName.Trim(), NewTagColorIndex);
            Tags.Add(new TagListItem(tag, SaveTagAsync, DeleteTagAsync));

            NewTagName = null;
            NewTagColorIndex = 0;
        }

        Task SaveTagAsync(TagListItem item, string name, int colorIndex)
        {
            return tagService.UpdateTagAsync(item.Tag, name, colorIndex);
        }

        async Task DeleteTagAsync(TagListItem item)
        {
            bool confirmed = await Shell.Current.DisplayAlertAsync(
                localization["OptionsPage_DeleteTagTitle"],
                string.Format(localization["OptionsPage_DeleteTagMessage"], item.Name),
                localization["Common_Delete"],
                localization["Common_Cancel"]);

            if (!confirmed)
            {
                return;
            }

            await tagService.DeleteTagAsync(item.Tag);
            Tags.Remove(item);
        }

        [RelayCommand]
        async Task ImportDataAsync()
        {
            string? filePath;

            try
            {
                filePath = await fileService.PickImportFilePathAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync(localization["OptionsPage_ImportFailedTitle"], ex.Message, localization["Common_OK"]);
                return;
            }

            if (filePath == null)
            {
                return;
            }

            try
            {
                await dataService.ImportAsync(filePath);
                await LoadAsync();
                await Shell.Current.DisplayAlertAsync(
                    localization["OptionsPage_ImportCompleteTitle"],
                    localization["OptionsPage_ImportCompleteMessage"],
                    localization["Common_OK"]);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync(localization["OptionsPage_ImportFailedTitle"], ex.Message, localization["Common_OK"]);
            }
        }

        [RelayCommand]
        async Task ExportDataAsync()
        {
            if (OperatingSystem.IsAndroid() && !OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                await Shell.Current.DisplayAlertAsync(
                    localization["OptionsPage_ExportNotSupportedTitle"],
                    localization["OptionsPage_ExportNotSupportedMessage"],
                    localization["Common_OK"]);
                return;
            }

            string initialDirectory = fileService.GetDefaultExportDirectory();

            // The Android-version guard above makes this safe; the platform-compatibility
            // analyzer can't infer that from the combined condition.
#pragma warning disable CA1416
            FolderPickerResult pickResult = await FolderPicker.Default.PickAsync(initialDirectory);
#pragma warning restore CA1416

            if (!pickResult.IsSuccessful || pickResult.Folder == null)
            {
                return;
            }

            try
            {
                await dataService.ExportAsync(pickResult.Folder.Path);
                await Shell.Current.DisplayAlertAsync(
                    localization["OptionsPage_ExportCompleteTitle"],
                    string.Format(localization["OptionsPage_ExportCompleteMessage"], pickResult.Folder.Path),
                    localization["Common_OK"]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to export movie database data: {ex}");
                await Shell.Current.DisplayAlertAsync(
                    localization["OptionsPage_ExportFailedTitle"],
                    localization["OptionsPage_ExportFailedMessage"],
                    localization["Common_OK"]);
            }
        }
    }
}
