using Dapper;
using Ozon.Route256.Practice.CustomerService.Application;
using Ozon.Route256.Practice.CustomerService.Domain.Customer;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Dal.Common.Shard;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Repository.Dto;
using Ozon.Route256.Practice.CustomerService.Infrastructure.Repository.Extensions;

namespace Ozon.Route256.Practice.CustomerService.Infrastructure.Repository.Impl;

public class ShardCustomerRepository : BaseShardRepository, ICustomerRepository
{
    public ShardCustomerRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule): base(connectionFactory, longShardingRule, stringShardingRule)
    {
    }

    private const string Fields = "id, first_name as FirstName, last_name as LastName, mobile_number as MobileNumber, email";
    private const string FieldsToInsert = "id, first_name, last_name, mobile_number, email";
    private const string Table = $"{Shards.BucketPlaceholder}.customers";
    private const string LastNameIndex = $"{Shards.BucketPlaceholder}.idx_customers_last_name";

    public async Task<Customer[]> GetAll(
        CancellationToken token)
    {
        var result = new List<CustomerDto>();

        foreach (var bucketId in _connectionFactory.GetAllBuckets())
        {
            const string sql = @$"
                select {Fields}
                from {Table};
            ";

            await using var connection = await GetConnectionByBucket(bucketId, token);
            var customers = await connection.QueryAsync<CustomerDto>(sql);
            result.AddRange(customers);
        }

        return result.Select(customer => customer.ToDomain()).ToArray();
    }

    public async Task<Customer?> Find(
        int id,
        CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id = :id;
        ";
        
        await using var connection = GetConnectionByShardKey(id);
        var param = new DynamicParameters();
        param.Add("id", id);
        var result = await connection.QueryFirstOrDefaultAsync<CustomerDto>(sql, param);
        return result?.ToDomain();
    }

    public async Task Create(
        Customer customer,
        CancellationToken token)
    {
        const string sql = @$"
            insert into {Table} ({FieldsToInsert})
            values (:id, :firstName, :lastName, :mobileNumber, :email);
        ";

        var dto = customer.ToDto();

        var param = new DynamicParameters();
        param.Add("id", dto.Id);
        param.Add("firstName", dto.FirstName);
        param.Add("lastName", dto.LastName);
        param.Add("mobileNumber", dto.MobileNumber);
        param.Add("email", dto.Email);
        
        await using (var connection = GetConnectionByShardKey(dto.Id))
        {
            var cmd = new CommandDefinition(
                sql,
                param,
                cancellationToken: token);
            await connection.ExecuteAsync(cmd);

        }
        
        // -------------------------

        const string indexSql = $@"
            insert into  {LastNameIndex} (last_name, customer_id)
            VALUES (:LastName, :Id)
        ";
        
        await using (var connection = GetConnectionBySearchKey(dto.LastName))
        {
            await connection.ExecuteAsync(indexSql, new { dto.Id, dto.LastName });
        }
    }

    public async Task<Customer[]> FindByLastName(
        string lastName,
        CancellationToken token)
    {
        const string indexSql = @$"
            select customer_id 
            from {LastNameIndex}
            where last_name = :lastName
        ";

        IEnumerable<int> customerIds;
        await using (var connectionIndex = GetConnectionBySearchKey(lastName))
        {
            customerIds = await connectionIndex.QueryAsync<int>(indexSql, new { lastName });
        }

        const string sql = $@"
            select {Fields}
            from {Table}
            where id = any(:ids)
        ";

        var bucketToIdsMap = customerIds
            .Select(id => (BucketId: _longShardingRule.GetBucketId(id), Id: id))
            .GroupBy(x => x.BucketId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Id).ToArray());

        var result = new List<CustomerDto>();
        foreach (var (bucketId, idsInBucket) in bucketToIdsMap)
        {
            await using var connection = await GetConnectionByBucket((int)bucketId, token);
            var customersInBucket = await connection.QueryAsync<CustomerDto>(sql, new { ids = idsInBucket });
            result.AddRange(customersInBucket);
        }
        return result.Select(customer => customer.ToDomain()).ToArray();
    }
}