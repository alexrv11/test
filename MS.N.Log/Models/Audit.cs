using System;
using System.Dynamic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MS.N.Log.Models
{
    public class Audit
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("data")]
        public ExpandoObject Data { get; set; }
    }
}
