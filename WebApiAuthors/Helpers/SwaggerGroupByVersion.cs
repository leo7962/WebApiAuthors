using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAuthors.Helpers;

public class SwaggerGroupByVersion : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var nameSpaceController = controller.ControllerType.Namespace; //controllers.v1
        var versionApi = nameSpaceController?.Split('.').Last().ToLower(); // v1
        controller.ApiExplorer.GroupName = versionApi;
    }
}
