using CsvHelper;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MicroFun.Utils.Commands;

public abstract class OutputSettings : CommandSettings
{
    [CommandOption("--colors")]
    [Description("Enable colors in output.")]
    [DefaultValue(false)]
    public bool EnableColors { get; init; }

    [CommandOption("--no-indent")]
    [Description("No indent json output.")]
    [DefaultValue(false)]
    public bool NoIndent { get; init; }

    [CommandOption("--quoted")]
    [Description("Quote csv fields")]
    [DefaultValue(false)]
    public bool QuoteFields { get; init; }

    [CommandOption("--no-headers")]
    [Description("Omit csv headers")]
    [DefaultValue(false)]
    public bool OmitHeaders { get; init; }

    [CommandOption("-f|--output-format")]
    [Description("Output format. Default is text.")]
    [DefaultValue(OutputFormat.Text)]
    public OutputFormat OutputFormat { get; init; }
}

public enum OutputFormat
{
    Text,
    Json,
    Yaml,
    Table,
    Csv,
    Xml,
}

public static class OutputFormatExtensions
{
    public static void Print<T>(this OutputSettings settings, T source)
    {
        switch (settings.OutputFormat)
        {
            case OutputFormat.Text:
                settings.PrintText(source);
                break;
            case OutputFormat.Json:
                settings.PrintJson(source);
                break;
            case OutputFormat.Yaml:
                settings.PrintYaml(source);
                break;
            case OutputFormat.Table:
                settings.PrintTable(source);
                break;
            case OutputFormat.Csv:
                settings.PrintCsv(source);
                break;
            case OutputFormat.Xml:
                settings.PrintXml(source);
                break;
        }
    }

    public static void PrintText<T>(this OutputSettings settings, T source)
    {
        var text = source?.ToString() ?? string.Empty;
        AnsiConsole.Write(text);
    }

    public static void PrintJson<T>(this OutputSettings settings, T source)
    {
        var options = new JsonSerializerOptions();
        options.WriteIndented = !settings.NoIndent;

        var json = JsonSerializer.Serialize(source, options);

        if (!settings.EnableColors)
        {
            AnsiConsole.WriteLine(json);
        }
        else
        {
            AnsiConsole.Write(new JsonText(json));
        }
    }

    public static void PrintYaml<T>(this OutputSettings settings, T source)
    {
        var yaml = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build()
            .Serialize(source);

        AnsiConsole.WriteLine(yaml);
    }

    public static void PrintXml<T>(this OutputSettings settings, T source)
    {
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings()
        {
            Indent = !settings.NoIndent,
        });

        new XmlSerializer(typeof(T)).Serialize(xmlWriter, source);

        var xml = stringWriter.ToString();

        AnsiConsole.WriteLine(xml);
    }

    public static void PrintTable<T>(this OutputSettings settings, T source)
    {
        if (source is ITableOutput output)
        {
            var table = new Table().SimpleBorder();

            table.AddColumns(output.GetHeaders().ToArray());

            foreach (var row in output.GetRows())
            {
                table.AddRow(row.ToArray());
            }

            AnsiConsole.Write(table);
        }
        else
        {
            settings.PrintText(source);
        }
    }

    public static void PrintCsv<T>(this OutputSettings settings, T source)
    {
        if (source is ITableOutput output)
        {
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            if (!settings.OmitHeaders)
            {
                foreach (var header in output.GetHeaders())
                {
                    if (settings.QuoteFields)
                    {
                        csv.WriteField(header, true);
                    }
                    else
                    {
                        csv.WriteField(header);
                    }
                }
                csv.NextRecord();
            }

            foreach (var row in output.GetRows())
            {
                foreach (var field in row)
                {
                    if (settings.QuoteFields)
                    {
                        csv.WriteField(field, true);
                    }
                    else
                    {
                        csv.WriteField(field);
                    }
                }
                csv.NextRecord();
            }

            var text = writer.ToString();

            AnsiConsole.Write(text);
        }
        else
        {
            settings.PrintText(source);
        }
    }
}
