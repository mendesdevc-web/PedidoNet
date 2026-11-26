using PedidosNet.Application.Commands;
using PedidosNet.Domain;
using PedidosNet.Domain.Repositories;

namespace PedidosNet.Application.Handlers
{
    public sealed class ProcessarPedidoHandler(IPedidoRepositoriy repositoriy) : ICommandHandler<ProcessarPedidoCommand>
    {
        public async Task<Result> HandleAsnync(ProcessarPedidoCommand command)
        {
            var pedido = await repositoriy.ObterPorIdAsnyc(command.PedidoId);

            if (pedido is null)
                return Result.Failure("pedido_nao_encontratdo", "Pedido não encontrado");

            var resultado = pedido.Aprovar();

            if (!resultado.IsSuccess)
                return resultado;

            await repositoriy.AtualizarAsnyc(pedido);

            return Result.Success();
        }
    }
}
