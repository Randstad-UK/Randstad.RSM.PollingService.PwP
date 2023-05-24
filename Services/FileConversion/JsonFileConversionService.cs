using ExcelDataReader;
using Randstad.RSM.PollingService.PwP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.FileConversion
{
    public class JsonFileConversionService : IFileConversionService<string>
    {
        public Task<string> ConvertAsync(string filename, Guid correlationId, string requiredColumnNames = null)
        {
            #region param validation
            if (string.IsNullOrWhiteSpace(filename))
                throw new FileConversionServiceException(
                    $"{nameof(JsonFileConversionService)}.{nameof(ConvertAsync)} - missing parameter value for {nameof(filename)}");

            #endregion param validation

            var fileExtension = Path.GetExtension(filename).ToLower();

            if (!fileExtension.Equals(".csv") && !fileExtension.Equals(".xlsx"))
                throw new FileConversionServiceException(
                    $"{nameof(JsonFileConversionService)}.{nameof(ConvertAsync)} - not supported file extension {fileExtension}");

            IExcelDataReader reader;
            FileStream stream;
            if (fileExtension.Equals(".csv"))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                stream = File.Open(filename, FileMode.Open, FileAccess.Read);

                reader = ExcelReaderFactory.CreateCsvReader(stream,
                    new ExcelReaderConfiguration { FallbackEncoding = Encoding.GetEncoding(1252) });
            }
            else
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                stream = File.Open(filename, FileMode.Open, FileAccess.Read);

                reader = ExcelReaderFactory.CreateReader(stream,
                    new ExcelReaderConfiguration { FallbackEncoding = Encoding.GetEncoding(1252) });
            }


            try
            {
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });

                var dataTable = dataSet.Tables[0];

                if (dataTable.Rows.Count <= 0)
                {
                    var message = $"File - {filename} does not include any data.";
                    throw new FileConversionServiceException(
                        $"{nameof(JsonFileConversionService)}.{nameof(ConvertAsync)} - {message}");
                }

                /* column headers is a dictionary where:
                    - key is column name from the file
                    - value is column name from the file with spaces replaced with '' and enclosed in double quotes
                    */
                var columnHeaders = new Dictionary<string, string>();
                foreach (var column in dataTable.Columns)
                {
                    columnHeaders.Add($"{column}", $"\"{column.ToString()?.Replace(" ", "")}\"");
                }

                var headers = columnHeaders.Keys.Aggregate((x, y) => x + "," + y);
                var missingColumns = AnyMissingColumn(headers, requiredColumnNames).ToList();
                if (missingColumns.Any())
                {
                    var message =
                        $@"The file '{filename}' you are attempting to upload does not contain the necessary columns to be able to complete the import. 
                        The column(s) missing are ({missingColumns.Aggregate((x, y) => x + ", " + y)}). 
                        You will need to correct this before the file can be imported.";
                    throw new FileConversionServiceException(
                        $"{nameof(JsonFileConversionService)}.{nameof(ConvertAsync)} - {message}");
                }

                var dataBuilder = new StringBuilder();

                dataBuilder.Append("{\"records\":["); // start of json array


                for (var index = 0; index < dataTable.Rows.Count; index++)
                {
                    var row = dataTable.Rows[index];

                    dataBuilder.Append('{'); // start of json object

                    var counter = 1;
                    foreach (var kVp in columnHeaders)
                    {
                        var value = $"{row[kVp.Key]}";
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            value = "null";
                        }
                        else
                        {
                            //var dataType = row[kVp.Key].GetType();
                            //if (dataType == typeof(string)) value = $"\"{value}\"";
                            value = $"\"{value}\"";
                        }

                        if (counter == columnHeaders.Count) // last column
                        {
                            dataBuilder.Append($"{kVp.Value}:{value}");
                        }
                        else
                        {
                            dataBuilder.Append($"{kVp.Value}:{value}, ");
                        }

                        counter++;
                    }

                    // mark end of json object
                    dataBuilder.Append(index == dataTable.Rows.Count - 1 ? "}" : "},");
                }

                dataBuilder.Append("]}"); // end of json array

                return Task.FromResult(dataBuilder.ToString());
            }
            catch (Exception e)
            {
                throw new FileConversionServiceException(
                    $"{nameof(JsonFileConversionService)}.{nameof(ConvertAsync)} - {e.Message}", e);
            }
            finally
            {
                reader?.Dispose();
                stream.Dispose();
            }
        }

        private IEnumerable<string> AnyMissingColumn(string source, string required)
        {
            var sourceArray = source.Split(',');
            var requiredArray = required.Split(',');
            var result = requiredArray.Except(sourceArray);
            return result;
        }

    }
}
