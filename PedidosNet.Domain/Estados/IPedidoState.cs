using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Estado
{
    public interface IPedidoState
    {
        PedidoStatus Status { get; }
        Result Aprovar(Pedido pedido);
        Result Processar(Pedido pedido);
        Result Enviar(Pedido pedido);
        Result Entregar(Pedido pedido);

    }
}
