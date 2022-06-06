using LTA.API.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace LTA.API.Pages;

public class APKDownload : PageModel
{
    public List<FileModel> Files { get; set; }
    private IHostingEnvironment Environment;

    public APKDownload(IHostingEnvironment environment)
    {
        Environment = environment;
    }
    public void OnGet()
    {
        var filePaths = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Storage\\");

        Files = new List<FileModel>();

        foreach (var filePath in filePaths)
        {
            Files.Add(new FileModel {FileName = Path.GetFileName(filePath)});
        }
    }

    public FileResult OnGetDownloadFile(string fileName)
    {
        var path = Directory.GetCurrentDirectory() + "\\Storage\\" + fileName;

        var bytes = System.IO.File.ReadAllBytes(path);

        return File(bytes, "application/octet-stream", fileName);
    }
}