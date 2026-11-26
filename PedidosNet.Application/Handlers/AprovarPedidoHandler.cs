using PedidosNet.Application.Commands;
using PedidosNet.Domain;
using PedidosNet.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosNet.Application.Handlers
{
    public sealed class AprovarPedidoHandler(IPedidoRepositoriy repositoriy) : ICommandHandler<AprovarPedidoCommand>
    {
        public async Task<Result> HandleAsnync(AprovarPedidoCommand command)
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
