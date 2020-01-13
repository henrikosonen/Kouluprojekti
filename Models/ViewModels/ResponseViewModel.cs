using Apoa.Models;
using System.Collections.Generic;
using System.Linq;

namespace Apoa.Models.ViewModels
{

    public class ResponseViewModel
    {

        public List<Category> categories { get; set; }
        public Dictionary<int, string> responseOptions { get; set; }

        public List<Response> responses { get; set; }

        public string UserId { get; set; }

        public int index { get; set; }

        public ResponseViewModel()
        {
            this.categories = new List<Category>();
            this.responseOptions = new Dictionary<int, string>() {
                {1, "Huonoin"},
                {2, "Huono"},
                {3, "Ok"},
                {4, "Hyvä"},
                {5, "Paras"}
            };
        }


    }

}