using PedidosNet.Models;

namespace PedidosNet.Domain.Validacoes
{
    public interface IPedidoValidationHandler
    {
        IPedidoValidationHandler SetNext (IPedidoValidationHandler next);

        Task<Result> HandleAsync(Cliente cliente, IReadOnlyCollection<ItemPedido> itens);
    }
}
