using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Binding
{
    public class BindPostedParametersConvention : IActionModelConvention
    {
        private bool HasPostAttribute(MethodInfo methodInfo)
        {
            var attributeExists = new Func<MethodInfo, bool>(m => m.GetCustomAttributes<HttpPostAttribute>(true).FirstOrDefault() != null);

            var interfaceMethod = methodInfo.DeclaringType.GetInterfaces()
                .Where(i => i.GetMethod(methodInfo.Name) != null)
                .Select(m => m.GetMethod(methodInfo.Name)).FirstOrDefault();

            return attributeExists(methodInfo) || (interfaceMethod != null && attributeExists(interfaceMethod));
        }

        public void Apply(ActionModel action)
        {
            if(HasPostAttribute(action.ActionMethod) || action.ActionMethod.Name.StartsWith("Post"))
            {
                foreach (var parameter in action.Parameters)
                {
                    parameter.BindingInfo = new BindingInfo
                    {
                        BindingSource = BindingSource.Body
                    };
                }
            }
        }
    }
}