using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosNet.Application.Commands
{
    public sealed record CriarPedidoCommand(Guid ClienteId, IReadOnlyCollection<CriarPedidoCommand.Item> Itens)
         : ICommand<Guid>
    {
        public sealed record Item(Guid ProdutoId, int Quantidade);
        
    }
}
