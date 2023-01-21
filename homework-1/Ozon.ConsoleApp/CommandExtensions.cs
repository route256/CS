using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Ozon.ConsoleApp;

public static class CommandExtensions
{
    private static readonly MethodInfo GenericMethod = typeof(CommandExtensions)
        .GetMethod(nameof(CreateOptionWithValidator), BindingFlags.Static | BindingFlags.Public)!;

    public static void SetRequestHandler<T>(this Command command, Action<T> handle)
    {
        command.SetRequestHandler(new Func<T,Task>(x =>
        {
            handle.Invoke(x);
            return Task.CompletedTask;
        }));
    }

    private static void SetRequestHandler<T>(this Command command, Func<T, Task> handle)
    {
        var type = typeof(T);
        var constructors = type.GetConstructors();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        if (constructors.Any(constructor => !constructor.GetParameters().Any()))
        {
            var propertyOptions = GetPropertyOption(command, properties).ToArray();
            command.SetHandler(async x =>
            {
                var request = GetRequest<T>(propertyOptions, x);
                await handle.Invoke(request);
            });
        }
        else
        {
            if (constructors.Length != 1)
                throw new Exception("Cant find constructor");

            var constructor = constructors.Single();
            var constructorParams = constructor.GetParameters();
            var parameters = GetParameterInfoOption(command, constructorParams, properties).ToArray();
            
            command.SetHandler(async x =>
            {
                var parameterValues = parameters.Select(parameter => x.ParseResult.GetValueForOption(parameter.Option));
                var request = (T)constructor.Invoke(parameterValues.ToArray());
                await handle.Invoke(request);
            });
        }
    }

    private static T GetRequest<T>(PropertyOption[] parameters, InvocationContext x)
    {
        var request = Activator.CreateInstance<T>();
        foreach (var parameterPropertyInfo in parameters)
        {
            var value = x.ParseResult.GetValueForOption(parameterPropertyInfo.Option);
            parameterPropertyInfo.PropertyInfo.SetValue(request, value);
        }

        return request;
    }

    private static IEnumerable<PropertyOption> GetPropertyOption(Command command, IEnumerable<PropertyInfo> properties)
    {
        return properties
            .Select(property => (property, displayAttribute: property.GetCustomAttribute<DisplayAttribute>()))
            .Where(x => x.displayAttribute != null)
            .Select(x =>
            {
                var (property, displayAttribute) = x;
                
                if (displayAttribute == null)
                    throw new Exception("Unreachable exception");
                
                var option = GetOption(command, property.PropertyType, displayAttribute);
                return new PropertyOption(property, option);
            }).ToArray();
    }

    private static IEnumerable<ParameterInfoOption> GetParameterInfoOption(
        Command command,
        IEnumerable<ParameterInfo> parameterInfos,
        ICollection<PropertyInfo> properties)
    {
        return parameterInfos
            .Select(parameterInfo =>
            {
                var attribute = parameterInfo.GetCustomAttribute<DisplayAttribute>() ??
                                GetPropertyByParameter(properties, parameterInfo)
                                    ?.GetCustomAttribute<DisplayAttribute>();
                
                return (parameter: parameterInfo, displayAttribute: attribute);
            })
            .Where(x => x.displayAttribute != null)
            .Select(x =>
            {
                var (property, displayAttribute) = x;

                if (displayAttribute == null)
                    throw new Exception("Unreachable exception");
                
                var optionDescriptor = GetOption(command, property.ParameterType, displayAttribute);
                return new ParameterInfoOption(property, optionDescriptor);
            }).ToArray();
    }

    private static PropertyInfo? GetPropertyByParameter(IEnumerable<PropertyInfo> properties, ParameterInfo parameterInfo) 
        => properties.SingleOrDefault(propertyInfo => propertyInfo.Name == parameterInfo.Name);

    private static Option GetOption(Command command, Type type, DisplayAttribute displayAttribute)
    {
        var genericMethod = GenericMethod.MakeGenericMethod(type);
        var option = (Option)genericMethod.Invoke(null,
            new object?[] { command, displayAttribute.Name, displayAttribute.Description })!;
        return option;
    }

    public static Option<T> CreateOptionWithValidator<T>(this Command customerCommand, string name, string description)
    {
        var option = new Option<T>(name, description);
        customerCommand.AddOption(option);
        customerCommand.AddValidator(x => ValidateValue(name, x, option));
        return option;
    }

    private static void ValidateValue<T>(string name, CommandResult x, Option<T> option)
    {
        try
        {
            var value = x.GetValueForOption(option);
            if (value == null)
                x.ErrorMessage = $"Parameter \"{name}\" must be defined";
        }
        catch (Exception e)
        {
            x.ErrorMessage = e.Message;
        }
    }

    private record PropertyOption(PropertyInfo PropertyInfo, Option Option);
    private record ParameterInfoOption(ParameterInfo ParameterInfo, Option Option);
}