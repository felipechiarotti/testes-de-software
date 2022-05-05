using Bogus;
using Bogus.DataSets;
using Features.Clientes;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Features.Test._01___Traits
{
    [CollectionDefinition(nameof(ClienteCollection))]
    public class ClienteCollection : ICollectionFixture<ClienteTestsFixture>
    {

    }
    public class ClienteTestsFixture : IDisposable
    {
        public ClienteService ClienteService;
        public AutoMocker Mocker;
        public Cliente GerarClienteValido()
        {
            return GerarClientesValidos(1, true).FirstOrDefault();
        }

        public IEnumerable<Cliente> ObterClientesVariados()
        {
            var clientes = new List<Cliente>();

            clientes.AddRange(GerarClientesValidos(50, true));
            clientes.AddRange(GerarClientesValidos(50, false));

            return clientes;
        }

        public Cliente GerarClienteInvalido()
            => new Cliente(
                    Guid.NewGuid(),
                    "",
                    "Chiarotti",
                    DateTime.Now.AddYears(-24),
                    "felihidsail.com",
                    true,
                    DateTime.Now
            );

        public IEnumerable<Cliente> GerarClientesValidos(int quantidade, bool ativo)
        {
            var genero = new Faker().PickRandom<Name.Gender>();
            var clientes = new Faker<Cliente>("pt_BR")
                .CustomInstantiator(f => new Cliente(
                    Guid.NewGuid(),
                    f.Name.FirstName(genero),
                    f.Name.LastName(genero),
                    f.Date.Past(80, DateTime.Now.AddYears(-18)),
                    email: "",
                    ativo: ativo,
                    DateTime.Now))
                .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.Nome.ToLower(), c.Sobrenome.ToLower()));
            return clientes.Generate(quantidade) ;
        }

        public ClienteService ObterClienteService()
        {
            Mocker = new AutoMocker();
            ClienteService = Mocker.CreateInstance<ClienteService>();

            return ClienteService;
        }

        public void Dispose()
        {

        }
    }
}
