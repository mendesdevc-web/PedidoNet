using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosNet.Application.Commands
{
    public sealed record EnviarPedidoCommand(Guid PedidoId): ICommand;
}
