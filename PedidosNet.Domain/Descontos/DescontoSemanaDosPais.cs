using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Descontos
{
    public class DescontoSemanaDosPais : IDescontoStrategy
    {
        private static readonly DateTime Inicio = new(2026, 8, 3);
        private static readonly DateTime Fim = new(2026, 8, 9);
        private static readonly decimal ValorMinimo = 100m;

        public bool AplicarPara(Pedido pedido)
        {
            var data = pedido.DataCriacao.Date;
            var estaNaSemanaPromocional = data >= Inicio && data <= Fim;
            return estaNaSemanaPromocional && pedido.Total > ValorMinimo;
        }

        public decimal CalcularDesconto(Pedido pedido)
            => pedido.Total * 0.20m;

        public bool Exclusiva => true;
    }
}
