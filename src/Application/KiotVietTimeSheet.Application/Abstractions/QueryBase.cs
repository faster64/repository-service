using MediatR;

namespace KiotVietTimeSheet.Application.Abstractions
{
    public abstract class QueryBase<TResponse> : IRequest<TResponse>
    {

    }
}
