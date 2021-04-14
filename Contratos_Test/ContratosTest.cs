using System;
using Xunit;
using Contratos.Controllers;
using Contratos.Models;
using Prestacoes.Controllers;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;

namespace Contratos_Test
{
    [Collection("Contratos_Serial")]
    public class ContratosTest
    {

        ContratoController contratoController;
        PrestacaoController prestacaoController;
        Contratos.Data.DataContext context;
        public ContratosTest()
        {
            var _options = new DbContextOptionsBuilder<DbContext>()
                .UseInMemoryDatabase(databaseName: "Contratos")
                .Options;
            if (context == null)
            {
                context = new Contratos.Data.DataContext(_options);
            }
            contratoController = new ContratoController(new MemoryCache(new MemoryCacheOptions()), null);
            prestacaoController = new PrestacaoController();
            Thread.Sleep(2000);
        }

        [Fact]
        public async Task Test1_CriarContrato()
        {

            Contrato contrato = new Contrato();
            contrato.DataContratacao = DateTime.Now.AddDays(-60);
            contrato.ValorFianciado = 12000;
            contrato.QuantidadeParcelas = 3;
            ActionResult<Contrato> result = await contratoController.Post(context, contrato);
            Assert.IsType<Contrato>(result.Value);
            Assert.Equal(contrato.QuantidadeParcelas, result.Value.Prestacoes.Count);
            long totalPrestacoes = 0;
            string statusPrimeiraPrestacao = String.Empty;
            foreach (Prestacao p in result.Value.Prestacoes)
            {
                if (statusPrimeiraPrestacao == string.Empty) { statusPrimeiraPrestacao = p.StatusPrestacao; }
                totalPrestacoes += p.Valor;
            }
            Assert.Equal("Atrasada", statusPrimeiraPrestacao);
            Assert.Equal(contrato.ValorFianciado, totalPrestacoes);

        }


        [Fact]
        public async Task Test2_AlterarPrestacao()
        {

            var result = await prestacaoController.Baixar(context, DateTime.Now.AddDays(-10).ToString(), 1);
            Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);

            var model = ((Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result).Value;
            Assert.IsType<Prestacao>(model);

            var valor = ((Prestacao)model).StatusPrestacao;
            Assert.Equal("Baixada", valor);
        }

        [Fact]
        public async Task Test3_ConsultarPrestacoes()
        {
            Thread.Sleep(10000);
            ActionResult<List<Prestacao>> result = await prestacaoController.GetByContratoId(context, 1);
            Assert.IsType<List<Prestacao>>(result.Value);
            Assert.Equal(3, result.Value.Count);
            Assert.Equal("Baixada", result.Value[0].StatusPrestacao);
            Assert.Equal("Atrasada", result.Value[1].StatusPrestacao);
            Assert.Equal("Aberta", result.Value[2].StatusPrestacao);


        }

    }
}

