using PedidosNet.Domain.Descontos;

namespace PedidosNet.Models;

public class Pedido
{
    public Guid Id { get; set; }

    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public Endereco EnderecoEntrega { get; set; } = null!;
    public string? CupomDesconto { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime DataCriacao { get; set; }

    public List<ItemPedido> Itens { get; set; } = new();

    public void CalcularTotal()
    {
        Total = Itens.Sum(i => i.Preco * i.Quantidade);
    }
    
    public void AplicarDesconto( IEnumerable<IDescontoStrategy> Strategies)
    {
        foreach (var strategy in Strategies)
        {
            var desconto = strategy.CalcularDesconto(this);
            Total -= desconto;
        }
    }
}
