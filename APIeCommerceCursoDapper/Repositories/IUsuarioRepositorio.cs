using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIeCommerceCursoDapper.Models;

namespace APIeCommerceCursoDapper.Repositories
{
    interface IUsuarioRepositorio<T>
    {

        public List<T> Get();
        public T Get(int id);

        public void Cadastrar(T usuario);

        public void Update(T usuario);

        public void Delete(int id);


    }
}
