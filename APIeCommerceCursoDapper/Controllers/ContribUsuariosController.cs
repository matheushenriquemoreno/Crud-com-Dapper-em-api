using APIeCommerceCursoDapper.Models;
using APIeCommerceCursoDapper.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIeCommerceCursoDapper.Controllers
{
    [Route("api/Contrib/Usuarios")]
    [ApiController]
    public class ContribUsuariosController : ControllerBase
    {

        private readonly IUsuarioRepositorio<Usuario> _repository;

        public ContribUsuariosController()
        {
            _repository = new ContribUsuarioRepositorio();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_repository.Get());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var usuarioRetornar = _repository.Get(id);

            if (usuarioRetornar == null)
            {
                return NotFound();
            }

            return Ok(usuarioRetornar);

        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Usuario usuario)
        {
            _repository.Cadastrar(usuario);
            return Ok(usuario);
        }

        [HttpPut]
        public IActionResult Update([FromBody] Usuario usuario)
        {
            _repository.Update(usuario);
            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            return Ok();
        }
    }
}
