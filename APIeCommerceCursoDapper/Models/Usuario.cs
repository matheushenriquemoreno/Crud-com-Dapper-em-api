using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIeCommerceCursoDapper.Models
{
    [Table("Usuarios")] // diz pro dapper contrib qual tabela e pra usar 
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Sexo { get; set; }

        public string RG { get; set; }

        public string CPF { get; set; }

        public string NomeMae { get; set; }

        public string SituacaoCadastro { get; set; }

        public DateTimeOffset DataCadastro { get; set; } // pega a data levando em conta o fuso horario  

        [Write(false)] 
        /* composicao com relacionamento de 1 pra 1 */
        public Contato Contato { get; set; }

        [Write(false)]
        /* composicao com relacionamento de 1 para muitos */
        public ICollection<EnderecoEntrega> EnderecosEntrega { get; set; }

        [Write(false)] // anotação pra evitar erro ao inserir usuario no dapper contrib, faz com que esse campo seja desconsiderado.
        /*Composicao com relacionamento de muitos para muitos*/
        public ICollection<Departamento> Departamentos { get; set; }

        public Usuario()
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
