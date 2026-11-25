using Microsoft.EntityFrameworkCore;
using PedidosNet.Domain.Entities;

namespace PedidosNet.Infra.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
            .ComplexProperty(c => c.Endereco);

        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Cliente)
            .WithMany()
            .HasForeignKey(p => p.ClienteId);

        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Itens)
            .WithOne(i => i.Pedido)
            .HasForeignKey(i => i.PedidoId);

        modelBuilder.Entity<Pedido>()
            .ComplexProperty(c => c.EnderecoEntrega);

        modelBuilder.Entity<ItemPedido>()
            .HasOne(i => i.Produto)
            .WithMany()
            .HasForeignKey(i => i.ProdutoId);
    }
}
