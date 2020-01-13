using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Apoa.Models
{
    public class Assessment
    {
        [Key]
        public int Id { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public int CategoryId { get; set; }

        public virtual List<Response> Responses { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public Category Category { get; set; }

        public DateTime? Deleted { get; set; }

        public int? ResponseOptionsId { get; set; }

        public ResponseOptions ResponseOptions { get; set; }

    }
}