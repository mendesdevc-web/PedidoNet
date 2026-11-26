using Microsoft.EntityFrameworkCore;
using PedidosNet.Application.Commands;
using PedidosNet.Application.Dispatchers;
using PedidosNet.Application.Handlers;
using PedidosNet.Domain.Criacao;
using PedidosNet.Domain.Descontos;
using PedidosNet.Domain.Repositories;
using PedidosNet.Domain.Validacoes;
using PedidosNet.Infra;
using PedidosNet.Infra.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Corta o ciclo Pedido -> Itens -> ItemPedido -> Pedido -> ...
        // Fix definitivo (DTOs de resposta) entra organicamente mais adiante no curso.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PedidosNet — Sistema Ingênuo", Version = "v1" });
});

builder.Services.AddScoped<IPedidoFactory, PedidoFactory>();

//definir a ordem de desconto que sera aplicado
builder.Services.AddScoped<IDescontoStrategy, DescontoVip>();
builder.Services.AddScoped<IDescontoStrategy, DescontoValorPedido>();
builder.Services.AddScoped<IDescontoStrategy, DescontoCupom>();
builder.Services.AddScoped<IDescontoStrategy, DescontoSemanaDosPais>();
builder.Services.AddScoped<SeledorDeDescontos>();

builder.Services.AddScoped<ClienteBloqueadoHandler>();
builder.Services.AddScoped<EstoqueHandler>();
builder.Services.AddScoped<LimiteCreditoHandler>();

builder.Services.AddScoped<PedidoValidationChain>();

builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();


builder.Services.AddScoped<ICommandHandler<AprovarPedidoCommand>, AprovarPedidoHandler>();
builder.Services.AddScoped<ICommandHandler<ProcessarPedidoCommand>, ProcessarPedidoHandler>();
builder.Services.AddScoped<ICommandHandler<EnviarPedidoCommand>, EnviarPedidoHandler>();
builder.Services.AddScoped<ICommandHandler<EntregarPedidoCommand>, EntregarPedidoHandler>();

builder.Services.AddScoped<ICommandHandler<CriarPedidoCommand, Guid>, CriarPedidoHandler>();

builder.Services.AddScoped<IDispatcher, Dispatcher>();

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

var app = builder.Build();

// ⚠️ EnsureCreated + seed fixo — sem migrations. Coerente com o estágio
// "ingênuo" do curso; migrations formais entram quando a Infrastructure
// virar uma camada de verdade na Clean Architecture.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DbSeeder.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PedidosNet v1"));
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
