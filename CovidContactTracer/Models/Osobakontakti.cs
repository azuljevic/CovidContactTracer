using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidContactTracer.Models
{
    public class Osobakontakti
    {
        public Osoba Osoba { get; set; }
        public OsobaRepozitorij Kontakti { get; set; }
    }

}
