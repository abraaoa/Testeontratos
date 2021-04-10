using System;
using System.Net.Http;
using Xunit;
namespace Contratos_Test
{
    public class UnitTest1
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        [Fact]
        public void Test1()
        {
            Contrato c = new Contrato();
            c.Id = 0;
            c.DataContratacao = DateTime.Now;
            c.QuantidadeParcelas = 3;
            c.ValorFianciado = 5000;
            
        }
    }
}
