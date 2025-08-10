using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace NexAI.Config;

public interface IOptions;

public class Options(IConfiguration configuration)
{
    public TOptions Get<TOptions>() where TOptions : IOptions, new()
    {
        var options = configuration.GetSection(typeof(TOptions).Name.Replace("Options", "")).Get<TOptions>() ?? new TOptions();
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(options, new(options), validationResults, validateAllProperties: true))
        {
            throw new ConfigurationException(validationResults.Select(validationResult => validationResult.ErrorMessage));
        }
        var subOptions = typeof(TOptions).GetProperties().Where(property => property.PropertyType.IsClass && typeof(IOptions).IsAssignableFrom(property.PropertyType));
        foreach (var subOption in subOptions)
        {
            var subOptionValue = subOption.GetValue(options);
            if (subOptionValue == null)
            {
                validationResults.Add(new($"The property '{subOption.Name}' cannot be null.", [subOption.Name]));
                continue;
            }
            Validator.TryValidateObject(subOptionValue, new(subOptionValue), validationResults, validateAllProperties: true);
        }
        if (validationResults.Any())
        {
            throw new ConfigurationException(validationResults.Select(validationResult => validationResult.ErrorMessage));
        }
        return options;
    }
}