using System;
using System.Net.Http;
using Castle.DynamicProxy;

namespace Core
{
    public class ControllerProxy
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient = new HttpClient();

        public ControllerProxy(string url)
        {
            _baseUrl = url;
        }

        public T Create<T>() where T : class
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget<T>(new CallInterceptor(Intercept));
            return proxy;
        }

        private void Intercept(IInvocation invocation)
        {

        }

        public void DoShit()
        {
            string url = "http://localhost:57978/api/hello/GetFoo?number=5";
            var response = _httpClient.GetAsync(url).Result;
            var resultAsString = response.Content.ReadAsStringAsync().Result;
        }

        private class CallInterceptor : IInterceptor
        {
            private readonly Action<IInvocation> _intercept;

            public CallInterceptor(Action<IInvocation> intercept)
            {
                _intercept = intercept;
            }

            public void Intercept(IInvocation invocation)
            {
                _intercept(invocation);
            }
        }
    }
}
