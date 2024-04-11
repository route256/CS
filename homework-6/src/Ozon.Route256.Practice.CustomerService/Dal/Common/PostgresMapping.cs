using Npgsql;
using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Dal.Common;

public class PostgresMapping
{
    public static void MapCompositeTypes()
    {
        var mapper = NpgsqlConnection.GlobalTypeMapper;
        mapper.MapComposite<AddressDto>("address");
    }
}