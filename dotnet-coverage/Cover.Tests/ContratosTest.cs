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

namespace Contratos.Contratos_Test
{
    [Collection("Contratos_Serial")]
    public class ContratosTest
    {

        ContratoController contratoController;
        PrestacaoController prestacaoController;


        public ContratosTest()
        {


            contratoController = new ContratoController(new MemoryCache(new MemoryCacheOptions()), null);
            prestacaoController = new PrestacaoController();

        }

        [Fact]
        public async Task Test1_CriarContrato()
        {

            var _options = new DbContextOptionsBuilder<Contratos.Data.DataContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
    .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new Contratos.Data.DataContext(_options))
            {
                //Arrange
                Contrato contrato = new Contrato();
                contrato.DataContratacao = DateTime.Now.AddDays(-60);
                contrato.ValorFianciado = 12000;
                contrato.QuantidadeParcelas = 3;
                
                //Act
                ActionResult<Contrato> result = await contratoController.Post(context, contrato);
                long totalPrestacoes = 0;
                string statusPrimeiraPrestacao = String.Empty;
                foreach (Prestacao p in result.Value.Prestacoes)
                {
                    if (statusPrimeiraPrestacao == string.Empty) { statusPrimeiraPrestacao = p.StatusPrestacao; }
                    totalPrestacoes += p.Valor;
                }

                //Assert
                Assert.IsType<Contrato>(result.Value);
                Assert.Equal(contrato.QuantidadeParcelas, result.Value.Prestacoes.Count);
                Assert.Equal("Atrasada", statusPrimeiraPrestacao);
                Assert.Equal(contrato.ValorFianciado, totalPrestacoes);
            }
        }


        [Fact]
        public async Task Test2_AlterarPrestacao()
        {
            // Arrange
            var _options = new DbContextOptionsBuilder<Contratos.Data.DataContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
    .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new Contratos.Data.DataContext(_options))
            {
                
                Contrato contrato = new Contrato();
                contrato.DataContratacao = DateTime.Now.AddDays(-60);
                contrato.ValorFianciado = 12000;
                contrato.QuantidadeParcelas = 3;
                context.Contratos.Add(contrato);

                Prestacao p1 = new Prestacao();
                p1.ContratoId = 1;
                p1.DataVencimento = DateTime.Now.AddDays(-29);
                p1.Valor = 1000;
                context.Prestacoes_Contrato.Add(p1);
                
                Prestacao p2 = new Prestacao();
                p2.ContratoId = 1;
                p2.DataVencimento = DateTime.Now.AddDays(-1);
                p2.Valor = 1000;
                context.Prestacoes_Contrato.Add(p2);
                
                Prestacao p3 = new Prestacao();
                p3.ContratoId = 1;
                p3.DataVencimento = DateTime.Now.AddDays(+29);
                p3.Valor = 1000;
                context.Prestacoes_Contrato.Add(p3);
                context.SaveChanges();
                context.Entry<Contrato>(contrato).State = EntityState.Detached;
                context.Entry<Prestacao>(p1).State = EntityState.Detached;
                context.Entry<Prestacao>(p2).State = EntityState.Detached;
                context.Entry<Prestacao>(p3).State = EntityState.Detached;
                //context.SaveChanges();
                

                //Act
                var result = await prestacaoController.Baixar(context, DateTime.Now.AddDays(-10).ToString(),1);
                ActionResult<List<Prestacao>> result2 = await prestacaoController.GetByContratoId(context, 1);
                Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);

                var model = ((Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result).Value;
                Assert.IsType<Prestacao>(model);

                var valor = ((Prestacao)model).StatusPrestacao;

                //Assert
                //verifica o status apos a transacao
                Assert.Equal("Baixada", valor);
                //verifica se as prestacoes estao na base
                Assert.IsType<List<Prestacao>>(result2.Value);
                Assert.Equal(3, result2.Value.Count);
                // verifica o status das prestacoes
                Assert.Equal("Baixada", result2.Value[0].StatusPrestacao);
                Assert.Equal("Atrasada", result2.Value[1].StatusPrestacao);
                Assert.Equal("Aberta", result2.Value[2].StatusPrestacao);

            }
        }

        [Fact]
        public async Task Test3_ConsultarPrestacoeContratoExcluido()
        {
            //Arrange
            var _options = new DbContextOptionsBuilder<Contratos.Data.DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new Contratos.Data.DataContext(_options))
            {
                Contrato contrato = new Contrato();
                contrato.DataContratacao = DateTime.Now.AddDays(-60);
                contrato.ValorFianciado = 12000;
                contrato.QuantidadeParcelas = 3;
                context.Contratos.Add(contrato);

                Prestacao p1 = new Prestacao();
                p1.ContratoId = 1;
                p1.DataVencimento = DateTime.Now.AddDays(-29);
                p1.Valor = 1000;
                context.Prestacoes_Contrato.Add(p1);

                Prestacao p2 = new Prestacao();
                p2.ContratoId = 1;
                p2.DataVencimento = DateTime.Now.AddDays(-1);
                p2.Valor = 1000;
                context.Prestacoes_Contrato.Add(p2);

                Prestacao p3 = new Prestacao();
                p3.ContratoId = 1;
                p3.DataVencimento = DateTime.Now.AddDays(+29);
                p3.Valor = 1000;
                context.Prestacoes_Contrato.Add(p3);
                context.SaveChanges();
                context.Entry<Contrato>(contrato).State = EntityState.Detached;
                context.Entry<Prestacao>(p1).State = EntityState.Detached;
                context.Entry<Prestacao>(p2).State = EntityState.Detached;
                context.Entry<Prestacao>(p3).State = EntityState.Detached;

                //Act
                ActionResult<List<Prestacao>> result = await prestacaoController.GetByContratoId(context, 1);
                ActionResult<Contrato> result1 = await contratoController.Delete(context, 1);
                ActionResult<List<Prestacao>> result2 = await prestacaoController.GetByContratoId(context,1);

                //Assert
                // antes da exclusao existem prestacoes
                Assert.IsType<List<Prestacao>>(result.Value);
                Assert.NotEmpty(result.Value);
                //apos exclusao nao existem mais
                Assert.IsType<List<Prestacao>>(result2.Value);
                Assert.Empty(result2.Value);

            }
        }

    }
}

