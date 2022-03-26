using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIeCommerceCursoDapper.Repositories;
using APIeCommerceCursoDapper.Models;

namespace APIeCommerceCursoDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly IUsuarioRepositorio<Usuario> _repository;

        public UsuariosController()
        {
            _repository = new UsuarioRepositorio();
        }

        /*
            CRUD
            GET -> Obter a lista de usuários.
            GET -> Obter o usuario passando o ID.
            POST -> Cadastrar um usuário.
            PUT -> Atualizar um usuário.
            DELETE -> Excluir um usuario.

            HTTP - 200 Ok, 300 - Rediricione, 400 - erro cliente, 500 erro senvidor
         */



        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_repository.Get());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var usuarioRetornar = _repository.Get(id);

            if (usuarioRetornar.Id != id)
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
