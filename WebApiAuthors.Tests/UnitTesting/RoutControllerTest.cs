using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApiAuthors.Controllers.V1;
using WebApiAuthors.Tests.Mocks;

namespace WebApiAuthors.Tests.UnitTesting;

[TestClass]
public class RoutControllerTest
{
    [TestMethod]
    public async Task UserIsAdmid_Get4links()
    {
        //preparación
        var authoizationService = new AuthorizationMock
        {
            Result = AuthorizationResult.Success()
        };

        var rootController = new RoutesController(authoizationService)
        {
            Url = new UrlHelperMock()
        };

        //Ejecución
        var result = await rootController.Get();

        //Verificación
        if (result.Value != null) Assert.AreEqual(4, result.Value.Count());
    }

    [TestMethod]
    public async Task UserNotAdmid_Get2links()
    {
        //preparación
        var authoizationService = new AuthorizationMock
        {
            Result = AuthorizationResult.Failed()
        };

        var rootController = new RoutesController(authoizationService)
        {
            Url = new UrlHelperMock()
        };

        //Ejecución
        var result = await rootController.Get();

        //Verificación
        if (result.Value != null) Assert.AreEqual(2, result.Value.Count());
    }

    [TestMethod]
    public async Task UserNotAdmid_Get2linksUsingMoq()
    {
        //preparación
        var mockAuthorizationService = new Mock<IAuthorizationService>();
        mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(),
            It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

        mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(),
            It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(string.Empty);

        var rootController = new RoutesController(mockAuthorizationService.Object)
        {
            Url = mockUrlHelper.Object
        };

        //Ejecución
        var result = await rootController.Get();

        //Verificación
        if (result.Value != null) Assert.AreEqual(2, result.Value.Count());
    }
}
