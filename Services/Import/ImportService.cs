using Randstad.Logging;
using Randstad.RSM.PollingService.PwP.JsonConverters;
using Randstad.RSM.PollingService.PwP.Models;
using Randstad.RSM.PollingService.PwP.Services.Api;
using Randstad.RSM.PollingService.PwP.Services.FileConversion;
using Randstad.RSM.PollingService.PwP.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.Import
{
    public class ImportService : IImportService
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly ILogger _logger;
        private readonly IFileConversionService<string> _fileConversionService;
        private readonly IApiService _apiService;
        private readonly ClientApiSettings _clientApiSettings;

        public ImportService(ApplicationSettings applicationSettings, ILogger logger,
            IFileConversionService<string> fileConversionService,
            IApiService apiService, ClientApiSettings clientApiSettings)
        {
            _applicationSettings = applicationSettings;
            _logger = logger;
            _fileConversionService = fileConversionService;
            _apiService = apiService;
            _clientApiSettings = clientApiSettings;
        }

        public async Task<IEnumerable<Timesheet>> ProcessAsync(Guid correlationId)
        {
            var timesheetList = new List<Timesheet>();

            var filenameValidationResult = GetTnaFilename(correlationId);

            if (!filenameValidationResult.valid) return timesheetList;

            var tnaRecordCollection = await CreateTnaRecordCollectionFromTnaFileAsync(filenameValidationResult.filename, correlationId);

            timesheetList = await CreateTimesheetListFromTnaRecordCollectionAsync(tnaRecordCollection, correlationId);

            return timesheetList;
        }

        private async Task<TnaRecordCollection> CreateTnaRecordCollectionFromTnaFileAsync(string filename, Guid correlationId)
        {
            // convert the file into a json load
            var tnaRecordsJsonLoad = await _fileConversionService.ConvertAsync(filename, correlationId, null /*TODO comma separated mandatory column names*/);

            // deserialize json load
            var jsonOption = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new TnaNullableDateTimeOffsetJsonConverter(),
                    new TnaDateTimeOffsetJsonConverter(),
                    new TnaNullableDecimalJsonConverter()
                }
            };
            var tnaRecords = JsonSerializer.Deserialize<TnaRecordCollection>(tnaRecordsJsonLoad, jsonOption);

            // validations on deserialized json load
            if (tnaRecords == null)
            {
                var message =
                    $"{nameof(ImportService)}.{nameof(ProcessAsync)} - missing value for {nameof(tnaRecords)}";
                _logger.Warn(message, correlationId, null, null, null, null);
                throw new TnaImportException(message);
            }
            if (!tnaRecords.Records.Any())
            {
                var message =
                    $"{nameof(ImportService)}.{nameof(ProcessAsync)} - no records to process {nameof(TnaRecordCollection.Records)}";
                _logger.Warn(message, correlationId, null, null, null, null);
                throw new TnaImportException(message);
            }

            /*TODO other tna collection validation if any*/

            _logger.Info(
                $"{nameof(ImportService)}.{nameof(CreateTnaRecordCollectionFromTnaFileAsync)}: Tna converted data from file",
                correlationId, tnaRecords, null, null, null);

            return tnaRecords;
        }

        private async Task<List<Timesheet>> CreateTimesheetListFromTnaRecordCollectionAsync(TnaRecordCollection tnaRecordCollection, Guid correlationId)
        {
            var timesheetList = new List<Timesheet>();

            await Task.Delay(1000);  /*TODO place holder for the proper async method call*/

            /*TODO logic create timesheet*/

            _logger.Info(
                $"{nameof(ImportService)}.{nameof(CreateTnaRecordCollectionFromTnaFileAsync)}: Timesheets created from Tna records - ({timesheetList.Count})",
                correlationId, timesheetList, null, null, null);

            return timesheetList;
        }

        private (bool valid, string filename) GetTnaFilename(Guid correlationId)
        {
            if (!Directory.Exists(_applicationSettings.SftpDropZone))
            {
                var error = new DirectoryNotFoundException(
                    $"Sftp drop zone directory {_applicationSettings.SftpDropZone} does not exist.");
                _logger.Error(
                    $"Sftp drop zone directory {_applicationSettings.SftpDropZone} does not exist.", correlationId, null,
                    null, null, null, error);
                throw error;
            }

            var originalFullFilename = Directory.GetFiles(_applicationSettings.SftpDropZone).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(originalFullFilename)) return (valid: false, filename: null);

            if (!Directory.Exists(_applicationSettings.ArchiveDirectory))
                Directory.CreateDirectory(_applicationSettings.ArchiveDirectory);

            var filenameOnly = Path.GetFileNameWithoutExtension(originalFullFilename);
            var fileExtension = Path.GetExtension(originalFullFilename);

            var copyToFilename = $"{Path.Combine(_applicationSettings.ArchiveDirectory, filenameOnly)}-{DateTime.Now:ddMMyyyyHmms}{fileExtension}";
            File.Copy(originalFullFilename, copyToFilename, true);
            File.Delete(originalFullFilename);

            var matchingFilename = $"{filenameOnly}{fileExtension}";
            if (!Regex.IsMatch(matchingFilename, _applicationSettings.ValidFilePattern))
            {
                var message =
                    $"The file {matchingFilename} in folder {_applicationSettings.SftpDropZone} - do not conform to {_applicationSettings.ValidFilePattern} pattern.  The files have been archived and removed from the drop zone.";
                _logger.Warn(message, correlationId, null, null, null, null);
                throw new ArgumentException(message);
            }

            return (valid: true, filename: copyToFilename);
        }

    }
}
