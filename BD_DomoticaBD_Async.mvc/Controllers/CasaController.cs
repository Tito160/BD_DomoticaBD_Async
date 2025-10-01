using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Biblioteca;
using MinimalApi.Dtos;

namespace BD_DomoticaBD_Async.mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasaController : Controller
    {
        private readonly IAdoAsync _repo;

        public CasaController(IAdoAsync repo)
        {
            _repo = repo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var casa = await _repo.ObtenerCasaAsync(id);
            if (casa is null) return NotFound();

            var response = new CasaResponse(casa.IdCasa, casa.Direccion);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var casas = await _repo.ObtenerTodasLasCasasAsync();
            var response = casas.Select(c => new CasaResponse(c.IdCasa, c.Direccion)).ToList();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CrearCasaRequest request)
        {
            var nuevaCasa = new Casa { Direccion = request.Direccion };
            await _repo.AltaCasaAsync(nuevaCasa);

            var response = new CasaResponse(nuevaCasa.IdCasa, nuevaCasa.Direccion);
            return CreatedAtAction(nameof(Get), new { id = nuevaCasa.IdCasa }, response);
        }
    }
}