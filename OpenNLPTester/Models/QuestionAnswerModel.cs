using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionAnswerAi.Models
{
    public class QuestionAnswerModel : BaseResponseModel
    {
        public QuestionAnswerModel()
        {
            NeedsHumanTraining = false;
            Answer = "";
            WholeAnswer = "";
            FoundAnswerSentence = "";
            Score = 0.0;
            TrainingModel = new ExternalHumanTrainingModel();
        }

        public bool NeedsHumanTraining { get; set; }
        public string Answer { get; set; }
        public string WholeAnswer { get; set; }
        public string FoundAnswerSentence { get; set; }
        public double Score { get; set; }
        public ExternalHumanTrainingModel TrainingModel { get; set; }
    }
}
