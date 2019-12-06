using System.Collections.Generic;

namespace QuestionAnswerAi.Models
{
    public class QuestionParserResponseModel
    {
        public string Question { get; set; }
        public List<string> QueryParams { get; set; }
        public string[] QuestionTypes { get; set; }
        public string QuestionIdentifier { get; set; }
        public bool HasTimeRef { get; set; }
    }
}
