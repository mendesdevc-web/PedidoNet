namespace PedidosNet.Domain.Estado
{
    public class PedidoEntregueState : PedidoStateBase
    {
        public override PedidoStatus Status => PedidoStatus.Entregue;
    }
}
