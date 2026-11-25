using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Estado
{
    public class PedidoEnviadoState : PedidoStateBase
    {
        public override PedidoStatus Status => PedidoStatus.Enviado;

        public override Result Processar(Pedido pedido)
        {
            pedido.AlterarEstado(new PedidoEntregueState());

            return Result.Success();
        }
    }
}
