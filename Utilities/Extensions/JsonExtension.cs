using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Utilities.Extensions
{
    public class JsonExtension
    {
        /// <summary>
        /// 設定Json預設DateTime格式化
        /// https://blog.csdn.net/kukubashen/article/details/123798040
        /// </summary>
        public class DateTimeJsonConverter(string format) : JsonConverter<DateTime>
        {
            private readonly string Format = format;

            // Deserialization
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string dateString = reader.GetString() ?? "0001-01-01T00:00:00"; // 預設為最小日期
                return DateTime.ParseExact(dateString, Format, null);
            }

            // Serialization
            public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
            {
                writer.WriteStringValue(date.ToString(Format));
            }
        }

    }
}
