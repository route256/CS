using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Ozon.WebApp.Entities;
using Ozon.WebApp.Exceptions;
using Ozon.WebApp.Services;

namespace Ozon.WebApp.Handlers;

[Controller]
[Route("client")]
[AppExceptionFilter]
[ProducesResponseType(typeof(ReturnObject), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ReturnObject), StatusCodes.Status500InternalServerError)]
public sealed class AddNewClientHandler : IAddNewClientHandler
{
    private readonly IClientStorage _clientStorage;
    private readonly IAddNewClientHandler.RequestValidator _validator;

    public AddNewClientHandler(
        IClientStorage clientStorage, 
        IAddNewClientHandler.RequestValidator validator)
    {
        _clientStorage = clientStorage;
        _validator = validator;
    }

    [HttpPost]
    public void Handle([FromBody] IAddNewClientHandler.Request request)
    {
        _validator.ValidateAndThrow(request);
        
        var client = new Client(request.Name, request.Gender, request.Age, request.Hobby);
        _clientStorage.Save(client);
    }
}