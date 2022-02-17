using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidContactTracer.Models
{
    public class OsobaRepozitorij : IOsobaRepozitorij
    {

        private List<Osoba> _osobaLista;
        public OsobaRepozitorij()
        {
            _osobaLista = new List<Osoba> { };
        }

        public Osoba Dodaj(Osoba osoba)
        {
            _osobaLista.Add(osoba);
            return osoba;
        }

        public Osoba DohvatOsoba(int Id)
        {
            return _osobaLista.FirstOrDefault(e => e.id == Id);
        }

        public IEnumerable<Osoba> DohvatSvihOsoba()
        {
            return _osobaLista;
        }

        public Osoba Izbrisi(int Id)
        {
            Osoba osoba = _osobaLista.FirstOrDefault(e => e.id == Id);
            if (osoba != null)
            {
                _osobaLista.Remove(osoba);
            }
            return osoba;
        }

        public Osoba Promijeni(Osoba osobaPromjene)
        {
            Osoba osoba = _osobaLista.FirstOrDefault(e => e.id == osobaPromjene.id);
            if (osoba != null)
            {
                osoba.ime = osobaPromjene.ime;
                osoba.rezultat = osobaPromjene.rezultat;
                osoba.vrijemetestiranja = osobaPromjene.vrijemetestiranja;
            }
            return osoba;
        }
    }


        
}

