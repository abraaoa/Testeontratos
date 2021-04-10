using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contratos.Models;
using Contratos.Data;
using System.Linq;

namespace Prestacaos.Controllers
{
    [Route("prestacoes")]
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
        [Route("")]
        public async Task<ActionResult<Prestacao>> Post([FromServices] DataContext context, [FromBody] Prestacao model)
        {

            if (ModelState.IsValid && model.Id == 0)
            {
                context.Prestacoes_Contrato.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            else
            {
                return BadRequest(ModelState);
            }

        }
    }
}
