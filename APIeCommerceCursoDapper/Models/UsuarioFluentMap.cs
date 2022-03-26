using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIeCommerceCursoDapper.Models
{ 
   
    public class UsuarioFluentMap /* Criada somente para teste */
    { 
 
        public int Codigo { get; set; }

        public string NomeCompleto { get; set; }

        public string Email { get; set; }

        public string Sexo { get; set; }

        public string RG { get; set; }

        public string CPF { get; set; }

        public string NomeCompletoMae { get; set; }

        public string Situacao { get; set; }

        public DateTimeOffset DataCadastro { get; set; } 

        public Contato Contato { get; set; }


        public ICollection<EnderecoEntrega> EnderecosEntrega { get; set; }

        public ICollection<Departamento> Departamentos { get; set; }

        public UsuarioFluentMap()
        {
            EnderecosEntrega = new List<EnderecoEntrega>();
            Departamentos = new List<Departamento>();
        }

        public void AdicionarEnderecoEntrega(EnderecoEntrega endereco)
        {
            EnderecosEntrega.Add(endereco);
        }

        public void AdicionarDepartamentos(Departamento departamento)
        {
            Departamentos.Add(departamento);
        }

    }
}
