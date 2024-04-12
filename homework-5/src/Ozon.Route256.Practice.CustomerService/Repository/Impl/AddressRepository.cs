using System.Data;
using Npgsql;
using Ozon.Route256.Practice.CustomerService.Dal.Common;
using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Repository.Impl;

public class AddressRepository : IAddressRepository
{
    private readonly IPostgresConnectionFactory _connectionFactory;

    private const string Fields = "id, region, city, street, building, apartment, latitude, longitude";
    private const string FieldsForInsert = "region, city, street, building, apartment, latitude, longitude";
    private const string Table = "addresses";

    public AddressRepository(
        IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory    = connectionFactory;
    }


    public async Task<AddressDto?> Find(int id, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id = :id;
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("id", id);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(
            CommandBehavior.SingleRow,
            token);

        var result = await MapToDto(token, reader);

        return result.FirstOrDefault();
    }

    public async Task<AddressDto[]> FindMany(IEnumerable<int> ids, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id = any(:ids::int[]);
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("ids", ids);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await MapToDto(token, reader);
        return result;
    }

    public async Task<AddressDto[]> GetAll(CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table};
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await MapToDto(token, reader);
        return result;
    }

    public async Task<int> Create(
        AddressDto address,
        CancellationToken token)
    {
        const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            values (:region, :city, :street, :building, :apartment, :latitude, :longitude)
            returning id;
        ";
        
        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("region", address.Region);
        command.Parameters.Add("city", address.City);
        command.Parameters.Add("street", address.Street);
        command.Parameters.Add("building", address.Building);
        command.Parameters.Add("apartment", address.Apartment);
        command.Parameters.Add("latitude", address.Latitude);
        command.Parameters.Add("longitude", address.Longitude);

        await connection.OpenAsync(token);
        var id = await command.ExecuteScalarAsync(token);
        return (int) id!;
    }

    private static async Task<AddressDto[]> MapToDto(
        CancellationToken token,
        NpgsqlDataReader reader)
    {
        var result = new List<AddressDto>();
        while (await reader.ReadAsync(token))
        {
            result.Add( new AddressDto(
                Id: reader.GetFieldValue<int>(0),
                Region: reader.GetFieldValue<string>(1),
                City: reader.GetFieldValue<string>(2),
                Street: reader.GetFieldValue<string>(3),
                Building: reader.GetFieldValue<string>(4),
                Apartment: reader.GetFieldValue<string>(5),
                Latitude: reader.GetFieldValue<double>(6),
                Longitude: reader.GetFieldValue<double>(7)
            ));
        }

        return result.ToArray();
    }
}