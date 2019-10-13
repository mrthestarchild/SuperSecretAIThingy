using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester.Models
{
    class QuestionParserResponseModel
    {
        public string Question { get; set; }
        public List<string> QueryParams { get; set; }
        public string[] QuestionTypes { get; set; }
        public string QuestionIdentifier { get; set; }
        public bool HasTimeRef { get; set; }
    }
}
