using System.Text;
using ExcelDataReader;
using Microsoft.KernelMemory.DataFormats;
using Microsoft.KernelMemory.DataFormats.Office;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.Pipeline;

namespace AiCore.FileIngestion.Service.Common.DataFormats.Office
{

    /// <summary>
    /// Excel Data Reader Decoder functionality for using in Kernel Memory
    /// Excel Data Reader does not support formula calculation
    /// https://github.com/ExcelDataReader/ExcelDataReader
    /// </summary>
    public class ExcelDataReaderDecoder: IContentDecoder
    {
        private readonly MsExcelDecoderConfig _config;
        private readonly ILogger<ExcelDataReaderDecoder> _log;

        public ExcelDataReaderDecoder(
            MsExcelDecoderConfig? config = null,
            ILoggerFactory? loggerFactory = null)
        {
            _config = config ?? new MsExcelDecoderConfig();
            _log = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<ExcelDataReaderDecoder>();

        }

        public bool SupportsMimeType(string mimeType)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            return mimeType != null && mimeType.StartsWith(MimeTypes.MsExcelX, StringComparison.OrdinalIgnoreCase);
        }

        public Task<FileContent> DecodeAsync(string filename, CancellationToken cancellationToken = default)
        {
            using var stream = File.OpenRead(filename);
            return DecodeAsync(stream, cancellationToken);
        }

        public Task<FileContent> DecodeAsync(BinaryData data, CancellationToken cancellationToken = default)
        {
            using var stream = data.ToStream();
            return DecodeAsync(stream, cancellationToken);
        }

        public Task<FileContent> DecodeAsync(Stream data, CancellationToken cancellationToken = default)
        {
            _log.LogDebug("Extracting text from MS Excel file");
            var result = new FileContent(MimeTypes.PlainText);
            using var workbook = ExcelReaderFactory.CreateReader(data);
            var sb = new StringBuilder();
            var worksheetNumber = 0;
            do
            {
                worksheetNumber++;
                if (_config.WithWorksheetNumber)
                {
                    sb.AppendLine(_config.WorksheetNumberTemplate.Replace("{number}", $"{worksheetNumber}", StringComparison.OrdinalIgnoreCase));
                }
                while (workbook.Read()) // read new row
                {
                    var objValues = new object[workbook.FieldCount];
                    _ = workbook.GetValues(objValues);
                    if (!objValues.Any(e => e != null)) continue;
                    sb.Append(_config.RowPrefix);
                    for (var i = 0; i < workbook.FieldCount; i++)
                    {
                        var cellValue = workbook.GetValue(i);

                        if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                        {
                            if (_config.WithQuotes)
                            {
                                sb.Append('"');
                                sb.Append(_config.BlankCellValue);
                                sb.Append('"');
                            }
                            else
                            {
                                sb.Append(_config.BlankCellValue);
                            }
                            if (i < workbook.FieldCount - 1) sb.Append(_config.ColumnSeparator);
                            continue;
                        }
                        var error = workbook.GetCellError(i);
                        if (error != null && error.HasValue)
                        {
                            if (_config.WithQuotes)
                            {
                                sb.Append('"');
                                sb.Append(error.Value.ToString().Replace("\"", "\"\"", StringComparison.Ordinal));
                                sb.Append('"');
                            }
                            else
                            {
                                sb.Append(error.Value.ToString());
                            }
                            if (i < workbook.FieldCount - 1) sb.Append(_config.ColumnSeparator);
                            continue;
                        }
                        var fieldType = workbook.GetFieldType(i);
                        if (_config.WithQuotes)
                        {
                            sb.Append('"');
                            if (fieldType == null) // yes this can be :)
                            {
                                sb.Append(cellValue.ToString());
                            }
                            else
                            {
                                if (fieldType == typeof(DateTime))
                                {
                                    sb.Append(workbook.GetDateTime(i).ToString(_config.DateFormat, _config.DateFormatProvider));
                                }
                                else if (fieldType == typeof(bool))
                                {
                                    sb.Append(workbook.GetBoolean(i) ? _config.BooleanTrueValue : _config.BooleanFalseValue);
                                }
                                else if (fieldType == typeof(string))
                                {
                                    var value = workbook.GetString(i).Replace("\"", "\"\"", StringComparison.Ordinal);
                                    sb.Append(string.IsNullOrEmpty(value) ? _config.BlankCellValue : value);
                                }
                                else
                                {
                                    sb.Append(cellValue.ToString());
                                }
                            }
                            sb.Append('"');
                        }
                        else
                        {
                            if (fieldType == null)
                            {
                                sb.Append(cellValue.ToString());
                            }
                            else
                            {
                                if (fieldType == typeof(DateTime))
                                {
                                    sb.Append(workbook.GetDateTime(i).ToString(_config.DateFormat, _config.DateFormatProvider));
                                }
                                else if (fieldType == typeof(bool))
                                {
                                    sb.Append(workbook.GetBoolean(i) ? _config.BooleanTrueValue : _config.BooleanFalseValue);
                                }
                                else
                                {
                                    sb.Append(cellValue.ToString());
                                }
                            }
                        }
                        if (i < workbook.FieldCount - 1) sb.Append(_config.ColumnSeparator);
                    }
                    sb.AppendLine(_config.RowSuffix);
                }
                if (_config.WithEndOfWorksheetMarker)
                {
                    sb.AppendLine(_config.EndOfWorksheetMarkerTemplate.Replace("{number}", $"{worksheetNumber}", StringComparison.OrdinalIgnoreCase));
                }
                var cnt = sb.ToString().Trim();
                sb.Clear();
                result.Sections.Add(new FileSection(worksheetNumber, cnt, true));
            } while (workbook.NextResult());// read new sheet
            return Task.FromResult(result);
        }
    }
}
