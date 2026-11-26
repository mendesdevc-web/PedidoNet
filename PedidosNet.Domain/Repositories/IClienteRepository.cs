using PedidosNet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosNet.Domain.Repositories
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObterProIdAsync(Guid clienteId);
    }
}
