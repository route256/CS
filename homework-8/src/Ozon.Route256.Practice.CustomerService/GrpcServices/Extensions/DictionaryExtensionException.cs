namespace Ozon.Route256.Practice.CustomerService.GrpcServices.Extensions;

public class DictionaryExtensionException : Exception
{
    public DictionaryExtensionException(string error) : base(error)
    {
    }
}