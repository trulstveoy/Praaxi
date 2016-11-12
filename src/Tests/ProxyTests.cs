using System;
using System.Linq;
using System.Net.Http;
using Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Web;
using Web.Contracts.Controllers;
using Xunit;

namespace Tests
{
    public class ProxyTests
    {
        private TestServer _testServer;
        private HttpClient _httpClient;
        private IHelloController _controller;

        public ProxyTests()
        {
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _httpClient = _testServer.CreateClient();
            var proxyFactory = new ProxyFactory(_httpClient, "");
            _controller = proxyFactory.Create<IHelloController>();
        }

        [Fact]
        public void GetThrowsException()
        {
            try
            {
                _controller.GetThrowsException();
            }
            catch (AggregateException ex)
            {
                Assert.Contains(typeof(DivideByZeroException), ex.InnerExceptions.Select(x => x.GetType()));
                return;
            }

            throw new InvalidOperationException("Should have thrown AggregateException");
        }

        [Fact]
        public void GetFoo()
        {
            var foos = _controller.GetFoo(2);
            Assert.Equal(2, foos.Count);
        }

        [Fact]
        public void GetWithDateTime()
        {
            var date = DateTime.Now;
            var result = _controller.GetWithDateTime(date);
            Assert.Equal(date.ToString("G"), result.ToString("G"));
        }

        [Fact]
        public void GetWithMultiple()
        {
            var result = _controller.GetWithMultiple(2, "testing");
            Assert.Equal(2, result.Count);
            Assert.Equal("testing", result.First().Bar);
        }
    }
}
