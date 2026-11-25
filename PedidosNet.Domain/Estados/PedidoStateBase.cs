using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Estado
{
    public abstract class PedidoStateBase : IPedidoState
    {
        public abstract PedidoStatus Status { get; }

        public virtual Result Aprovar(Pedido pedido) => TransicaoInvalida("Aprovar");
        public virtual Result Processar(Pedido pedido) => TransicaoInvalida("processar");
        public virtual Result Enviar(Pedido pedido) => TransicaoInvalida("enviar");
        public virtual Result Entregar(Pedido pedido) => TransicaoInvalida("entregar");

        private Result TransicaoInvalida(string operacao)
        {
            return Result.Failure("PEDIDO_TRANSICAO_INVALIDA", 
                $"Um pedido no estado {Status} nao pode realizar a operacao '{operacao}'");
        }
    }
}
