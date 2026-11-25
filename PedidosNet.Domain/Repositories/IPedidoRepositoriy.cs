using PedidosNet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosNet.Domain.Repositories
{
    public interface IPedidoRepositoriy
    {
        Task<Pedido?> ObterPorIdAsnyc(Guid pedidoId);
        Task AtualizarAsnyc(Pedido pedido);
    }
}
