using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Ozon.WebApp.Entities;

namespace Ozon.WebApp.Handlers;

public interface IAddNewClientHandler
{
    void Handle(Request request);
    
    public class Request
    {
        public Request(string name, Gender gender, int age, string hobby)
        {
            Name = name;
            Gender = gender;
            Age = age;
            Hobby = hobby;
        }

        [Required]
        public string Name { get; set; }
        public Gender Gender { get; set;  }
        public int Age { get; set;  }
        [Required]
        public string Hobby { get; set;  }
    }
    
    public class RequestValidator: AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Hobby)
                .NotEmpty();

            RuleFor(x => x.Gender)
                .IsInEnum();

            RuleFor(x => x.Age)
                .NotEmpty()
                .GreaterThan(0)
                .LessThan(120);
        }
    }
}