using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Framework.WebApi.Contracts;

public class RestApiConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                if (action.Attributes.Any(z => z.GetType() == typeof(HttpPostAttribute)))
                    action.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status201Created));

                action.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiMessageResponse), StatusCodes.Status400BadRequest));
                action.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiMessageResponse), StatusCodes.Status401Unauthorized));
                action.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiMessageResponse), StatusCodes.Status403Forbidden));
                action.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiMessageResponse), StatusCodes.Status422UnprocessableEntity));
            }
        }
    }
}
