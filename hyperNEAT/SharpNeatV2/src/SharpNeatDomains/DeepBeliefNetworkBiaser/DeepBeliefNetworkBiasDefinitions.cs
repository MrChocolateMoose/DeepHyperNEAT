using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.DeepBeliefNetworkBiaser
{	
    public class Constants
    {
        public const bool IS_MULTI_THREADING = true;

        public const int INPUT_AND_OUTPUT_WIDTH = 28,
                         INPUT_AND_OUTPUT_HEIGHT = 25,
                         INPUT_AND_OUTPUT_SIZE = INPUT_AND_OUTPUT_WIDTH * INPUT_AND_OUTPUT_HEIGHT;

        public const double MAX_WEIGHT = 1;
        public const double THRESHOLD_WEIGHT = 0.1D;
        
        private const string RELATIVE_DIR    = "../../../../../../";
        public  const string INPUT_FILENAME  = RELATIVE_DIR + "shared/initials/inputs.csv";
        
		public const string PLOTS_DIR = RELATIVE_DIR + "shared/dbn_plots/";
        
		public const string OUTPUT_DIR = RELATIVE_DIR + "shared/hyperNEAT_outputs/";
        public static string GET_OUTPUT_FILENAME(int id = 0)
        {
            return OUTPUT_DIR + "outputs" + id + ".csv";
        }
		
		public const string FITNESS_DIR = RELATIVE_DIR + "shared/hyperNEAT_fitnesses/";
        public static string GET_FITNESS_FILENAME(int id = 0)
        {
            return FITNESS_DIR + "fitness" + id + ".csv";
        }

        public const string PYTHON_DBN_FILENAME = RELATIVE_DIR + "deep_learning/code/DBN.py";
    }
}
