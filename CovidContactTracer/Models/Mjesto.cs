using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;
using System.ComponentModel.DataAnnotations;

namespace CovidContactTracer.Models
{
    public class Mjesto
    {
        public Mjesto()
        {

        }
        public Mjesto(INode node)
        {
            
            this.ime = node["ime"].As<string>();
            this.id = node["id"].As<int>();
            this.opis = node["opis"].As<string>();
        }
        
        [Required]
        [Display(Name = "ID")]
        public int id { get; set; }
        [Required]
        [Display(Name = "ime")]
        public string ime { get; set; }
        [Required]
        [Display(Name = "opis")]
        public string opis { get; set; }

        public Dictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object>
            {
                {"id",id },
                {"ime",ime },
                {"opis",opis },

            };
        }

    }
}
