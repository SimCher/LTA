using System.IO;
using System.Threading.Tasks;

namespace LTA.Mobile.Interfaces;

public interface IPhotoPickerService
{
    Task<Stream> GetImageStreamAsync();
}