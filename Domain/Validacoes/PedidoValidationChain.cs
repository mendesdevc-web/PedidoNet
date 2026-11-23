using PedidosNet.Models;

namespace PedidosNet.Domain.Validacoes
{
    public class PedidoValidationChain
    {
        private readonly IPedidoValidationHandler _first;
        public PedidoValidationChain(ClienteBloqueadoHandler clienteBloqueado, EstoqueHandler estoque, LimiteCreditoHandler limiteCredito)
        {
            clienteBloqueado.SetNext(estoque).SetNext(limiteCredito);

            _first = clienteBloqueado;
        }

        public Task<Result> ValidateAsnyc(Cliente cliente, IReadOnlyCollection<ItemPedido> items)
        {
            return _first.HandleAsync(cliente, items);
        }
    }
}
