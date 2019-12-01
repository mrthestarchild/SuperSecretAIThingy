namespace QuestionAnswerAi.Models
{
    class ScoringModel
    {
        public string SubstringedSentence { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int WordDistance { get; set; }
        public double Boost { get; set; }
    }
}
