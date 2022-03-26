using APIeCommerceCursoDapper.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace APIeCommerceCursoDapper.Repositories
{
    public class ContribUsuarioRepositorio : IUsuarioRepositorio<Usuario>
    {

        private IDbConnection _conecao;

        public ContribUsuarioRepositorio()
        {
            _conecao = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ECommerce;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        public List<Usuario> Get()
        {
            return _conecao.GetAll<Usuario>().ToList();
        }

        public Usuario Get(int id)
        {
           return _conecao.Get<Usuario>(id);
        }
        public void Cadastrar(Usuario usuario)
        {
           usuario.Id =  Convert.ToInt32(_conecao.Insert(usuario));
        }

        public void Update(Usuario usuario)
        {
           _conecao.Update(usuario);
        }

        public void Delete(int id)
        {

            var usuarios = _conecao.GetAll<Usuario>().ToList();

            _conecao.Delete(usuarios.Where(x => x.Id == id).First());
        }
    }
}
