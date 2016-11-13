using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Binding
{
    public class ComplexTypeConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action.ActionMethod.Name.StartsWith("Post"))
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