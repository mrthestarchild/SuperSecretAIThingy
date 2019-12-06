using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionAnswerAi.Models
{
    public class ExternalHumanTrainingResponseModel : BaseResponseModel
    {
        public string OriginalQuestion { get; set; }
    }
}
