using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Descontos
{
    public class SeledorDeDescontos
    {
        private readonly IReadOnlyList<IDescontoStrategy> _strategiesEmOrdem;
        public SeledorDeDescontos(IEnumerable<IDescontoStrategy> strategies)
        {
            _strategiesEmOrdem = strategies.ToList();
        }

        public IEnumerable<IDescontoStrategy> Selecionar(Pedido pedido)
        {
            var aplicaveis = _strategiesEmOrdem.Where(s => s.AplicarPara(pedido)).ToList();

            var exclusiva = aplicaveis.FirstOrDefault(s => s.Exclusiva);

            return exclusiva is not null
                ? new[] { exclusiva } : aplicaveis; // se um exclusiva se aplica, ignora todo o resto senao, comportamento de sempre
        }
    }
}
