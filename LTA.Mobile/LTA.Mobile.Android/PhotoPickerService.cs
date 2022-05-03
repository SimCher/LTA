using System.IO;
using System.Threading.Tasks;
using Android.Content;
using LTA.Mobile.Droid;
using LTA.Mobile.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhotoPickerService))]
namespace LTA.Mobile.Droid
{
    public class PhotoPickerService : IPhotoPickerService
    {
        public async Task<Stream> GetImageStreamAsync()
        {
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            MainActivity.Instance.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                MainActivity.PickImageId);

            MainActivity.Instance.PickImageTaskCompletionSource = new TaskCompletionSource<Stream>();

            return await MainActivity.Instance.PickImageTaskCompletionSource.Task;
        }
    }
}