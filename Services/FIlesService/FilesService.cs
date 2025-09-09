using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorAvalonia.Services.FIlesService
{
    public class FilesService : IFilesService
    {
        private readonly Window _target;

        private FilePickerFileType _xmlOnly { get; } = new(".xml file")
        {
            Patterns = new[] { "*.xml" }
        };

        public FilesService()
        {
            _target = new Window();
        }

        public async Task<IStorageFile?> OpenFileAsync()
        {
            var files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open .xml File",
                FileTypeFilter = new[] { _xmlOnly },
                AllowMultiple = false
            });

            return files.Count >= 1 ? files[0] : null;
        }

        public async Task<IStorageFile?> SaveFileAsync()
        {
            return await _target.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Save .xml File",
                FileTypeChoices = new[] { _xmlOnly }
            });
        }
    }
}
