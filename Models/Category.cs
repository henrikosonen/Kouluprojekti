using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Apoa.Models
{
    public class Category
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Assessment> Assessments { get; set; }


        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<CategoryGroup> CategoryGroups { get; set; }

        public DateTime? Deleted { get; set; }

    }
}