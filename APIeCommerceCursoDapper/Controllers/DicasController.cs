using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using APIeCommerceCursoDapper.Models;
using Dapper.FluentMap;
using APIeCommerceCursoDapper.Mappers;

namespace APIeCommerceCursoDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DicasController : ControllerBase
    {

        private IDbConnection _conecao;

        public DicasController()
        {
            _conecao = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ECommerce;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" SELECT* FROM USUARIOS WHERE ID = @Id; ");
            query.AppendLine(" SELECT* FROM CONTATOS WHERE USUARIOid = @Id; ");
            query.AppendLine(" SELECT* FROM EnderecosEntrega where usuarioId = @Id; ");
            query.AppendLine(" SELECT D.* FROM UsuariosDepartamentos UD ");
            query.AppendLine("          INNER JOIN Departamentos D On UD.DepartamentoId = D.Id ");
            query.AppendLine("          WHERE UD.UsuarioId = @Id; ");

            using (var multiplosResultados = _conecao.QueryMultiple(query.ToString(), new { Id = id })) //metodo muito utilizado para relatorio, ele executa todas as querys passada pra ele e pega o resultado de cada uma.
            {
                var usuario = multiplosResultados.Read<Usuario>().First();
                var Contato = multiplosResultados.Read<Contato>().First();

                var enderecos = multiplosResultados.Read<EnderecoEntrega>().ToList();
                var departamentos = multiplosResultados.Read<Departamento>().ToList();

                if (usuario != null)
                {
                    usuario.Contato = Contato;
                    usuario.EnderecosEntrega = enderecos;
                    usuario.Departamentos = departamentos;

                    return Ok(usuario);
                }
            }
            return NotFound();
        }


        [HttpGet("stored/usuarios")]
        public IActionResult PegarUsuariosPorProcedureGet()
        {

           // _conecao.Query<Usuario>("EXEC SelecionarUsuarios"); // outra forma

          var usuarios = _conecao.Query<Usuario>("SelecionarUsuarios", commandType: CommandType.StoredProcedure);

            return Ok(usuarios);
        }

        [HttpGet("stored/usuario/{id}")]
        public IActionResult PegarUsuarioPorProcedureGet(int id)
        {

          var usuario = _conecao.Query<Usuario>("SelecionarUsuario", new { Id = id}, commandType: CommandType.StoredProcedure);

            return Ok(usuario);
        }

        [HttpGet("mapper1/usuarios")]
        public IActionResult MapperComDadosDiferentesSolucao1()
        {
            /*
             Quando uma coluna do banco diverge de nome com uma propriedade de uma classe a 2 soluções para esse problema.
             */

            // Primeira solução Renomenando as colunas via SQL com elias 
            var usuarios = _conecao.Query<UsuarioFluentMap>("SELECT Id as Codigo, Nome as NomeCompleto, Email, Sexo, RG, CPF, NomeMae as NomeCompletoMae, SituacaoCadastro as Situacao, DataCadastro FROM Usuarios");

            // var usuarios = _conecao.Query<UsuarioFluentMap>("SELECT * FROM Usuarios;"); // gera um erro pois as colunas do banco diverge das coluna da classe.

            return Ok(usuarios);

        }

        [HttpGet("mapper2/usuarios")]
        public IActionResult MapperComDadosDiferentesSolucao2()
        {

            /* inicializando a nossa clase onde fica o mapeamento do usuario, OBS: geralmente fica no Startup
             * Mapeamento por meio da Biblioteca Dapper.FluenteMap  
             */
            FluentMapper.Initialize(config =>
           {
               config.AddMap(new UsuarioMap());
           });

            var usuarios = _conecao.Query<UsuarioFluentMap>("SELECT * FROM Usuarios;");
            return Ok(usuarios);

        }
    }
}
