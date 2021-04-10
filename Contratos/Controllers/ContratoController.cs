using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contratos.Models;
using Contratos.Data;

namespace Contratos.Controllers
{
    [Route("")]
    [ApiController]
    public class ContratoController : ControllerBase
    {
        [HttpGet]
        [Route("")]

        public async Task<ActionResult<List<Contrato>>> Get([FromServices] DataContext context)
        {
            var contratos = await context.Contratos.ToListAsync();
            return contratos;
        }

        [HttpGet]
        [Route("{id:long}")]
        public async Task<ActionResult<Contrato>> GetById([FromServices] DataContext context, long id)
        {
            var contrato  = await context.Contratos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return contrato;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Contrato>> Post([FromServices] DataContext context, [FromBody] Contrato model)
        {
            if (ModelState.IsValid && model.Id == 0)
            {
                context.Contratos.Add(model);
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
