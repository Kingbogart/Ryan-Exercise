using Confluent.Kafka;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace JsonSerDes
{
    public class JsonSerializer<T> : IAsyncSerializer<T> where T : class
    {
        Task<byte[]> IAsyncSerializer<T>.SerializeAsync(T data, SerializationContext context)
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            return Task.FromResult(Encoding.ASCII.GetBytes(jsonString));
        }
    }

    public class JsonDeserializer<T> : IAsyncDeserializer<T> where T : class
    {
        public Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
        {
            string json = Encoding.ASCII.GetString(data.Span);
            
            return Task.FromResult(JsonConvert.DeserializeObject<T>(json));
        }
    }
}