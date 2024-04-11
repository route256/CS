using System.Text.Json.Serialization;
using Ozon.WebApp.Entities;

namespace Ozon.WebApp.Services;

[JsonSerializable(typeof(Client))]
public partial class ClientGenerationContext: JsonSerializerContext
{
    public static ClientGenerationContext Custom => new ClientGenerationContext(JsonHelper.GetJsonSerializerOptions());
    
}