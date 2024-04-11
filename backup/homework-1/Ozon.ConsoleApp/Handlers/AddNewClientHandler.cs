using System.ComponentModel.DataAnnotations;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Services;

namespace Ozon.ConsoleApp.Handlers;

public interface IAddNewClientHandler
{
    void Handle(Request request);
    
    public class Request
    {
        [Display(Name = "name")]
        public string? Name { get; set; }
    
        [Display(Name = "gender")]
        public string? Gender { get; set;  }

        [Display(Name = "age")]
        public int? Age { get; set;  }
    
        [Display(Name = "hobby")]
        public string? Hobby { get; set;  }
    }
}

internal sealed class AddNewClientHandler : IAddNewClientHandler
{
    private readonly IClientStorage _clientStorage;

    public AddNewClientHandler(IClientStorage clientStorage)
    {
        _clientStorage = clientStorage;
    }

    public void Handle(IAddNewClientHandler.Request request)
    {
        if (string.IsNullOrEmpty(request.Name))
            throw new ArgumentException(request.Name, nameof(request.Name));
        
        if (string.IsNullOrEmpty(request.Hobby))
            throw new ArgumentException(request.Hobby, nameof(request.Hobby));

        if (!Enum.TryParse<Gender>(request.Gender, out var gender))
            throw new ArgumentException(request.Gender, nameof(request.Gender));
        
        ArgumentNullException.ThrowIfNull(request.Age);

        var client = new Client(request.Name, gender, request.Age.Value, request.Hobby);
        _clientStorage.Save(client);
    }
}