using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core
{
    public class ProxyFactory
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public ProxyFactory(HttpClient httpClient, string url)
        {

            _httpClient = httpClient;
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
            var arguments = GetArguments(invocation);
            var controller = GetController(invocation);
            var returnType = invocation.Method.ReturnType;
            var action = invocation.Method.Name;
            var restOperation = GetRestOperation(invocation.Method);

            if(restOperation == RestOperation.GET)
            {
                Get(controller, action, arguments, returnType, invocation);
            }
            else if(restOperation == RestOperation.POST)
            {
                Post(controller, action, arguments, returnType, invocation);
            }
        }

        private RestOperation GetRestOperation(MethodInfo method)
        {
            var attributes = method.CustomAttributes.Select(x => x.AttributeType).ToList();
            if(attributes.Contains(typeof(HttpGetAttribute)) || method.Name.StartsWith("Get"))
                return RestOperation.GET;

            if(attributes.Contains(typeof(HttpPostAttribute)) || method.Name.StartsWith("Post"))
                return RestOperation.POST;

            throw new InvalidOperationException($"Unknown rest operation for operation {method.Name}");
        }

        private void Post(string controller, string action, Dictionary<string, object> arguments, Type returnType, IInvocation invocation)
        {
            string url = $"{_baseUrl}/api/{controller}/{action}";
            System.Diagnostics.Debug.WriteLine(url);
            var json = JsonConvert.SerializeObject(arguments.First().Value);
            System.Diagnostics.Debug.WriteLine(json);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var resultAsString = _httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            if (returnType != typeof(void))
            {
                if (returnType == typeof(string))
                {
                    invocation.ReturnValue = resultAsString;
                }
                else
                {
                    var resultInstance = JsonConvert.DeserializeObject(resultAsString, returnType);
                    invocation.ReturnValue = resultInstance;
                }
            }
        }

        private void Get(string controller, string action, Dictionary<string, object> arguments, Type returnType, IInvocation invocation)
        {
            string querystring = arguments.Count > 0 ?
                "?" + string.Join("&", arguments.Select(x => $"{x.Key}={ParseArgument(x.Value)}"))
                : "";
            var url = $"{_baseUrl}/api/{controller}/{action}{querystring}";
            System.Diagnostics.Debug.WriteLine(url);

            var reply = _httpClient.GetAsync(url).Result;
            string resultAsString = reply.Content.ReadAsStringAsync().Result;
            System.Diagnostics.Debug.WriteLine(resultAsString);
            if (!reply.IsSuccessStatusCode)
            {
                throw new PraaxyException(url, resultAsString, reply.StatusCode);
            }

            var resultInstance = JsonConvert.DeserializeObject(resultAsString, returnType);
            invocation.ReturnValue = resultInstance;
        }

        private string ParseArgument(object obj)
        {
            if (obj == null)
                return "";

            return obj.ToString();
        }

        private string GetController(IInvocation invocation)
        {
            string interfaceName = invocation.Method.DeclaringType.Name;
            var controller = interfaceName.TrimStart('I').Replace("Controller", "");
            return controller;
        }

        private static Dictionary<string,object> GetArguments(IInvocation invocation)
        {
            var parameters = invocation.Method.GetParameters();
            var parameterNames = parameters.Select(x => x.Name).ToArray();
            var pairs = Enumerable.Range(0, parameterNames.Count())
                .Select(i => new { Name =  parameterNames[i], Value = invocation.Arguments[i]})
                .ToDictionary(x => x.Name, y => y.Value);
            return pairs;
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

        private enum RestOperation
        {
            GET,
            POST
        }
    }
}
