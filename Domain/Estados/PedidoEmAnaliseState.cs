using PedidosNet.Models;

namespace PedidosNet.Domain.Estado
{
    public class PedidoEmAnaliseState : PedidoStateBase
    {
        public override PedidoStatus Status => PedidoStatus.EmAnalise;

        public override Result Processar(Pedido pedido)
        {
            pedido.AlterarEstado(new PedidoAprovadoState());

            return Result.Success();
        }
    }
}
