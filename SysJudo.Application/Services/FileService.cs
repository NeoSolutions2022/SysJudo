using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SysJudo.Application.Contracts;
using SysJudo.Core.Enums;
using SysJudo.Core.Extension;
using SysJudo.Core.Settings;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ClosedXML.Excel;

namespace SysJudo.Application.Services;

public class FileService : IFileService
{
    private readonly AppSettings _appSettings;
    private readonly UploadSettings _uploadSettings;

    public FileService(IOptions<AppSettings> appSettings, IOptions<UploadSettings> uploadSettings)
    {
        _appSettings = appSettings.Value;
        _uploadSettings = uploadSettings.Value;
    }

    // public async Task<string> Upload(IFormFile arquivo, EUploadPath uploadPath,
    //     EPathAccess pathAcess = EPathAccess.Private)
    // {
    //     var fileName = GenerateNewFileName(arquivo.FileName);
    //     var filePath = MountFilePath(fileName, pathAcess, uploadPath);
    //
    //     try
    //     {
    //         await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
    //     }
    //     catch (DirectoryNotFoundException)
    //     {
    //         var file = new FileInfo(filePath);
    //         file.Directory?.Create();
    //         await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
    //     }
    //
    //     return GetFileUrl(fileName, pathAcess, uploadPath);
    // }

    public async Task<string> Upload(IFormFile arquivo, EUploadPath uploadPath,
        EPathAccess pathAcess = EPathAccess.Private)
    {
        var connectionString = "DefaultEndpointsProtocol=https;AccountName=judofiles;AccountKey=lPNcfeFM2OjfrMGokU0tfs1ZjLEHcichRvjP0OC8loJzMiDMoY48tYwhJOjm49OCa4QrcpKx+6Pt+AStNHomjw==;EndpointSuffix=core.windows.net";
        
        var fileName = GenerateNewFileName(arquivo.FileName);
        BlobContainerClient container = new BlobContainerClient(connectionString, "agremicoes");
        BlobClient blob = container.GetBlobClient(fileName);
        await blob.UploadAsync(arquivo.OpenReadStream());

        return blob.Uri.AbsoluteUri;
    }
    
    public async Task<string> UploadExcel(XLWorkbook arquivo, EUploadPath uploadPath,
        EPathAccess pathAcess = EPathAccess.Private)
    {
        var connectionString = "DefaultEndpointsProtocol=https;AccountName=judofiles;AccountKey=lPNcfeFM2OjfrMGokU0tfs1ZjLEHcichRvjP0OC8loJzMiDMoY48tYwhJOjm49OCa4QrcpKx+6Pt+AStNHomjw==;EndpointSuffix=core.windows.net";
        using var ms = new MemoryStream();
        arquivo.SaveAs(ms);
        var fileName = GenerateNewFileName("AgremiacaoPlanilha.xlsx");
        BlobContainerClient container = new BlobContainerClient(connectionString, "agremicoes");
        BlobClient blob = container.GetBlobClient(fileName);
        var blobHttpHeader = new BlobHttpHeaders();

        blobHttpHeader.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        ms.Position = 0;
        await blob.UploadAsync(ms, blobHttpHeader);

        return blob.Uri.AbsoluteUri;
    }

    public bool Apagar(Uri uri)
    {
        try
        {
            var filePath = GetFilePath(uri);

            new FileInfo(filePath).Delete();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static string GenerateNewFileName(string fileName)
    {
        var newFileName = $"{Guid.NewGuid()}_{fileName}";
        newFileName = newFileName.Replace("-", "");

        return newFileName;
    }

    private string MountFilePath(string fileName, EPathAccess pathAccess, EUploadPath uploadPath)
    {
        var path = pathAccess == EPathAccess.Private ? _uploadSettings.PrivateBasePath : _uploadSettings.PublicBasePath;
        return Path.Combine(path, uploadPath.ToDescriptionString(), fileName);
    }

    private string GetFileUrl(string fileName, EPathAccess pathAccess, EUploadPath uploadPath)
    {
        return Path.Combine(_appSettings.UrlApi, pathAccess.ToDescriptionString(), uploadPath.ToDescriptionString(),
            fileName);
    }

    private string GetFilePath(Uri uri)
    {
        var filePath = uri.AbsolutePath;
        if (filePath.StartsWith("/"))
        {
            filePath = filePath.Remove(0, 1);
        }

        var pathAccessStr = filePath.Split("/")[1];
        var pathAccess = Enum.Parse<EPathAccess>(pathAccessStr, true);
        filePath = filePath.Remove(0, pathAccess.ToDescriptionString().Length);
        if (filePath.StartsWith("/"))
        {
            filePath = filePath.Remove(0, 1);
        }

        var basePath = pathAccess == EPathAccess.Private
            ? _uploadSettings.PrivateBasePath
            : _uploadSettings.PublicBasePath;

        return Path.Combine(basePath, filePath);
    }

    private static byte[] ConvertFileInByteArray(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}