using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Descontos
{
    public class DescontoVip : IDescontoStrategy
    {
        public bool AplicarPara(Pedido pedido)
            => pedido.Cliente is { Segmento: SegmentoCliente.Vip };

        public decimal CalcularDesconto(Pedido pedido)
            => pedido.Total * 0.1m;
    }
}
