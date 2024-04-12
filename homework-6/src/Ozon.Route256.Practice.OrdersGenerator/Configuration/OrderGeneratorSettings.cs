using Ozon.Route256.Practice.OrdersGenerator.Models;

namespace Ozon.Route256.Practice.OrdersGenerator.Configuration;

public class OrderGeneratorSettings
{
    public OrderSource OrderSource { get; set; }
    
    public string OrderRequestTopic { get; set; }
}