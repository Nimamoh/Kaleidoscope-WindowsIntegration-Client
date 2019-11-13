using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using System.Threading.Tasks;
using kio_windows_integration.Models;
using log4net;

namespace kio_windows_integration.Services
{
    public class PersistenceService
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(PersistenceService));

        
        /// <summary>
        /// Persist the app layer mappings
        /// </summary>
        private const string MappingsFileName = "AppLayerMappings.json";

        /// <summary>
        /// Default options for the serializer
        /// </summary>
        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public async Task Save(ISet<ApplicationLayerMapping> mappings)
        {
            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForDomain();

            var json = JsonSerializer.Serialize(mappings, options);

            using (var stream =
                new IsolatedStorageFileStream(MappingsFileName, FileMode.Create, isolatedStorageFile))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(json);
            }
        }

        /// <summary>
        /// Loads the app layer mappings
        /// </summary>
        public async Task<ISet<ApplicationLayerMapping>> load()
        {
            try
            {
                IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForDomain();

                var json = "";
                using (var stream = new IsolatedStorageFileStream(MappingsFileName, FileMode.Open, isolatedStorageFile))
                using (StreamReader reader = new StreamReader(stream))
                {
                    json = await reader.ReadToEndAsync();
                }

                var mappings = JsonSerializer.Deserialize<ISet<ApplicationLayerMapping>>(json, options);
                return mappings;
            }
            catch (Exception e)
            {
                Log.Error("Failed to read mapping file", e);
                return new HashSet<ApplicationLayerMapping>();
            }
        }
    }
}