using Features.Clientes;
using Features.Test._01___Traits;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Features.Test._03___Fluent_Assertions
{
    [Collection(nameof(ClienteCollection))]
    public class ClienteServiceFluentAssertionsTests
    {
        private readonly ClienteTestsFixture _clienteTestsFixture;
        private readonly ClienteService _clienteService;
        public ClienteServiceFluentAssertionsTests(ClienteTestsFixture clienteTestsFixture)
        {
            this._clienteTestsFixture = clienteTestsFixture;
            this._clienteService = _clienteTestsFixture.ObterClienteService();
        }

        

        [Fact(DisplayName = "Adicionar Cliente com Sucesso")]
        [Trait("Categoria", "Cliente Service Fluent Assertions Tests")]
        public void ClienteService_Adicionar_DeveExecutarComSucesso()
        {
            var cliente = _clienteTestsFixture.GerarClienteValido();

            _clienteService.Adicionar(cliente);

            cliente.EhValido().Should().BeTrue();
            _clienteTestsFixture.Mocker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Once);
            _clienteTestsFixture.Mocker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Cliente com Falha")]
        [Trait("Categoria", "Cliente Service Fluent Assertions Tests")]
        public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
        {
            var cliente = _clienteTestsFixture.GerarClienteInvalido();
            var clienteRepository = new Mock<IClienteRepository>();
            var mediatr = new Mock<IMediator>();
            var clienteService = new ClienteService(clienteRepository.Object, mediatr.Object);

            clienteService.Adicionar(cliente);

            cliente.EhValido().Should().BeFalse("Possui inconsistencias");
            cliente.ValidationResult.Errors.Should().HaveCountGreaterThanOrEqualTo(1);
            clienteRepository.Verify(r => r.Adicionar(cliente), Times.Never);
            mediatr.Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Obter Clientes Ativos")]
        [Trait("Categoria", "Cliente Service Fluent Assertions Tests")]
        public void ClienteService_ObterTodosAtivos_DeveRetornarApenasClientesAtivos()
        {
            var mock = _clienteTestsFixture.Mocker;
            mock.GetMock<IClienteRepository>().Setup(c => c.ObterTodos()).Returns(_clienteTestsFixture.ObterClientesVariados());

            var clientes = _clienteService.ObterTodosAtivos();

            mock.GetMock<IClienteRepository>().Verify(r => r.ObterTodos(), Times.Once);

            clientes.Should().HaveCountGreaterThanOrEqualTo(1).And.OnlyHaveUniqueItems();
            clientes.Should().NotContain(c => !c.Ativo); 
        }
    }
}
