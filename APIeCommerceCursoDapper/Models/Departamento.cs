using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIeCommerceCursoDapper.Models
{
    public class Departamento
    {

        public int Id { get; set; }
        public string Nome { get; set; }


        /*Composicao com relacionamento de muitos para muitos*/
        public ICollection<Usuario> Usuarios { get; set; }

    }
}
