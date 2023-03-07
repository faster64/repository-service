using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using MediatR;
using ServiceStack;
using ServiceStack.Messaging;

namespace KiotVietTimeSheet.Application.Commands.ImportExport
{
    public class ImportCommissionCommandHandler : BaseCommandHandler,
        IRequestHandler<ImportCommissionCommand, ImportExportFileDto>
    {
        private readonly IMessageFactory _messageFactory;
        private readonly IRetailDbService _retailDbService;
        private readonly IImportExportService _importExportService;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public ImportCommissionCommandHandler(
            IEventDispatcher eventDispatcher,
            IMessageFactory messageFactory,
            IRetailDbService retailDbService,
            IImportExportService importExportService,
            IApplicationConfiguration applicationConfiguration)
            : base(eventDispatcher)
        {
            _messageFactory = messageFactory;
            _retailDbService = retailDbService;
            _importExportService = importExportService;
            _applicationConfiguration = applicationConfiguration;
        }

        public async Task<ImportExportFileDto> Handle(ImportCommissionCommand request, CancellationToken cancellationToken)
        {
            var eventType = "CommissionProductList";
            var hasDuplicateTask = await _retailDbService.HasDuplicateTaskImportBySessionId(request.KvSessionId, eventType);
            if (hasDuplicateTask)
            {
                NotifyValidationErrors(typeof(ExportCommand), new List<string> { "Yêu cầu này đang được xử lý. Vui lòng kiểm tra trong nội dung Xử lý import, xuất file" });
                return null;
            }
            
            string[] extensions = { ".xlsx" };
            if (request.Files == null)
            {
                NotifyValidationErrors(typeof(ExportCommand), new List<string> { "Tệp tin không đúng định dạng, vui lòng kiểm tra lại." });
                return null;
            }

            foreach (var file in request.Files)
            {
                var fileName = Path.GetFileName(file.FileName);

                if (fileName == null) continue;

                if (file.ContentLength > _applicationConfiguration.MaxUploadFileSize)
                {
                    NotifyValidationErrors(typeof(ExportCommand), new List<string> {$"Dung lượng file không được lớn quá {(_applicationConfiguration.MaxUploadFileSize / 1048576) + " MB"}."});
                    return null;
                }

                if (!extensions.Any(x => x.Equals(Path.GetExtension(fileName.ToLower()), StringComparison.OrdinalIgnoreCase)))
                {
                    NotifyValidationErrors(typeof(ExportCommand), new List<string> { "Hệ thống hỗ trợ duy nhất định dạng xlsx(Excel 2007, 2010...)." });
                    return null;
                }

                using (var ftpClient = new FtpClient(_applicationConfiguration.Ftp.Host, _applicationConfiguration.Ftp.Port, _applicationConfiguration.Ftp.UserName, _applicationConfiguration.Ftp.Password))
                {
                    try
                    {
                        await ftpClient.ConnectAsync(cancellationToken);
                    }
                    catch
                    {
                        NotifyValidationErrors(typeof(ExportCommand), new List<string> { "Không thể kết nối đến máy chủ FTP" });
                        return null;
                    }

                    var importExportExecutionContext = await _importExportService.GetExecutionContext();
                    var importExportSession = await _importExportService.GetSession(request.KVSession);

                    ftpClient.RetryAttempts = 3;
                    var directoryPath = $"{_applicationConfiguration.Ftp.Directory}/{DateTime.Today:dd-MM-yyyy}/{importExportSession.CurrentRetailerCode}";
                    var newFileNamePath = $"{Guid.NewGuid()}{Path.GetExtension(fileName.ToLower())}";
                    var filePath = $"{directoryPath}/{newFileNamePath}";

                    if (!(await ftpClient.DirectoryExistsAsync(directoryPath, cancellationToken)))
                    {
                        await ftpClient.CreateDirectoryAsync(directoryPath, cancellationToken);
                    }

                    var isUploaded = await ftpClient.UploadAsync(file.InputStream.ToBytes(), filePath, FtpRemoteExists.Overwrite, token: cancellationToken) == FtpStatus.Success;
                    if (!isUploaded) continue;

                    await ftpClient.DisconnectAsync(cancellationToken);

                    using (var mqClient = _messageFactory.CreateMessageQueueClient())
                    {
                        var importExportFile = new ImportExportFile
                        {
                            EventType = eventType,
                            IsImport = true,
                            KvSessionId = request.KvSessionId,
                            FileName = fileName,
                            FilePath = filePath,
                            Status = (byte)ImportExportStatus.Processing
                        };

                        var result = await _retailDbService.AddImportExportFile(importExportFile);
                        var data = request.ConvertTo<TsImportRequestObject>();
                        data.__type = "KiotViet.TimeSheet.ImportExport.Utilities.Mq.TsImportRequestObject, KiotViet.TimeSheet.ImportExport.Utilities.Mq";
                        data.ExecutionContext = importExportExecutionContext;
                        data.KVSession = importExportSession;
                        data.FileImport = importExportFile;
                        data.Params = new object[] {request.BearerToken};
                        mqClient.Publish(data);
                        return result.ConvertTo<ImportExportFileDto>();
                    }
                }
            }

            return null;
        }

        public class TsImportRequestObject
        {
            public string __type { get; set; }
            public string Type { get; set; }
            public ImportExportFile FileImport { get; set; }
            public ImportExportSession KVSession { get; set; }
            public ImportExportExecutionContext ExecutionContext { get; set; }
            public object[] Params { get; set; }
        }
    }
}

