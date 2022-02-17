using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidContactTracer.Models
{
    public class OsobaLokacije
    {
        public Osoba osoba { get; set; }
        public MjestoRepozitorij lokacije { get; set; }
    }
}
