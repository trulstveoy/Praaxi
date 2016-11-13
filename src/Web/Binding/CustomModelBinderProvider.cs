using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Binding
{
    public class CustomModelBinderProvider: IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.IsComplexType)
            {
                //return new CustomModelBinder();
            }

            return null;
        }
    }
}