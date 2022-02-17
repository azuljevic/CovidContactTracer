using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidContactTracer.Models
{
    public interface IOsobaRepozitorij
    {
        Osoba DohvatOsoba(int Id);
        IEnumerable<Osoba> DohvatSvihOsoba();
        Osoba Dodaj(Osoba osoba);
        Osoba Promijeni(Osoba osobaPromjene);
        Osoba Izbrisi(int Id);






    }
}
