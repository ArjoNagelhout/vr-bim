using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace revit_to_vr_common
{
    public class VRBIM_Vector3Converter : JsonConverter<VRBIM_Vector3>
    {
        public override VRBIM_Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            reader.Read();
            float x = reader.GetSingle();

            reader.Read();
            float y = reader.GetSingle();

            reader.Read();
            float z = reader.GetSingle();

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException();
            }

            return new VRBIM_Vector3 { x = x, y = y, z = z };
        }

        public override void Write(Utf8JsonWriter writer, VRBIM_Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.x);
            writer.WriteNumberValue(value.y);
            writer.WriteNumberValue(value.z);
            writer.WriteEndArray();
        }
    }

    public class VRBIM_Vector3ListConverter : JsonConverter<List<VRBIM_Vector3>>
    {
        public override List<VRBIM_Vector3> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            var list = new List<VRBIM_Vector3>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                float x = reader.GetSingle();

                reader.Read();
                float y = reader.GetSingle();

                reader.Read();
                float z = reader.GetSingle();

                list.Add(new VRBIM_Vector3 { x = x, y = y, z = z });
            }

            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<VRBIM_Vector3> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var vector in value)
            {
                writer.WriteNumberValue(vector.x);
                writer.WriteNumberValue(vector.y);
                writer.WriteNumberValue(vector.z);
            }
            writer.WriteEndArray();
        }
    }
}
