using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UrlShortener.Data
{
    public class TheUrl
    {
        [BsonId]
        public ObjectId InternalId { get; set; }

        public string Id { get; set; }

        [Display(Name = "Original URL")]
        public string OriginalUrl { get; set; }

        [Display(Name = "Shortened URL")]
        public string ShortUrl { get; set; }

        [BsonDateTimeOptions]
        public DateTime CreatedDate { get; set; }

        [BsonDateTimeOptions]
        public DateTime LastAccessed { get; set; }

        public string IP { get; set; }

        public long Counter { get; set; }

        public TheUrl()
        {
            CreatedDate = DateTime.Now;
            Id = ObjectId.GenerateNewId().ToString();
        }

    }
}
