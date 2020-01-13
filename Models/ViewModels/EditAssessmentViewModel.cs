using System.Collections.Generic;

namespace Apoa.Models.ViewModels
{
    public class EditAssessmentViewModel
    {

        public Assessment Assessment { get; set; }
        public virtual ICollection<Category> Categories { get; set; }

        public List<ResponseOptions> responseOptions { get; set; }

    }

}