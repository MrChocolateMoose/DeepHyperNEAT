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
using SharpNeat.Phenomes.NeuralNets;

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
            
			FastCyclicNetwork network = box as FastCyclicNetwork;
			
			// Write them out to a file. 
            DeepBeliefNetworkBiaserIO.WriteOutputMatrix(network._connectionArray, 
			                                            Constants.INPUT_SIZE + 1,
			                                            Constants.OUTPUT_SIZE);
            
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
