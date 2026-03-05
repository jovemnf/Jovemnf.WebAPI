using System;
using System.Net;

namespace Jovemnf.WebApi.Internal;

public class WebApiResponse<T>
{
    public T? Content { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public Exception? Exception { get; set; }
}