using PedidosNet.Models;

namespace PedidosNet.Domain.Validacoes
{
    public class EstoqueHandler : PedidoValidationHandlerBase
    {
        protected override Task<Result> ValidarAsync(Cliente cliente, IReadOnlyCollection<ItemPedido> itens)
        {
            foreach (var item in itens)
            {
                if (item.Produto.Estoque < item.Quantidade)
                {
                    return Task.FromResult(Result.Failure("estoque_insuficiente", $"Estoque insuficiente para, {item.NomeProduto}"));
                }
            }
            return Task.FromResult(Result.Success());
        }
    }
}
