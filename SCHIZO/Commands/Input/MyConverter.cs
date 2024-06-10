using System;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace SCHIZO.Commands.Input;

// https://stackoverflow.com/a/6417753
internal class MyConverter : CustomCreationConverter<Dictionary<string, object>>
{
    public override Dictionary<string, object> Create(Type objectType)
        => [];

    public override bool CanConvert(Type objectType)
        => objectType == typeof(object) || base.CanConvert(objectType);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType is JsonToken.StartObject or JsonToken.Null)
        {
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        // if the next token is not an object
        // then fall back on standard deserializer (strings, numbers etc.)
        return serializer.Deserialize(reader);
    }
}
