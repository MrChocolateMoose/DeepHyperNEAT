using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.DeepBeliefNetworkBiaser
{	
    public class Constants
    {
        public const bool IS_MULTI_THREADING = true;
		
		// 783
        public const int INPUT_WIDTH = 29,
                         INPUT_HEIGHT = 27,
                         INPUT_SIZE = INPUT_WIDTH * INPUT_HEIGHT;
		
		// 1000
        public const int OUTPUT_WIDTH = 50,
                         OUTPUT_HEIGHT = 20,
                         OUTPUT_SIZE = OUTPUT_WIDTH * OUTPUT_HEIGHT;
		
		// (783 + 1 bias) * 1000 = 784,000 fully connnected
		public const int FULLY_CONNECTED_SIZE = INPUT_SIZE * OUTPUT_SIZE;
		
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
