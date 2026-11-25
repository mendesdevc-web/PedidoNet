using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Descontos
{
    public interface IDescontoStrategy
    {
        bool AplicarPara(Pedido pedido);
        decimal CalcularDesconto(Pedido pedido);

        bool Exclusiva => false;
    }
}
