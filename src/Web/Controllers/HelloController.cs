using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Web.Contracts.Controllers;
using Web.Contracts.Hello;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class HelloController : Controller, IHelloController
    {

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

        public void PostFoo([FromBody]Foo foo)
        { }

        public Foo PostWithReturnType([FromBody]Foo foo)
        {
            return foo;
        }

        public List<Foo> GetThrowsException()
        {
            throw new DivideByZeroException();
        }
    }
}
