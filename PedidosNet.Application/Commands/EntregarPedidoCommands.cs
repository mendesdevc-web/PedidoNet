using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosNet.Application.Commands
{
    public sealed record EntregarPedidoCommand(Guid PedidoId): ICommand;
}
