using PedidosNet.Domain;
using PedidosNet.Domain.Descontos;
using PedidosNet.Domain.Estado;

namespace PedidosNet.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }

    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public Endereco EnderecoEntrega { get; set; } = null!;
    public string? CupomDesconto { get; set; }

    public PedidoStatus Status { get; private set; }
    private IPedidoState? _estado;
    public decimal Total { get; set; }
    public DateTime DataCriacao { get; set; }

    public List<ItemPedido> Itens { get; set; } = new();

    public void CalcularTotal()
    {
        Total = Itens.Sum(i => i.Preco * i.Quantidade);
    }
    
    public void AplicarDescontos( IEnumerable<IDescontoStrategy> Strategies)
    {
        foreach (var strategy in Strategies)
        {
            var desconto = strategy.CalcularDesconto(this);
            Total -= desconto;
        }
    }
    public Result Aprovar() => ObterEstado().Aprovar(this);
    public Result Processar() => ObterEstado().Processar(this);
    public Result Enviar() => ObterEstado().Enviar(this);
    public Result Entregar() => ObterEstado().Entregar(this);

    public void AlterarEstado(IPedidoState novoEstado)
    {
        _estado = novoEstado;
        Status = novoEstado.Status;
    }

    private IPedidoState ObterEstado()
    {
        return _estado ??= CriarEstado(Status);
    }

    private static IPedidoState? CriarEstado(PedidoStatus status)
    {
        return status switch 
        {
            PedidoStatus.AguardandoValidacao => new PedidoAguardandoValidacaoState (),
            PedidoStatus.EmAnalise => new PedidoEmAnaliseState(),
            PedidoStatus.Pendente => new PedidoPendenteState(),
            PedidoStatus.Aprovado => new PedidoAprovadoState(),
            PedidoStatus.Processando => new PedidoProcessadoState(),
            PedidoStatus.Enviado => new PedidoEnviadoState(),
            PedidoStatus.Entregue => new PedidoEntregueState(),
            _=> throw new InvalidOperationException ($"Estado de pedido invalido: {status}") 
        };
    }
}
