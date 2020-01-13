using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Apoa.Models
{
    public class Response
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public DateTime AnswerDate { get; set; }

        public int Week { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int AssessmentId { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public Assessment Assessment { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ApplicationUser User { get; set; }

        public string Body { get; set; }

    }
}