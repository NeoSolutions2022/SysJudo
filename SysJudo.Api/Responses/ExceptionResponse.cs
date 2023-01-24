using System.Net;

namespace SysJudo.Api.Responses;

public class ExceptionResponse : Response
{
    public ExceptionResponse()
    {
        Title = "Ops, ocorreu um erro no servidor";
        Status = (int)HttpStatusCode.InternalServerError;
    }

    public ExceptionResponse(string title) : this()
    {
        Title = title;
    }
}