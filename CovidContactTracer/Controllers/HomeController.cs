using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CovidContactTracer.Models;
using Neo4j.Driver;


namespace CovidContactTracer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDriver _driver;


        public HomeController(IDriver driver)
        {
            _driver = driver;

        }



        public async Task<IActionResult> DohvatPozitivnih()
        {
            IResultCursor cursor;
            var osobe = new List<Osoba>();
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                cursor = await session.RunAsync(@"MATCH(o:Osoba) WHERE o.rezultat = 'Pozitivan' RETURN o.id as id, o.ime as ime, o.rezultat as rezultat,o.vrijemetesta as vrijemetesta ORDER BY o.vrijemetesta DESC");

                await cursor.ForEachAsync(record =>
                {
                    var osoba = new Osoba()
                    {
                        id = record["id"].As<double>(),
                        ime = record["ime"].As<string>(),
                        //rezultat testa se pretvara u enum value
                        rezultat = (RezultatEnum)Enum.Parse(typeof(RezultatEnum), record["rezultat"].As<string>()),
                        vrijemetestiranja = record["vrijemetesta"].As<DateTimeOffset>()
                    };
                    osobe.Add(osoba);
                });



            }
            finally
            {
                await session.CloseAsync();
            }
            return View(osobe);
        }
        [HttpPost]
        public async Task<IActionResult> KontaktiOsobe(string id)
        {
               
            OsobaRepozitorij kontakti = new OsobaRepozitorij();
            IResultCursor cursor;
            Osoba osoba = null;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                //RunAsync - asynchronously execute a query and return a task of result stream Task<IResultCursor> 
                //Dohvat podataka osobe (o1) preko id-a
                cursor = await session.RunAsync(@"MATCH (o1:Osoba {id:$id}) RETURN o1", new { id });
                IRecord rec = await cursor.SingleAsync();
                osoba = new Osoba(rec["o1"].As<INode>());

                //Dohvat svih osoba (o2) koje su bile na istoj lokaciji s osobom (01)
                cursor = await session.RunAsync(@"MATCH (o1:Osoba {id:$id})-[p1:POSJETE]->(m:Mjesto)<-[p2:POSJETE]-(o2:Osoba) WITH o2, apoc.coll.max([p1.vrijemepocetak.epochMillis, p2.vrijemepocetak.epochMillis]) AS maxStart,apoc.coll.min([p1.vrijemekraj.epochMillis, p2.vrijemekraj.epochMillis]) AS minEnd WHERE maxStart<=minEnd RETURN o2 ORDER BY o2.id DESC", new { id });
                await cursor.ForEachAsync(record =>
                {
                    Osoba kontakt = new Osoba(record["o2"].As<INode>());
                    kontakti.Dodaj(kontakt);
                });
            }
            catch 
            {
                return View("Privacy");
            }
            finally
            {
                await session.CloseAsync();
            }
            var osobaKontakti = new Osobakontakti
            {
                Osoba = osoba,
                Kontakti = kontakti
            };
            return View(osobaKontakti);
        }

        [HttpPost]
        public async Task<IActionResult> IspravakOsobe2(string id)
        {
            //string id = "81";     

            IResultCursor cursor;
            Osoba osoba = null;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                //RunAsync - asynchronously execute a query and return a task of result stream Task<IResultCursor> 
                //Dohvat podataka osobe (o1) preko id-a
                cursor = await session.RunAsync(@"MATCH (o1:Osoba {id:$id}) RETURN o1", new { id });
                IRecord rec = await cursor.SingleAsync();
                osoba = new Osoba(rec["o1"].As<INode>());

                
                
            }
            catch 
            {
                return View("Privacy");
            }
            finally
            {
                await session.CloseAsync();
            }
            return View(osoba);
        }

            //Create extended model for the view
           



        [HttpPost]
        public async Task<IActionResult> UnosPosjete(Posjeta posjeta)
        {
            IResultCursor cursor;
            IResultCursor cursor2;
            IResultCursor cursor3;
            var osoba = new Osoba(posjeta.idosobe);
            IAsyncSession session = _driver.AsyncSession();
            string imemjesta = posjeta.imemjesta;
            var mjesta = new MjestoRepozitorij();

            try
            {

                cursor3 = await session.RunAsync(@"MATCH(m:Mjesto) RETURN m");
                await cursor3.ForEachAsync(record =>
                {

                    Mjesto mjesto = new Mjesto(record["m"].As<INode>());
                    mjesta.Add(mjesto);
                });
                cursor = await session.RunAsync(@"MATCH (p:Posjeta) RETURN MAX(p.id)");
                await cursor.ForEachAsync(record =>
                {

                    posjeta.id = (record["MAX(p.id)"].As<int>()) + 1;

                });
                cursor2 = await session.RunAsync(@"MATCH (m1:Mjesto {ime:$imemjesta}) RETURN m1.id", new { imemjesta });
                await cursor2.ForEachAsync(record =>
                {
                    posjeta.idmjesta = record["m1.id"].As<int>();
                });


                await session.RunAsync(@"MATCH (o:Osoba {id:$po.idosobe}), (m:Mjesto {id:$po.idmjesta}) CREATE (o) -[:POSJECUJE]->(p:Posjeta {id:$po.id, vrijemepocetak:$po.vrijemepocetak, vrijemekraj:$po.vrijemekraj})-[:LOKACIJE]->(m) create(o)-[po:POSJETE {id:$po.id, vrijemepocetak:$po.vrijemepocetak, vrijemekraj:$po.vrijemekraj}]->(m) SET p.trajanje=duration.inSeconds(p.vrijemepocetak,p.vrijemekraj) SET po.trajanje=duration.inSeconds(po.vrijemepocetak,po.vrijemekraj) ", new { po = posjeta.AsDictionary()});
            }
            finally
            {
                await session.CloseAsync();

            }
            var osobaLokacije = new OsobaLokacije
            {
                osoba = osoba,
                lokacije = mjesta
            };

            return View("UnosOsobe" ,osobaLokacije);
        }
        public async Task<IActionResult> DohvatKontakata()
        {
            return View();
        }

        public async Task<IActionResult> IspravakOsobe()
        {
            return View();
        }
        public async Task<IActionResult> UnosMjesta()
        {
           return View();
        }
        [HttpPost]
        public async Task<IActionResult> UnosOsobe(Osoba osoba)
        {
            IResultCursor cursor;
            
            var mjesta = new MjestoRepozitorij();
            
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                await session.RunAsync(@"CREATE (osoba: Osoba {id:$os.id,ime:$os.ime,rezultat:$os.rezultat,vrijemetesta:$os.vrijemetestiranja})", new { os = osoba.AsDictionary() });
                cursor = await session.RunAsync(@"MATCH(m:Mjesto) RETURN m");

                await cursor.ForEachAsync(record =>
                {

                    Mjesto mjesto = new Mjesto(record["m"].As<INode>());
                    mjesta.Add(mjesto);
                });
            }
            finally
            {
                await session.CloseAsync();

            }
            var osobaLokacije = new OsobaLokacije
            {
                osoba = osoba,
                lokacije = mjesta
            };
            return View(osobaLokacije);
        }
        [HttpPost]
        public async Task<IActionResult> IspravakOsobe3(Osoba osoba)
        {
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                await session.RunAsync(@"MERGE (osoba: Osoba {id:$os.id}) SET osoba.id=$os.id, osoba.ime=$os.ime, osoba.rezultat=$os.rezultat, osoba.vrijemetesta=$os.vrijemetestiranja", new { os = osoba.AsDictionary() });
            }
            finally
            {
                await session.CloseAsync();

            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ObrisiOsobu(Osoba osoba)
        {
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                await session.RunAsync(@"MATCH (n: Osoba {id:$os.id}) DETACH DELETE n", new { os = osoba.AsDictionary() });
            }
            finally
            {
                await session.CloseAsync();

            }
            return View("IspravakOsobe3");
        }
        [HttpPost]
        public async Task<IActionResult> UnesiMjesto(Mjesto mjesto)
        {
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                await session.RunAsync(@"CREATE (mjesto: Mjesto {id:$mj.id,ime:$mj.ime,opis:$mj.opis})", new { mj = mjesto.AsDictionary()});
            }
            finally
            {
                await session.CloseAsync();

            }
            return View("UnosMjesta");
        }
        
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
       

        public async Task<IActionResult> DohvatMjesta()
        {
            IResultCursor cursor;
            var mjesta = new List<Mjesto>();
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                cursor = await session.RunAsync(@"MATCH(m:Mjesto) RETURN m");

                await cursor.ForEachAsync(record =>
                {
                    
                    Mjesto mjesto = new Mjesto(record["m"].As<INode>());
                    mjesta.Add(mjesto);
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return View(mjesta);
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}


