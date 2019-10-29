﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionAnswerAi.Models
{
    class PossibleResultsModel
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string FoundNerResult { get; set; }
        public string PossibleResultSentence { get; set; }
        public string NerResultSentence { get; set; }
        public double Score { get; set; } = 0;
    }
}
