using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Validacoes
{
    public interface IPedidoValidationHandler
    {
        IPedidoValidationHandler SetNext (IPedidoValidationHandler next);

        Task<Result> HandleAsync(Cliente cliente, IReadOnlyCollection<ItemPedido> itens);
    }
}
