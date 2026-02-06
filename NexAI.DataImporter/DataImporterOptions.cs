// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.DataImporter;

public class DataImporterOptions : IOptions
{
    [Required]
    public bool UseBackup { get; init; }
    
    [Required]
    public DateTime ZendeskTicketStartDate { get; init; }
}