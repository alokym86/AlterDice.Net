﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AlterDice.Net.Converters
{
   
    public class Decimal10e8Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            {
                var parsed = token.ToObject<decimal>();
                /// in active orders all numbers are not multiplicated to 100_000_000, so simply return parsed value
                if (parsed / 1e8m <= 0.00000001m)
                {
                    return parsed;
                }
                return parsed / 1e8m;
            }
            if (token.Type == JTokenType.String)
            {
                var parsed= Decimal.Parse(token.ToString(), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.Any, CultureInfo.InvariantCulture) ;
                /// in active orders all numbers are not multiplicated to 100_000_000, so simply return parsed value
                if (parsed / 1e8m <= 0.00000001m)
                {
                    return parsed;
                }
                return parsed / 1e8m;
            }
            if (token.Type == JTokenType.Null && objectType == typeof(decimal?))
            {
                return null;
            }
            if (token.Type == JTokenType.Null && objectType == typeof(decimal))
            {
                return 0m;
            }
            throw new JsonSerializationException("Unexpected token type: " + token.Type.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                var text = value.ToString().Replace(",", ".");
                writer.WriteValue(text);
            }

        }
    }
}
