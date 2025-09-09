using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace CalculatorAvalonia.Services.FIlesService
{
    public interface IFilesService
    {
        public Task<IStorageFile?> OpenFileAsync();
        public Task<IStorageFile?> SaveFileAsync();
    }
}
