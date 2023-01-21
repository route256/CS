namespace Ozon.ConsoleApp.Entities;

public class Product
{
    public Product(string name) => Name = name;
    public int Id { get; init; }
    
    /// <summary>
    /// Наименование
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Теги
    /// </summary>
    public ICollection<string> Tags { get; init; } = Array.Empty<string>();
    
    /// <summary>
    /// Спонсорский товар
    /// </summary>
    public bool IsSponsored { get; init; }
    
    /// <summary>
    /// Не для детей
    /// </summary>
    public bool Adult { get; init; }
    
    /// <summary>
    /// Возростная категория
    /// </summary>
    public Ages? Age { get; init; }
    
    /// <summary>
    /// Для женщин / для мужчин
    /// </summary>
    public Gender? Gender { get; init; }
}