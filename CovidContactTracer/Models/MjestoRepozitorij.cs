using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidContactTracer.Models
{
    public class MjestoRepozitorij
    {
        private List<Mjesto> _mjestoLista;
        public MjestoRepozitorij()
        {
            _mjestoLista = new List<Mjesto> { };
        }
        public Mjesto Add(Mjesto mjesto)
        {
            _mjestoLista.Add(mjesto);
            return mjesto;
        }
        public IEnumerable<Mjesto> DohvatSvihMjesta()
        {
            return _mjestoLista;
        }
    }
}
