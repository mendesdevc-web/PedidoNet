using PedidosNet.Domain.Entities;
using PedidosNet.Domain.Repositories;
using PedidosNet.Infra.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosNet.Infra
{
    public sealed class PedidoRepository (AppDbContext context) : IPedidoRepositoriy
    {
         
        public async Task AtualizarAsnyc(Pedido pedido)
        {
            context.Pedidos.Update(pedido);
            await context.SaveChangesAsync();
        }

        public async Task<Pedido?> ObterPorIdAsnyc(Guid id)
        {
            return await context.Pedidos.FindAsync(id);
        }
    }
}
