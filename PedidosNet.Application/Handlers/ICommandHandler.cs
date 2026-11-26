using PedidosNet.Application.Commands;
using PedidosNet.Domain;

namespace PedidosNet.Application.Handlers
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Task<Result> HandleAsnync(TCommand command);
    }
}