using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using MediatR;
using ServiceStack;
using ServiceStack.Messaging;

namespace KiotVietTimeSheet.Application.Commands.ImportExport
{
    public class ExportCommandHandler : BaseCommandHandler,
        IRequestHandler<ExportCommand, ImportExportFileDto>
    {
        private readonly IMessageFactory _messageFactory;
        private readonly IRetailDbService _retailDbService;
        private readonly IImportExportService _importExportService;
        private readonly IApplicationConfiguration _configuration;
        private readonly IKiotVietServiceClient _serviceClient;

        public ExportCommandHandler(
            IEventDispatcher eventDispatcher,
            IMessageFactory messageFactory,
            IRetailDbService retailDbService,
            IImportExportService importExportService,
            IApplicationConfiguration configuration,
            IKiotVietServiceClient serviceClient)
            : base(eventDispatcher)
        {
            _messageFactory = messageFactory;
            _retailDbService = retailDbService;
            _importExportService = importExportService;
            _configuration = configuration;
            _serviceClient = serviceClient;
        }

        public async Task<ImportExportFileDto> Handle(ExportCommand request, CancellationToken cancellationToken)
        {
            var hasDuplicateTask = await _retailDbService.HasDuplicateTaskExportBySessionId(request.KvSessionId, request.Revision, request.Type);
            if (hasDuplicateTask)
            {
                NotifyValidationErrors(typeof(ExportCommand), new List<string> { "Yêu cầu này đang được xử lý. Vui lòng kiểm tra trong nội dung Xử lý import, xuất file" });
                return null;
            }

            var fileType = request.Type.ToLower().Contains("tozip") ? ".zip" : ".xlsx";
            var fileName = !string.IsNullOrEmpty(request.FileName) ? $"{request.FileName.Replace(">", "")}{fileType}" : $"{request.Type}_KV{(DateTime.Now.ToString("ddMMyyyy-HHmmssffff"))}{fileType}";
            ImportExportFile importExportFile = new ImportExportFile()
            {
                EventType = request.Type,
                IsImport = false,
                KvSessionId = request.KvSessionId,
                FileName = fileName,
                Status = (byte)ImportExportStatus.Processing

            };
            var importExportExecutionContext = await _importExportService.GetExecutionContext();
            if (_configuration.IsCallExportRequestToKiotApi)
            {
                importExportFile = await _serviceClient.AddImportExportFile(new InternalImportExportReq()
                {
                    BranchId = importExportExecutionContext.BranchId,
                    ImportExportFile = importExportFile,
                    RetailerId = importExportExecutionContext.RetailerId,
                    UserId = importExportExecutionContext.User?.Id ?? 0
                });
            }
            else
            {
                importExportFile = await _retailDbService.AddImportExportFile(importExportFile);
            }

            var importExportSession = await _importExportService.GetSession(request.KVSession);

            using (var mqClient = _messageFactory.CreateMessageQueueClient())
            {
                var data = request.ConvertTo<TsExportRequestObject>();
                data.__type = "KiotViet.TimeSheet.ImportExport.Utilities.Mq.TsExportRequestObject, KiotViet.TimeSheet.ImportExport.Utilities.Mq";
                data.ExecutionContext = importExportExecutionContext;
                data.KVSession = importExportSession;
                data.FileExport = importExportFile;
                mqClient.Publish(data);
            }

            return importExportFile.ConvertTo<ImportExportFileDto>();
        }

        public class TsExportRequestObject
        {
            public string __type { get; set; }

            public string Type { get; set; }
            public string FileName { get; set; }
            public string Columns { get; set; }
            public string Filters { get; set; }
            public string Revision { get; set; }

            public ImportExportFile FileExport { get; set; }

            public ImportExportSession KVSession { get; set; }

            public ImportExportExecutionContext ExecutionContext { get; set; }
        }
    }
}

