using NerdStore.Core.DomainObjects;

namespace NerdStore.Vendas.Domain
{
    public class Pedido
    {
        public static int MAX_UNIDADES_ITEM => 15;
        public static int MIN_UNIDADES_ITEM => 1;
        public Guid ClienteId { get; private set; }
        public decimal ValorTotal { get; private set; }

        private readonly List<PedidoItem> _pedidoItems;
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItems;
        public PedidoStatus PedidoStatus { get; private set; }

        protected Pedido()
        {
            _pedidoItems = new List<PedidoItem>();
        }

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(x => x.CalcularValor());
        }

        private bool PedidoItemExistente(PedidoItem item)
        {
            return _pedidoItems.Any(p => p.ProdutoId == item.ProdutoId);
        }

        private void ValidarPedidoItemInexistente(PedidoItem pedidoItem)
        {
            if (!PedidoItemExistente(pedidoItem))
                throw new DomainException($"Item {pedidoItem.ProdutoId} não foi encontrado no pedido");
        }

        private PedidoItem ObterPedidoItemPorId(Guid produtoItemId)
        {
            return _pedidoItems.FirstOrDefault(p => p.ProdutoId == produtoItemId);
        }
        private void ValidarQuantidadeItemPermitida(PedidoItem item)
        {
            var quantidadeItens = item.Quantidade;
            if (PedidoItemExistente(item))
            {
                var itemExistente = ObterPedidoItemPorId(item.ProdutoId);
                quantidadeItens += itemExistente.Quantidade;
            }

            if(quantidadeItens > MAX_UNIDADES_ITEM)
                throw new DomainException($"Máximo de {MAX_UNIDADES_ITEM} unidades por produto");
        }

        public void AdicionarItem(PedidoItem item)
        {
            ValidarQuantidadeItemPermitida(item);

            if (PedidoItemExistente(item))
            {
                var itemExistente = ObterPedidoItemPorId(item.ProdutoId);
                itemExistente.AdicionarQuantidade(item.Quantidade);
                item = itemExistente;

                _pedidoItems.Remove(itemExistente);
            }
            _pedidoItems.Add(item);
            CalcularValorPedido();
        }

        public void AtualizarItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);
            ValidarQuantidadeItemPermitida(pedidoItem);

            var itemExistente = ObterPedidoItemPorId(pedidoItem.ProdutoId);

            _pedidoItems.Remove(itemExistente);
            _pedidoItems.Add(pedidoItem);

            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);

            _pedidoItems.Remove(pedidoItem);

            CalcularValorPedido();
        }

        public void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido
                {
                    ClienteId = clienteId
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }


    }
}
