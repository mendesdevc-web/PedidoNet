using PedidosNet.Domain;
using PedidosNet.Application.Commands;

namespace PedidosNet.Application.Dispatchers
{
    public interface IDispatcher
    {
        Task<Result> SendAsync(ICommand command);
        Task<Result<TResult>> SendAsync<TResult>(ICommand<TResult> command);

    }
}