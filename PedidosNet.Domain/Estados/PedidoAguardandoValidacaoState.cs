using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Estado
{
    public class PedidoAguardandoValidacaoState : PedidoStateBase
    {
        public override PedidoStatus Status =>  PedidoStatus.AguardandoValidacao;

        public override Result Aprovar(Pedido pedido)
        {
            pedido.AlterarEstado(new PedidoAprovadoState());

            return Result.Success();
        }
    }
}
