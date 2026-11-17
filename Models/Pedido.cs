namespace PedidosNet.Models;

public class Pedido
{
    public Guid Id { get; set; }

    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public string EnderecoEntrega { get; set; } = string.Empty;
    public string? CupomDesconto { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime DataCriacao { get; set; }

    public List<ItemPedido> Itens { get; set; } = new();

    public void CalcularTotal()
    {
        Total = Itens.Sum(i => i.Preco * i.Quantidade);
    }

    public void AplicarCupom(string cupom, decimal percentual)
    {
        Total -= Total * percentual;
        CupomDesconto = cupom;
    }
}
