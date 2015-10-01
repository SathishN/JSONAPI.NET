using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JSONAPI.AcceptanceTests.EntityFrameworkTestWebApp.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }

        public string StreetLine1 { get; set; }

        public string StreetLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }
    }
}

