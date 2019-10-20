using OpenNLP.Tools.NameFind;
using SharpEntropy;
using System.Text.RegularExpressions;

namespace QuestionAnswerAi
{
    class OpenNLPTrainers
    {
        private OpenNLPMethods methods;
        private string trainingPath;

        // use DI to get everything we need for training.
        public OpenNLPTrainers(OpenNLPMethods _methods, string _trainingPath)
        {
            methods = _methods;
            trainingPath = _trainingPath;
        }

        /// <summary>
        /// Train a NER model this can be used for multiple types to be trained but it is not recomended.
        /// testSentence must contain a testable type that you are passing in.
        /// trainingFile needs to be a .train file
        /// </summary>
        /// <param name="testSentence"></param>
        /// <param name="iterations"></param>
        /// <param name="cuts"></param>
        /// <param name="trainingFile"></param>
        /// <param name="listOfTypes"></param>
        /// <returns>best GisModel</returns>
        public GisModel TrainNER(string testSentence, int[] iterations, int[] cuts, string trainingFile, string[] listOfTypes)
        {
            string pathToTrainingFile = $@"{trainingPath}NER\{trainingFile}";           

            // init values
            var bestIterationValue = iterations[0];
            var bestCutValue = cuts[0];
            var bestAccuracy = 0;

            // get a base model to init the best model, if no change then we will need to try again again.
            GisModel bestModel = MaximumEntropyNameFinder.TrainModel(pathToTrainingFile, bestIterationValue, bestCutValue);

            // Train the model (can take some time depending on your training file size)
            foreach (int iteration in iterations)
            {
                foreach (int cut in cuts)
                {
                    // train model
                    GisModel model = MaximumEntropyNameFinder.TrainModel(trainingFile, iteration, cut);
                    // test model
                    string modelTestResult = methods.NER(testSentence);

                    // set number of finds for what we are matching to
                    var numOfFinds = 0;
                    // Check numOfFinds
                    foreach (string type in listOfTypes)
                    {
                        numOfFinds += Regex.Matches(modelTestResult, type).Count;
                    }

                    // check if we find anything, if we do we set the values to know what the best system was and set the new model to bestModel.
                    if (numOfFinds > bestAccuracy)
                    {
                        bestAccuracy = numOfFinds;
                        bestIterationValue = iteration;
                        bestCutValue = cut;
                        bestModel = model;
                    }
                }
            }

            // Write out our findings.
            if (bestAccuracy == 0)
            {
                System.Console.WriteLine("The training was unsuccesful please ensure your file is long enough(17,000 lines mininum) and formatted correctly.");
            }
            else
            {
                System.Console.WriteLine($"The best accuracy was {bestAccuracy}, the best iteration value was {bestIterationValue}, and the best cutoff value was {bestCutValue}.");
            }

            return bestModel;
        }

        //public GisModel AnswerTypeTrainer()
        //{
        //    // todo create AnswerTypeCLassifier, AnswerTypeContextGenerator, and AnswerTypeEventStream.
        //    var initialFile = new java.io.File("");
        //    return GisModel
        //}
    }
}
