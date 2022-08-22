using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Tests.UnitTesting;

[TestClass]
public class FirstCapitalLetterAttributeTests
{
    [TestMethod]
    public void FirstLowerCaseLetterReturnError()
    {
        //Preparación
        var firstCapitalLetter = new FirstCapitalLetterAttribute();
        const string value = "felipe";
        var valContext = new ValidationContext(new { Name = value });

        //Ejecución
        var result = firstCapitalLetter.GetValidationResult(value, valContext);

        //Verificación

        Assert.AreEqual("La primera letra debe ser mayúscula", result?.ErrorMessage);
    }

    [TestMethod]
    public void NullValueNotReturnError()
    {
        //Preparación
        var firstCapitalLetter = new FirstCapitalLetterAttribute();
        const string value = null!;
        var valContext = new ValidationContext(new { Name = value });

        //Ejecución
        var result = firstCapitalLetter.GetValidationResult(value, valContext);

        //Verificación
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FirstCapitalLetterNotReturnError()
    {
        //Preparación
        var firstCapitalLetter = new FirstCapitalLetterAttribute();
        const string value = "Leonardo";
        var valContext = new ValidationContext(new { Name = value });

        //Ejecución
        var result = firstCapitalLetter.GetValidationResult(value, valContext);

        //Verificación
        Assert.IsNull(result);
    }
}