using System.Data;
using Npgsql;
using Ozon.Route256.Practice.CustomerService.Dal.Common;
using Ozon.Route256.Practice.CustomerService.Repository.Dto;

namespace Ozon.Route256.Practice.CustomerService.Repository.Impl;

public class CustomerRepository : ICustomerRepository
{
    private readonly IPostgresConnectionFactory _connectionFactory;
    private const string Fields = "id, first_name, last_name, mobile_number, email, address_id, addresses";
    private const string FieldsForInsert = "first_name, last_name, mobile_number, email, address_id, addresses";
    private const string Table = "customers";
    public CustomerRepository(
        IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CustomerDto[]> GetAll(CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table};
        ";
        
        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);
        
        var result = await ReadCustomerDto(reader, token);
        return result.ToArray();
    }

    public async Task<CustomerDto?> Find(int id, CancellationToken token)
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
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);
        
        var result = await ReadCustomerDto(reader, token);
        return result.FirstOrDefault();
    }

    public async Task Create(
        CustomerDto customer,
        CancellationToken token)
    {
        const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            select {FieldsForInsert} from unnest(:models);
        ";
        
        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("models", new[] { customer });

        await connection.OpenAsync(token);
        await command.ExecuteNonQueryAsync(token);
    }

    private static async Task<CustomerDto[]> ReadCustomerDto(
        NpgsqlDataReader reader,
        CancellationToken token)
    {
        var result = new List<CustomerDto>();
        while (await reader.ReadAsync(token))
        {
            result.Add(
                new CustomerDto(
                    Id: reader.GetFieldValue<int>(0),
                    FirstName: reader.GetFieldValue<string>(1),
                    LastName: reader.GetFieldValue<string>(2),
                    MobileNumber: reader.GetFieldValue<string>(3),
                    Email: reader.GetFieldValue<string>(4),
                    AddressID: reader.GetFieldValue<int>(5),
                    Addresses: reader.GetValue(6) as int[] ?? Array.Empty<int>()
                ));
        }
        return result.ToArray();
    }
}