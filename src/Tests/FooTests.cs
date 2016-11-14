using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Web.Binding;
using Web.Controllers;
using Xunit;

namespace Tests
{
    public class FooTests
    {
        [Fact]
        public void Bar()
        {
            var foo = nameof(HelloController.ThisIsAPost);
            MethodInfo methodInfo = typeof(HelloController).GetMethod(nameof(HelloController.ThisIsAPost));

            var list = methodInfo.GetCustomAttributes(true).ToList();

            var attributes = new List<object>();
            ActionModel actionModel = new ActionModel(methodInfo, attributes);

            var target = new BindPostedParametersConvention();
            target.Apply(actionModel);
        }
    }
}