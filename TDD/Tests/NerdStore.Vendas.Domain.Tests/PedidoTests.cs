﻿using NerdStore.Core.DomainObjects;
using System;
using System.Linq;
using Xunit;

namespace NerdStore.Vendas.Domain.Tests
{
    public class PedidoTests
    {
        [Fact(DisplayName = "Adicionar Item Novo Pedido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_NovoPedido_DeveAtualizarValor()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);

            // Act
            pedido.AdicionarItem(pedidoItem);

            // Assert
            Assert.Equal(200, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Adicionar Item Pedido Existente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_ItemExistente_DeveIncrementarUnidadesSomarValores()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 2, 100);
            var pedidoItem2 = new PedidoItem(produtoId, "Produto Teste", 1, 100);

            // Act
            pedido.AdicionarItem(pedidoItem);
            pedido.AdicionarItem(pedidoItem2);

            // Assert
            Assert.Equal(300, pedido.ValorTotal);
            Assert.Equal(1, pedido.PedidoItems.Count);
            Assert.Equal(3, pedido.PedidoItems.FirstOrDefault(p => p.ProdutoId == produtoId)?.Quantidade);
        }

        [Fact(DisplayName = "Adicionar Item Pedido Acima do Permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_UnidadesItemAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM+1, 100);

            // Act // Assert
            Assert.Throws<DomainException>( () => pedido.AdicionarItem(pedidoItem));
        }

        [Fact(DisplayName = "Adicionar Item Pedido Existente Acima do Permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_ItemExistenteSomaUnidadesAcimaPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 1, 100);
            var pedidoItem2 = new PedidoItem(produtoId, "Produto Teste 2", Pedido.MAX_UNIDADES_ITEM, 100);

            pedido.AdicionarItem(pedidoItem);
            // Act // Assert
            Assert.Throws<DomainException>(() => pedido.AdicionarItem(pedidoItem2));
        }

        [Fact(DisplayName = "Adicionar Item Pedido Abaixo do Permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_UnidadesItemAbaixoDoPermitido_DeveRetornarException()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            // Act // Assert
            Assert.Throws<DomainException>(() => new PedidoItem(produtoId, "Produto Teste", Pedido.MIN_UNIDADES_ITEM - 1, 100));
        }

        [Fact(DisplayName = "Atualizar Item Pedido Inexistente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemNaoExistenteNaLista_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 1, 100);

            // Act
            // Assert
            Assert.Throws<DomainException>(() => pedido.AtualizarItem(pedidoItem));
        }

        [Fact(DisplayName = "Atualizar Item Pedido Valido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemValido_DeveAtualizarQuantidade()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 2, 100);
            pedido.AdicionarItem(pedidoItem);

            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", 5, 100);
            var novaQuantidade = pedidoItemAtualizado.Quantidade;

            // Act
            pedido.AtualizarItem(pedidoItemAtualizado);
            // Assert
            Assert.Equal(novaQuantidade, pedido.PedidoItems.FirstOrDefault(item => item.ProdutoId == produtoId)?.Quantidade);
        }

        [Fact(DisplayName = "Atualizar Item Pedido Validar Total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_PedidoComProdutosDiferentes_DeveAtualizarValorTotal()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItemExistente1 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 5, 100);
            var pedidoItemExistente2 = new PedidoItem(produtoId, "Produto Teste", 2, 150);
            pedido.AdicionarItem(pedidoItemExistente1);
            pedido.AdicionarItem(pedidoItemExistente2);

            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", 10, 80);
            var valorTotalPedido = pedidoItemExistente1.Quantidade * pedidoItemExistente1.ValorUnitario +
                                   pedidoItemAtualizado.Quantidade * pedidoItemAtualizado.ValorUnitario;
            // Act
            pedido.AtualizarItem(pedidoItemAtualizado);

            // Assert
            Assert.Equal(valorTotalPedido, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Atualizar Item Pedido Quantidade Acima do Permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemUnidadesAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 5, 100);
            pedido.AdicionarItem(pedidoItem);

            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM+1, 80);

            // Act       
            // Assert
            Assert.Throws<DomainException>(() => pedido.AtualizarItem(pedidoItemAtualizado));
        }

        [Fact(DisplayName = "Remover Item Pedido Item Nao Existente Na Lista")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void RemoverItemPedido_ItemNaoExistenteNaLista_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 5, 100);
            
            // Act       
            // Assert
            Assert.Throws<DomainException>(() => pedido.RemoverItem(pedidoItem));
        }

        [Fact(DisplayName = "Remover Item Pedido Deve Calcular Valor Total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void RemoverItemPedido_ItemExistenteNaLista_DeveAtualizarValorTotal()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 5, 100);
            var pedidoItem2 = new PedidoItem(Guid.NewGuid(), "Produto Teste 2", 10, 200);
            var expected = pedidoItem2.Quantidade * pedidoItem2.ValorUnitario;

            pedido.AdicionarItem(pedidoItem1);
            pedido.AdicionarItem(pedidoItem2);

            // Act       
            pedido.RemoverItem(pedidoItem1);
            // Assert
            Assert.Equal(expected, pedido.ValorTotal);
        }
    }
}
