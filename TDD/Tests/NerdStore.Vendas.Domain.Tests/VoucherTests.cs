using System;
using System.Linq;
using Xunit;
using static NerdStore.Vendas.Domain.Voucher;

namespace NerdStore.Vendas.Domain.Tests
{
    public class VoucherTests
    {
        [Fact(DisplayName = "Validar Voucher Tipo Valor Válido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherValor_DeveEstarValido()
        {
            // Arrange
            var voucher = new Voucher(
                "PROMO-15-REAIS",
                15,
                null,
                TipoDescontoVoucher.Valor,
                1,
                DateTime.Now.AddDays(15),
                true,
                false
                );


            // Act
            var result = voucher.EhValido();

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validar Voucher Tipo Valor Inválido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherValor_DeveEstarInvalido()
        {
            // Arrange
            var voucher = new Voucher(
                "",
                null,
                15,
                TipoDescontoVoucher.Valor,
                0,
                DateTime.Now.AddDays(-1),
                false,
                true
                );


            // Act
            var result = voucher.EhValido();

            // Assert
            Assert.Equal(6, result.Errors.Count);
            Assert.False(result.IsValid);
            Assert.Contains(VoucherAplicavelValidation.AtivoErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.CodigoErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.DataValidadeErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.QuantidadeErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.UtilizadoErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.ValorDescontoErroMsg, result.Errors.Select(c => c.ErrorMessage));
        }

        [Fact(DisplayName = "Validar Voucher Tipo Porcentagem Válido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherPorcentagem_DeveEstarValido()
        {
            // Arrange
            var voucher = new Voucher(
                "PROMO-15-REAIS",
                null,
                15,
                TipoDescontoVoucher.Porcentagem,
                1,
                DateTime.Now.AddDays(15),
                true,
                false
                );


            // Act
            var result = voucher.EhValido();

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validar Voucher Tipo Porcentagem Inválido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherPorcentagem_DeveEstarInvalido()
        {
            // Arrange
            var voucher = new Voucher(
                "",
                15,
                null,
                TipoDescontoVoucher.Porcentagem,
                0,
                DateTime.Now.AddDays(-1),
                false,
                true
                );


            // Act
            var result = voucher.EhValido();

            // Assert
            Assert.Equal(6, result.Errors.Count);
            Assert.False(result.IsValid);
            Assert.Contains(VoucherAplicavelValidation.AtivoErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.CodigoErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.DataValidadeErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.QuantidadeErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.UtilizadoErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.PercentualDescontoErroMsg, result.Errors.Select(c => c.ErrorMessage));
        }
    }
}
