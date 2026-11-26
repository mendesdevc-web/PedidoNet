using PedidosNet.Domain.Entities;
using PedidosNet.Domain.Repositories;
using PedidosNet.Infra.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosNet.Infra
{
    public sealed class ClienteRepository(AppDbContext context) : IClienteRepository
    {
        public async Task<Cliente?> ObterProIdAsync(Guid clienteId)
        {
            return await context.Clientes.FindAsync(clienteId);
        }
    }
}
