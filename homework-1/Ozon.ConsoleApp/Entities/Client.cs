namespace Ozon.ConsoleApp.Entities;

public class Client
{
    public string Name { get; }
    
    public Gender Gender { get; }
    public int Age { get; }
    public string Hobby { get; }

    public Client(string name, Gender gender, int age, string hobby)
    {
        Name = name;
        Gender = gender;
        Age = age;
        Hobby = hobby;
    }
}