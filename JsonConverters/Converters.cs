using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Randstad.RSM.PollingService.PwP.JsonConverters
{
    public class TnaNullableDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset?>
    {
        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            _ = DateTimeOffset.TryParse(reader.GetString(), out var value);
            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public class TnaDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            _ = DateTimeOffset.TryParse(reader.GetString(), out var value);
            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public class TnaNullableDecimalJsonConverter : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            _ = decimal.TryParse(reader.GetString(), out var value);
            return value;
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
