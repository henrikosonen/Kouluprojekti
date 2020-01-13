using System;
using System.Collections.Generic;

namespace Apoa.Models
{
    public class CategoryGroup
    {

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }


    }
}
