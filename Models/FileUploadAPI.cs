namespace NaviConnectWebApi.Models;

public class FileUploadApi
{
    public FileUploadApi(IFormFile file)
    {
        File = file;
    }

    public IFormFile File { get; set; }
}