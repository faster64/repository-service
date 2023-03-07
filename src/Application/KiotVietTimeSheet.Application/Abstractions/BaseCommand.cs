using MediatR;

namespace KiotVietTimeSheet.Application.Abstractions
{
    public abstract class BaseCommand : IRequest
    {
        public  string Retailer { get; set; }
    }

    public abstract class BaseCommand<TResult> : IRequest<TResult>
    {

    }
}
