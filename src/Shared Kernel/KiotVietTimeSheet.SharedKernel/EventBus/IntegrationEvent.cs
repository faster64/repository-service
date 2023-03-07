using KiotVietTimeSheet.SharedKernel.Auth;
using Newtonsoft.Json;
using System;

namespace KiotVietTimeSheet.SharedKernel.EventBus
{
    public class IntegrationEventContext
    {
        [JsonConstructor]
        public IntegrationEventContext(int tenantId, int branchId, long userId, int groupId, string retailerCode, SessionUser user, string language = null)
        {
            TenantId = tenantId;
            UserId = userId;
            BranchId = branchId;
            GroupId = groupId;
            RetailerCode = retailerCode;
            User = user;
            Language = language ?? "vi-VN";
        }

        [JsonProperty]
        public int TenantId { get; private set; }

        [JsonProperty]
        public int BranchId { get; private set; }

        [JsonProperty]
        public long UserId { get; private set; }

        [JsonProperty]
        public int GroupId { get; private set; }

        [JsonProperty]
        public string RetailerCode { get; private set; }

        [JsonProperty]
        public SessionUser User { get; private set; }

        [JsonProperty]
        public bool WithDeleted { get; private set; }

        [JsonProperty]
        public string Language { get; private set; }

        public void SetUserId(long userId)
        {
            UserId = userId;
        }

        public void SetUserId(long userId, string name)
        {
            UserId = userId;
            User = new SessionUser()
            {
                Id = userId,
                UserName = name,
                Language = "vi-VN"
            };
        }

        public void SetBranchId(int branchId)
        {
            BranchId = branchId;
        }
    }

    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedTime = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createdTime, IntegrationEventContext context)
        {
            Id = id;
            CreatedTime = createdTime;
            Context = context;
        }

        [JsonProperty]
        public Guid Id { get; protected set; }

        [JsonProperty]
        public DateTime CreatedTime { get; private set; }

        [JsonProperty]
        public IntegrationEventContext Context { get; private set; }

        public void SetContext(IntegrationEventContext context)
        {
            Context = context;
        }
    }
}