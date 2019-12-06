using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionAnswerAi.Models
{
    public class ExternalHumanTrainingModel
    {
        public ExternalHumanTrainingModel()
        {
            QuestionIdentifier = "";
            OriginalQuestion = "";
            Date = false;
            Location = false;
            Money = false;
            Organization = false;
            Percentage = false;
            Person = false;
            Time = false;
        }

        public string QuestionIdentifier { get; set; }
        public string OriginalQuestion { get; set; }
        public bool Date { get; set; }
        public bool Location { get; set; }
        public bool Money { get; set; }
        public bool Organization { get; set; }
        public bool Percentage { get; set; }
        public bool Person { get; set; }
        public bool Time { get; set; }
    }
}
