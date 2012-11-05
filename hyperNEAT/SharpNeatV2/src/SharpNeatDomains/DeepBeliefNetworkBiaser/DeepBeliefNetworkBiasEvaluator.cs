using System;
using SharpNeat.Core;
using SharpNeat.Phenomes;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace SharpNeat.Domains.DeepBeliefNetworkBiaser
{
    public class DeepBeliefNetworkBiasEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        static DeepBeliefNetworkBiasEvaluator()
        {
        }

        // Happens when there are no misclassications
        const double MaxFitness = 10000; 

        public DeepBeliefNetworkBiasEvaluator()
        {
        }

        
        public ulong EvaluationCount
        {
            get;
            private set;
        }

        public bool StopConditionSatisfied
        {
            get;
            private set;
        }

        public FitnessInfo Evaluate(IBlackBox box)
        {
           EvaluationCount++;
            
            // show the network the fixed input to sample a 2D portion of the 4D space. 
            for (int i = 0; i < Constants.INPUT_AND_OUTPUT_SIZE; i++)
            {
                box.InputSignalArray[i] = DeepBeliefNetworkBiaserIO.InputMatrix[i];
            }
            
            // activate network which outputs the new random weight layer
            box.ResetState();
            box.Activate();
            if (!box.IsStateValid)
            {
                return new FitnessInfo(0,0);
            }

            // Retrieve outputs of the network. 
            double[] outputMatrix = new double[Constants.INPUT_AND_OUTPUT_SIZE];
            box.OutputSignalArray.CopyTo(outputMatrix, 0);
            // Write them out to a file. 
            DeepBeliefNetworkBiaserIO.WriteOutputMatrix(outputMatrix, 
			                                            Constants.INPUT_AND_OUTPUT_WIDTH,
			                                            Constants.INPUT_AND_OUTPUT_HEIGHT);
            
            // Run the python DBN script which will output 
            DeepBeliefNetworkBiaserIO.RunPythonScriptMultithreaded();

            Tuple<double, double> fitnessPair = DeepBeliefNetworkBiaserIO.ReadFitness();
            
            // Set stop flag when max fitness is attained.
            if (!StopConditionSatisfied && fitnessPair.Item1 == MaxFitness)
            {
                StopConditionSatisfied  = true;
            }

            return new FitnessInfo(fitnessPair.Item1, fitnessPair.Item2);
        }

        public void Reset()
        {
        }

    }
}
