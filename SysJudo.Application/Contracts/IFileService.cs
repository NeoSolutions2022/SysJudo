using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SysJudo.Core.Enums;

namespace SysJudo.Application.Contracts;

public interface IFileService
{
    Task<string> Upload(IFormFile arquivo, EUploadPath uploadPath, EPathAccess pathAcess = EPathAccess.Private);

    Task<string> UploadExcel(XLWorkbook arquivo, EUploadPath uploadPath,
        EPathAccess pathAcess = EPathAccess.Private);
    bool Apagar(Uri uri);
}