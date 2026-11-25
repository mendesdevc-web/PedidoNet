using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Estado
{
    public class PedidoAprovadoState : PedidoStateBase
    {
        public override PedidoStatus Status => PedidoStatus.Aprovado;

        public override Result Processar(Pedido pedido)
        {
            pedido.AlterarEstado(new PedidoProcessadoState());

            return Result.Success();
        }

    }
}
