using System.Text.Json.Serialization;
using Ozon.ConsoleApp.Entities;

namespace Ozon.ConsoleApp.Services;

[JsonSerializable(typeof(Client))]
public partial class ClientGenerationContext: JsonSerializerContext
{
    public static ClientGenerationContext Custom => new ClientGenerationContext(JsonHelper.GetJsonSerializerOptions());
    
}