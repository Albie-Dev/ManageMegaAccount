using Newtonsoft.Json;
using System.IO.Compression;

namespace MMA.Domain
{
    public class ProcessFileHelper
    {
        public static async Task ProcessJsonFileInBatchesAsync<T>(
            string filePath,
            Func<List<T>, Task> processBatch,
            int batchSize = 100,
            bool isCompressed = false,
            JsonSerializerSettings? settings = null) where T : class
        {
            filePath = Path.Combine(DirectoryHelper.GetSolutionBasePath(), filePath);
            Stream stream = isCompressed
                ? new GZipStream(new FileStream(filePath, FileMode.Open, FileAccess.Read), CompressionMode.Decompress)
                : new FileStream(filePath, FileMode.Open, FileAccess.Read);

            using (stream)
            using (StreamReader reader = new StreamReader(stream))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = settings?.NullValueHandling ?? NullValueHandling.Ignore;
                if (settings != null && settings.ContractResolver != null)
                {
                    serializer.ContractResolver = settings.ContractResolver;
                }
                serializer.Formatting = settings?.Formatting ?? Formatting.None;

                List<T> batch = new List<T>();
                while (await jsonReader.ReadAsync())
                {
                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        try
                        {
                            T? item = serializer.Deserialize<T>(jsonReader);

                            if (item != null)
                            {
                                batch.Add(item);
                            }

                            if (batch.Count >= batchSize)
                            {
                                await processBatch(batch);
                                batch.Clear();
                            }
                        }
                        catch (JsonSerializationException ex)
                        {
                            throw new Exception($"Error deserializing object: {ex.Message}", ex);
                        }
                    }
                }
                if (batch.Count > 0)
                {
                    await processBatch(batch);
                }
            }
        }

        public static void ProcessJsonFileInBatches<T>(
            string filePath,
            Action<List<T>> processBatch,
            int batchSize = 100,
            bool isCompressed = false,
            JsonSerializerSettings? settings = null) where T : class
        {
            Stream stream = isCompressed
                ? new GZipStream(new FileStream(filePath, FileMode.Open, FileAccess.Read), CompressionMode.Decompress)
                : new FileStream(filePath, FileMode.Open, FileAccess.Read);

            using (stream)
            using (StreamReader reader = new StreamReader(stream))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = settings?.NullValueHandling ?? NullValueHandling.Ignore;
                if (settings != null && settings.ContractResolver != null)
                {
                    serializer.ContractResolver = settings.ContractResolver;
                }
                serializer.Formatting = settings?.Formatting ?? Formatting.None;

                List<T> batch = new List<T>();
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        try
                        {
                            T? item = serializer.Deserialize<T>(jsonReader);
                            if (item != null)
                            {
                                batch.Add(item);
                            }

                            if (batch.Count >= batchSize)
                            {
                                processBatch(batch);
                                batch.Clear();
                            }
                        }
                        catch (JsonSerializationException ex)
                        {
                            throw new Exception($"Error deserializing object: {ex.Message}", ex);
                        }
                    }
                }

                if (batch.Count > 0)
                {
                    processBatch(batch);
                }
            }
        }
    }
}