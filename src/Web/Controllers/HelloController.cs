using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Web.Contracts.Controllers;
using Web.Contracts.Hello;

namespace Praaxi.Controllers
{
    [Route("api/[controller]")]
    public class HelloController : Controller, IHelloController
    {
        [HttpGet]
        [Route("GetFoo")]
        public List<Foo> GetFoo(int number)
        {
            var list = Enumerable.Range(0, number).Select(i => new Foo { Bar = i.ToString() }).ToList();
            return list;
        }

        public List<Foo> GetWithoutArguments()
        {
            return new List<Foo>
            {
                new Foo
                {
                    Bar = "bar"
                }
            };
        }

        public List<Foo> GetWithMultiple(int number, string bar)
        {
            var list = Enumerable.Range(0, number).Select(i => new Foo { Bar = bar }).ToList();
            return list;
        }

        public DateTime GetWithDateTime(DateTime dateTime)
        {
            return dateTime;
        }

        public void PostFoo(Foo foo)
        { }

        public Foo PostWithReturnType(Foo foo)
        {
            return foo;
        }

        public List<Foo> GetThrowsException()
        {
            throw new DivideByZeroException();
        }
    }
}
