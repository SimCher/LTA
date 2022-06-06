using System.IO;
using System.Windows.Input;
using LTA.Mobile.Application.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace LTA.Mobile.PageModels;

public class TestPageModel : BasePageModel
{
    private ImageSource _image;
    private byte[] _bytes;

    public byte[] Bytes
    {
        get => _bytes;
        set => SetProperty(ref _bytes, value);
    }

    public ImageSource Image
    {
        get
        {
            if (Bytes != null)
            {
                return ImageSource.FromStream(() => new MemoryStream(Bytes));
            }

            return null;
        }
        set => SetProperty(ref _image, value);
    }
    public ICommand PickPhotoCommand {get;}
    public IPageDialogService PageDialog;

    public TestPageModel(IPageDialogService dialogService, INavigationService navService) : base(navService, new ChatService())
    {
        PageDialog = dialogService;
        
        PickPhotoCommand = new DelegateCommand(OnPickPhotoButtonClicked);
    }
    private async void OnPickPhotoButtonClicked()
    {
        await CrossMedia.Current.Initialize();

        if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
        {
            await PageDialog.DisplayAlertAsync("Not supported",
                "Your device does not currently support this functionality", "OK");
            return;
        }

        var mediaOptions = new PickMediaOptions()
        {
            PhotoSize = PhotoSize.Medium
        };

        var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

        if (selectedImageFile == null)
        {
            await PageDialog.DisplayAlertAsync("Error", "Could not get the image, please try again.", "OK");
            return;
        }

        using (var memory = new MemoryStream())
        {
            var stream = selectedImageFile.GetStream();
            await stream.CopyToAsync(memory);
            Bytes = memory.ToArray();
        }

        var anotherMemory = new MemoryStream(Bytes);
        
            Image = ImageSource.FromStream(() => anotherMemory);
        
    }
}