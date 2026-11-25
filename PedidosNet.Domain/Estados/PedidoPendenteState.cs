using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Estado
{
    public class PedidoPendenteState : PedidoStateBase
    {
        public override PedidoStatus Status => PedidoStatus.Pendente;

        public override Result Aprovar(Pedido pedido)
        {
            pedido.AlterarEstado(new PedidoAprovadoState());

            return Result.Success();
        }
    }
}
