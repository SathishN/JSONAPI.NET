using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Http;
using JSONAPI.AcceptanceTests.EntityFrameworkTestWebApp.Models;

namespace JSONAPI.AcceptanceTests.EntityFrameworkTestWebApp.Controllers
{
    public class PersonsController : ApiController
    {
        public IHttpActionResult GetPersons()
        {
            var persons = new List<Person>();

            persons.Add(new Person() { Id = 1,Name = "Tim", Address = new Address() { Id = 1,StreetLine1 = "12 Dallas Pkwy", StreetLine2 = "APT 213", City = "Dallas", State = "TX", Zip = "75252"}});
            persons.Add(new Person() { Id = 2, Name = "Jack", Address = new Address() { Id = 2, StreetLine1 = "12 Dallas Pkwy", StreetLine2 = "APT 214", City = "Dallas", State = "TX", Zip = "75252" } });
            persons.Add(new Person() { Id = 3, Name = "Martin", Address = new Address() { Id = 3, StreetLine1 = "12 Dallas Pkwy", StreetLine2 = "APT 215", City = "Dallas", State = "TX", Zip = "75252" } });
            
            return Ok(persons.ToArray());
        }
    }
}