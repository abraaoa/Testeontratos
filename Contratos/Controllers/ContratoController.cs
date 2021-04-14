using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contratos.Models;
using Contratos.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.FeatureManagement;


namespace Contratos.Controllers
{
    [Route("v1/contratos")]
    [ApiController]
    public class ContratoController : ControllerBase
    {
        private IMemoryCache _cache;
        private readonly IFeatureManager _featureManager;
       public ContratoController(IMemoryCache memoryCache,  IFeatureManagerSnapshot featureManager)
        {

            _cache = memoryCache;
            _featureManager = featureManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Contrato>>> Get([FromServices] DataContext context)
        {
            string MyKey = "Contratos";
            if (_featureManager != null && await _featureManager.IsEnabledAsync(AppFeatureFlags.CacheAtivo))
            {
                if (!_cache.TryGetValue(MyKey, out List<Contrato> cacheEntry))
                {
                    // Key not in cache, so get data.
                    var contratos = await context.Contratos.ToListAsync();
                    DateTime newExpiretion = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(newExpiretion);

                    _cache.Set(MyKey, contratos, cacheEntryOptions);
                    return contratos;
                }
                else
                {
                    return cacheEntry;
                }
            }
            else
            {
                var contratos = await context.Contratos.ToListAsync();
                return contratos;
            }
        }


        [HttpGet]
        [Route("{id:long}")]
        public async Task<ActionResult<Contrato>> GetById([FromServices] DataContext context, long id)
        {

            if ( _featureManager != null && await _featureManager.IsEnabledAsync(AppFeatureFlags.CacheAtivo))
            {
                string MyKey = "Contrato_" + id;
                if (!_cache.TryGetValue(MyKey, out Contrato cacheEntry))
                {
                    // Key not in cache, so get data.
                    Contrato contrato = await ObterContratoPorId(context, id);
                    DateTime newExpiretion = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(newExpiretion);

                    _cache.Set(MyKey, contrato, cacheEntryOptions);
                    return contrato;
                }
                else
                {
                    return cacheEntry;
                }
            }
            else
            {
                Contrato contrato = await ObterContratoPorId(context, id);
                return contrato;
            }
        }

        private static async Task<Contrato> ObterContratoPorId(DataContext context, long id)
        {
            return await context.Contratos
            .AsNoTracking()
            .Include(Contrato => Contrato.Prestacoes)
            .FirstOrDefaultAsync(Contrato => Contrato.Id == id);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Contrato>> Post([FromServices] DataContext context, [FromBody] Contrato model)
        {
            if (ModelState.IsValid && model.Id == 0)
            {
                context.Contratos.Add(model);
                long valorPrestacoes = model.ValorFianciado / model.QuantidadeParcelas;
                DateTime dataProximaPrestacao;
                if (model.DataContratacao == DateTime.MinValue) {
                    dataProximaPrestacao = DateTime.Now.AddDays(30);
                }
                else {
                    dataProximaPrestacao = model.DataContratacao.AddDays(30);
                }
                
                for (int i = 1; i <= model.QuantidadeParcelas; i++)
                {
                    Prestacao p = new Prestacao();
                    p.ContratoId = model.Id;
                    p.Valor = valorPrestacoes;
                    p.DataVencimento = dataProximaPrestacao;
                    context.Prestacoes_Contrato.Add(p);
                    dataProximaPrestacao = dataProximaPrestacao.AddDays(30);
                }
                await context.SaveChangesAsync();
                return model;
            }
            else
            {
                return BadRequest(ModelState);
            }

        }



        [HttpDelete]
        [Route("{id:long}")]
        public async Task<ActionResult<Contrato>> Delete([FromServices] DataContext context, long id)
        {
            if (ModelState.IsValid)
            {
                Contrato contrato = await context.Contratos
                .AsNoTracking()
                .Include(Contrato => Contrato.Prestacoes)
                .FirstOrDefaultAsync(Contrato => Contrato.Id == id);
                if (contrato == null)
                {
                    return NotFound();
                }
                foreach (Prestacao p in contrato.Prestacoes)
                {
                    context.Prestacoes_Contrato.Remove(p);
                }
                context.Contratos.Remove(contrato);
                await context.SaveChangesAsync();
                //exclui do cache
                string MyKey = "Contrato_" + id;
                if (_cache.TryGetValue(MyKey, out Contrato cacheEntry))
                {
                    _cache.Remove(MyKey);
                }
                return NoContent();
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

    }
}

