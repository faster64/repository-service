using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/files/upload",
        "POST",
        Summary = "Upload files",
        Notes = "")
    ]
    public class FileUploadReq : IReturn<object>
    {

    }
}
