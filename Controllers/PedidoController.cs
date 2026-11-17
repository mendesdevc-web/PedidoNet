using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedidosNet.Data;
using PedidosNet.Domain;
using PedidosNet.DTOs;
using PedidosNet.Models;

namespace PedidosNet.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPedidoFactory _pedidoFactory;

    public PedidoController(AppDbContext context, IPedidoFactory pedidoFactory)
    {
        _context = context;
        _pedidoFactory = pedidoFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .AsNoTracking()
            .ToListAsync();

        return Ok(pedidos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pedido is null ? NotFound() : Ok(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CreatePedidoRequest request)
    {
        var cliente = await _context.Clientes.FindAsync(request.ClienteId);
        if (cliente is null)
            return BadRequest("Cliente não encontrado.");

        //Regra 1: Cliente bloqueado nem chega a criar pedido
        if(cliente.Bloqueado)
            return BadRequest("Cliente bloqueado não pode criar pedidos.");

        var itens = new List<ItemPedido>();

        foreach (var itemRequest in request.Itens)
        {
            var produto = await _context.Produtos.FindAsync(itemRequest.ProdutoId);
            if (produto is null)
                return BadRequest($"Produto {itemRequest.ProdutoId} não encontrado.");

            itens.Add(new ItemPedido
            {
                Id = Guid.NewGuid(),
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                Preco = produto.Preco,
                Quantidade = itemRequest.Quantidade
            });
        }

        var pedido = _pedidoFactory.Criar(cliente, itens);

        if (cliente.Vip)
            pedido.Total -= pedido.Total * 0.10m;

        if (pedido.Total > 2000)
            pedido.Total -= pedido.Total * 0.05m;

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return Ok(pedido.Id);
    }

    [HttpPost("kits")]
    public async Task<IActionResult> CriarKit(CreatePedidoKitRequest request)
    {
        var cliente = await _context.Clientes.FindAsync(request.ClienteId);

        if (cliente is null)
            return BadRequest("Cliente não encontrado.");

        if (cliente.Bloqueado)
            return BadRequest("Cliente bloqueado não pode criar pedidos.");

        var builder = new KitPromocionalBuilder(_pedidoFactory).ParaCliente(cliente);

        try
        {
            foreach (var itemRequest in request.Itens)
            {
                var produto = await _context.Produtos.FindAsync(itemRequest.ProdutoId);

                if (produto is null)
                    return BadRequest($"Produto {itemRequest.ProdutoId} não encontrado.");
                
                builder.AddProduto(produto, itemRequest.Quantidade);
            }

           if(!string.IsNullOrWhiteSpace(request.Cupom))
                builder.AplicarCupom(request.Cupom);

           var pedido = builder.Build();

           _context.Pedidos.Add(pedido);
           await _context.SaveChangesAsync();
           return Ok(pedido.Id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
