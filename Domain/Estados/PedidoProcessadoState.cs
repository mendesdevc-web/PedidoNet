using PedidosNet.Models;

namespace PedidosNet.Domain.Estado
{
    public class PedidoProcessadoState : PedidoStateBase
    {
        public override PedidoStatus Status => PedidoStatus.Processando;

        public override Result Processar(Pedido pedido)
        {
            pedido.AlterarEstado(new PedidoEnviadoState());

            return Result.Success();
        }
    }
}
