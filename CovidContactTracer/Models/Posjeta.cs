using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Neo4j.Driver;

namespace CovidContactTracer.Models
{
    public class Posjeta
    {
        public Posjeta()
        {

        }
        public Posjeta(INode node)
        {
            this.idosobe = node["id"].As<int>();
            this.idmjesta = node["id"].As<int>();
            this.id = node["id"].As<int>();
            this.vrijemepocetak = node["vrijemepocetak"].As<DateTimeOffset>();
            this.vrijemekraj = node["vrijemekraj"].As<DateTimeOffset>();
            this.trajanje = node["trajanje"].As<int>();
        }
        public int idosobe { get; set; }
        public int idmjesta { get; set; }
        public string imemjesta { get; set; }
        public int id { get; set; }
        public DateTimeOffset vrijemepocetak { get; set; }
        public DateTimeOffset vrijemekraj { get; set; }
        public int trajanje { get; set; }
        public Dictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object>
            {
                {"idosobe",idosobe.ToString()},
                {"idmjesta",idmjesta},
                {"id",id },
                {"vrijemepocetak",vrijemepocetak },
                {"vrijemekraj",vrijemekraj },
                {"trajanje",trajanje }
                
            };
        }
    }
}

