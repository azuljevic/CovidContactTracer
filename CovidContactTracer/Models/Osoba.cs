using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;
using System.ComponentModel.DataAnnotations;

namespace CovidContactTracer.Models
{
    public class Osoba
    {
        public Osoba()
        {

        }
        public Osoba(double id)
        {
            this.id = id;
        }
        public Osoba(INode node)
        {
            this.id = node["id"].As<double>();
            this.ime = node["ime"].As<string>();
            //rezultat testa se pretvara u enum value
            this.rezultat = (RezultatEnum)Enum.Parse(typeof(RezultatEnum), node["rezultat"].As<string>());
            this.vrijemetestiranja = node["vrijemetesta"].As<DateTimeOffset>();
        }

        [Required]
        [Display(Name = "ID")]
        public double id { get; set; }
        [Required]
        [Display(Name = "Ime")]
        public string ime { get; set; }
        [Required]
        [Display(Name = "Rezultat Testa")]
        public RezultatEnum rezultat { get; set; }
        [Display(Name = "Vrijeme Testa")]
     
        public DateTimeOffset vrijemetestiranja { get; set; }

        public Dictionary<string,object> AsDictionary()
        {
            return new Dictionary<string, object>
            {
                {"id",id.ToString() },
                {"ime",ime },
                {"rezultat",rezultat.ToString() },
                {"vrijemetestiranja",vrijemetestiranja },
            };
        }
    }
}
