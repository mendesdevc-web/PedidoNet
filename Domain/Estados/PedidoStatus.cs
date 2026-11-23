 namespace PedidosNet.Domain.Estado
{
    public enum PedidoStatus
    {
        Indefinido = 0,
        EmAnalise,
        AguardandoValidacao,
        Pendente,
        Aprovado,
        Processando,
        Enviado,
        Entregue
    }
}
