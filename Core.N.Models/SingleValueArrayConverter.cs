﻿using System;
using Newtonsoft.Json;

namespace BGBA.Models.N.Core
{
    public class SingleValueArrayConverter<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = new Object();
            if (reader.TokenType == JsonToken.StartObject)
            {
                T instance = (T)serializer.Deserialize(reader, typeof(T));
                retVal = new T[]{ instance };
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                retVal = serializer.Deserialize(reader, objectType);
            }
            else
            {
                retVal = new T[] { (T)reader.Value };
            }
            return retVal;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override bool CanWrite => false;
    }
}
