using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIeCommerceCursoDapper.Models;
namespace APIeCommerceCursoDapper.Mappers
{
    public class UsuarioMap : EntityMap<UsuarioFluentMap>
    {

        public UsuarioMap()
        {
            Map(u => u.Codigo).ToColumn("Id");
            Map(u => u.NomeCompleto).ToColumn("Nome");
            Map(u => u.NomeCompletoMae).ToColumn("NomeMae");
            Map(u => u.Situacao).ToColumn("SituacaoCadastro");
        }

    }
}
