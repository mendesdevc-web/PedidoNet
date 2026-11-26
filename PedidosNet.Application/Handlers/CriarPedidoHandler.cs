using PedidosNet.Domain;
using PedidosNet.Application.Commands;
using PedidosNet.Domain.Repositories;
using PedidosNet.Domain.Entities;
using PedidosNet.Domain.Validacoes;
using PedidosNet.Domain.Criacao;
using PedidosNet.Domain.Descontos;

namespace PedidosNet.Application.Handlers
{
    public sealed class CriarPedidoHandler(
        IClienteRepository clienteRepository,
        IProdutoRepository produtoRepository,
        PedidoValidationChain ValidationChain,
        IPedidoRepository pedidoRepository,
        IPedidoFactory pedidoFactory,
        SeledorDeDescontos seletorDescontos)
        : ICommandHandler<CriarPedidoCommand, Guid>
    {
        public async Task<Result<Guid>> HandleAsync(CriarPedidoCommand command)
        {
            var cliente = await clienteRepository.ObterProIdAsync(command.ClienteId);

            if (cliente is null)
                return Result<Guid>.Failure("cliente_nao_encontrado", "Cliente não encontrado.");

            var produtoIds = command.Itens.Select(i => i.ProdutoId).Distinct().ToList();
            var produtos = await produtoRepository.ObterPorIdsAsync(produtoIds);
            var produtosPorId = produtos.ToDictionary(p => p.Id);

            foreach (var produtoId in produtoIds)
            {
                if (!produtosPorId.ContainsKey(produtoId))
                {
                    return Result<Guid>.Failure("produto_nap_encontrato", $"Produto {produtoId} não encontrado.");
                }
            }

            IReadOnlyCollection<ItemPedido> itens =
                [..command.Itens.Select(item =>{
                    var produto = produtosPorId[item.ProdutoId];

                   return new ItemPedido{
                        Id = Guid.NewGuid(),
                        ProdutoId = produto.Id,
                        NomeProduto = produto.Nome,
                        Preco = produto.Preco,
                        Quantidade = item.Quantidade,
                        Produto = produto};
                     })
                ];

            var validacao = await ValidationChain.ValidateAsnyc(cliente, itens);
            if (!validacao.IsSuccess)
                return Result<Guid>.Failure(validacao.ErrorCode!, validacao.ErrorMessage);

            var pedido = pedidoFactory.Criar(cliente, itens);

            var strategies = seletorDescontos.Selecionar(pedido);
            pedido.AplicarDescontos(strategies);

            await pedidoRepository.AdicionarAsync(pedido);

            return Result<Guid>.Success(pedido.Id);

            throw new NotImplementedException();
        }
    }
}