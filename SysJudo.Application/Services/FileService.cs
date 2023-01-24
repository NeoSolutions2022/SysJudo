using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SysJudo.Application.Contracts;
using SysJudo.Core.Enums;
using SysJudo.Core.Extension;
using SysJudo.Core.Settings;

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

    public async Task<string> Upload(IFormFile arquivo, EUploadPath uploadPath,
        EPathAccess pathAcess = EPathAccess.Private)
    {
        var fileName = GenerateNewFileName(arquivo.FileName);
        var filePath = MountFilePath(fileName, pathAcess, uploadPath);

        try
        {
            await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
        }
        catch (DirectoryNotFoundException)
        {
            var file = new FileInfo(filePath);
            file.Directory?.Create();
            await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
        }

        return GetFileUrl(fileName, pathAcess, uploadPath);
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
        var newFileName = $"{Guid.NewGuid()}_{fileName}".ToLower();
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