using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester.Models
{
    class PossibleResultsModel
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string NerResult { get; set; }
        public string PossibleResult { get; set; }
    }
}
