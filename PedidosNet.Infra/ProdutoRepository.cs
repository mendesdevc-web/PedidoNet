using PedidosNet.Domain.Entities;
using PedidosNet.Domain.Repositories;
using PedidosNet.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace PedidosNet.Infra
{
    public sealed class ProdutoRepository(AppDbContext context) : IProdutoRepository
    {
        public async Task<IReadOnlyCollection<Produto>> ObterPorIdsAsync(
            IReadOnlyCollection<Guid> produtoIds)
        {
            return await context.Produtos
                .Where(p => produtoIds.Contains(p.Id))
                .ToListAsync();
        }
    }
}