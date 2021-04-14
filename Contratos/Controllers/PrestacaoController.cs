using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contratos.Models;
using Contratos.Data;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Prestacoes.Controllers
{
    [Route("v1/prestacoes")]
    [ApiController]
    public class PrestacaoController : ControllerBase
    {

        [HttpGet]
        [Route("contrato/{id:long}")]
        public async Task<ActionResult<List<Prestacao>>> GetByContratoId([FromServices] DataContext context, long id)
        {
            var prestacoes = await context.Prestacoes_Contrato
                .Include(x => x.Contrato)
                .AsNoTracking()
                .Where(x => x.ContratoId == id)
                 .ToListAsync();
            return prestacoes;
        }





        [HttpPost]
        [Route("{id:long}/baixar")]
        public async Task<ActionResult<Prestacao>> Baixar([FromServices] DataContext context, [FromBody] string dataPagamento, long id)
        {

            DateTime _dataPagamento;
            if (!DateTime.TryParse(dataPagamento, out _dataPagamento))
            {
                return BadRequest();
            }

            var Prestacao = await context.Prestacoes_Contrato
            .AsNoTracking()
            .FirstOrDefaultAsync(Prestacao => Prestacao.Id == id);
            if (Prestacao == null)
            {
                return NotFound();
            }
            Prestacao.DataPagamento = _dataPagamento;
            context.Prestacoes_Contrato.Update(Prestacao);

            await context.SaveChangesAsync();

            return Ok(Prestacao);
        }


    }
}
