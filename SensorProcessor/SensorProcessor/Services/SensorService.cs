using Microsoft.EntityFrameworkCore;
using SensorProcessor.Data;
using SensorProcessor.DTOs;
using SensorProcessor.Models;
using SensorProcessor.Repositories;
using System.Xml;
using System.Xml.Schema;

namespace SensorProcessor.Services;

public class SensorService : ISensorService
{
    private readonly ISensorRepository _repository;
    private readonly ILogger<SensorService> _logger;

    public SensorService(ISensorRepository repository, ILogger<SensorService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SensorDataResponseDto> AddSensorDataAsync(SensorDataDto dataDto)
    {
        var sensorData = new SensorData
        {
            SensorId = dataDto.SensorId,
            Value = dataDto.Value,
            Timestamp = dataDto.Timestamp,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _repository.AddAsync(sensorData);

        return new SensorDataResponseDto
        {
            Id = result.Id,
            SensorId = result.SensorId,
            Value = result.Value,
            Timestamp = result.Timestamp
        };
    }

    public async Task<IEnumerable<SensorDataResponseDto>> GetSensorDataAsync(DateTime start, DateTime end)
    {
        var data = await _repository.GetByTimeRangeAsync(start, end);

        return data.Select(d => new SensorDataResponseDto
        {
            Id = d.Id,
            SensorId = d.SensorId,
            Value = d.Value,
            Timestamp = d.Timestamp
        });
    }

    public async Task<IEnumerable<SensorSummary>> GetSensorsSummaryAsync(DateTime start, DateTime end)
    {
        return await _repository.GetSummaryAsync(start, end);
    }

    public async Task<XmlUploadResponseDto> ProcessXmlDataAsync(Stream xmlStream)
    {
        var validationResult = await ValidateXmlWithXsdAsync(xmlStream);
        if (!validationResult.IsValid)
        {
            return new XmlUploadResponseDto
            {
                IsValid = false,
                Message = "XML validation failed",
                Errors = validationResult.Errors
            };
        }

        try
        {
            return new XmlUploadResponseDto
            {
                IsValid = true,
                Message = "XML uploaded and validated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing XML data");
            return new XmlUploadResponseDto
            {
                IsValid = false,
                Message = "Error processing XML data",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private async Task<(bool IsValid, List<string> Errors)> ValidateXmlWithXsdAsync(Stream xmlStream)
    {
        var errors = new List<string>();

        try
        {
            var xsdSchema = @"
                <xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>
                    <xs:element name='SensorDataCollection'>
                        <xs:complexType>
                            <xs:sequence>
                                <xs:element name='SensorData' maxOccurs='unbounded'>
                                    <xs:complexType>
                                        <xs:sequence>
                                            <xs:element name='SensorId' type='xs:int' minOccurs='1'/>
                                            <xs:element name='Value' type='xs:decimal' minOccurs='1'/>
                                            <xs:element name='Timestamp' type='xs:dateTime' minOccurs='1'/>
                                        </xs:sequence>
                                    </xs:complexType>
                                </xs:element>
                            </xs:sequence>
                        </xs:complexType>
                    </xs:element>
                </xs:schema>";

            var schemaSet = new XmlSchemaSet();
            using var schemaReader = new StringReader(xsdSchema);
            using var xmlSchemaReader = XmlReader.Create(schemaReader);

            var schema = XmlSchema.Read(xmlSchemaReader, (sender, e) =>
            {
                errors.Add($"XSD Schema error: {e.Message}");
            });

            schemaSet.Add(schema);
            schemaSet.Compile();

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet,
                ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
                                XmlSchemaValidationFlags.ProcessSchemaLocation |
                                XmlSchemaValidationFlags.ReportValidationWarnings
            };

            settings.ValidationEventHandler += (sender, e) =>
            {
                var errorType = e.Severity == XmlSeverityType.Error ? "Error" : "Warning";
                errors.Add($"{errorType}: {e.Message} (Line: {e.Exception?.LineNumber}, Position: {e.Exception?.LinePosition})");
            };

            xmlStream.Position = 0;
            using var reader = XmlReader.Create(xmlStream, settings);

            while (await reader.ReadAsync()) { }

            return (errors.Count == 0, errors);
        }
        catch (XmlException ex)
        {
            errors.Add($"XML parsing error: {ex.Message} (Line: {ex.LineNumber}, Position: {ex.LinePosition})");
            return (false, errors);
        }
        catch (Exception ex)
        {
            errors.Add($"Validation error: {ex.Message}");
            return (false, errors);
        }
    }
}