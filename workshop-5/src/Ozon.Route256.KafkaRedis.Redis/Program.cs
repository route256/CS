// See https://aka.ms/new-console-template for more information

using System;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

using var connection = await ConnectionMultiplexer.ConnectAsync("localhost");

var db = connection.GetDatabase();

// await db.StringSetAsync("route256", new RedisValue("workshop5"));

var value = await db.StringGetAsync("route256");
Console.WriteLine(value);


IServiceCollection collection = new ServiceCollection();

collection.AddStackExchangeRedisCache(d => d.Configuration = "localhost");




