using PedidosNet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosNet.Domain.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido?> ObterPorIdAsnyc(Guid pedidoId);
        Task AtualizarAsnyc(Pedido pedido);
        Task AdicionarAsync(Pedido pedido);
    }
}
