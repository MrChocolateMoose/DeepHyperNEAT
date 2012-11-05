using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using log4net;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.Decoders.HyperNeat;
using SharpNeat.DistanceMetrics;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.HyperNeat;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;

namespace SharpNeat.Domains.DeepBeliefNetworkBiaser
{
    public class DeepBeliefNetworkBiasExperiment : IGuiNeatExperiment
    {
        private static readonly ILog __log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        NeatEvolutionAlgorithmParameters _eaParams;
        NeatGenomeParameters _neatGenomeParams;
        string _name;
        int _populationSize;
        int _specieCount;
        NetworkActivationScheme _activationSchemeCppn;
        NetworkActivationScheme _activationScheme;
        string _complexityRegulationStr;
        int? _complexityThreshold;
        string _description;
        ParallelOptions _parallelOptions;
        bool _lengthCppnInput;
        int _visualFieldResolution;
        int _visualFieldPixelCount;

        public DeepBeliefNetworkBiasExperiment()
        {
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        // 2 * (x,y,z) CPPN substrate node position coordinates, plus one optional connection length input.
        public int InputCount
        {
            get { return _lengthCppnInput ? 7 : 6; } 
        }

        // CPPN weight output and bias weight output.
        public int OutputCount
        {
            get { return 2; }
        }

        public int DefaultPopulationSize
        {
            get { return _populationSize; }
        }

        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
        {
            get { return _eaParams; }
        }

        public NeatGenomeParameters NeatGenomeParameters
        {
            get { return _neatGenomeParams; }
        }

        public void Initialize(string name, XmlElement xmlConfig)
        {
            // Read these from Domain XML file

            _name = name;
            _populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
            _specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
            _activationSchemeCppn = ExperimentUtils.CreateActivationScheme(xmlConfig, "ActivationCppn");
            _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
            _complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
            _complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
            _description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");
            _parallelOptions = ExperimentUtils.ReadParallelOptions(xmlConfig);
			_parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount / 2;
            

            _visualFieldResolution = XmlUtils.GetValueAsInt(xmlConfig, "Resolution");
            _visualFieldPixelCount = _visualFieldResolution * _visualFieldResolution;
            _lengthCppnInput = XmlUtils.GetValueAsBool(xmlConfig, "LengthCppnInput");

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            
            // Set these manually, use a high mutator just to test the water
            
            _neatGenomeParams = new NeatGenomeParameters()
            {
                AddConnectionMutationProbability = 0.80,
                DeleteConnectionMutationProbability = 0.50,
                ConnectionWeightMutationProbability = 0.80,
                AddNodeMutationProbability = 0.70,
                InitialInterconnectionsProportion = 0.80
            };
			
			// Clear OUTPUT and FITNESS directories before starting
			var directory = new DirectoryInfo(Constants.OUTPUT_DIR);
			directory.Empty();
			directory = new DirectoryInfo(Constants.FITNESS_DIR);
			directory.Empty();
			directory = new DirectoryInfo(Constants.PLOTS_DIR);
			directory.Empty();
        }

        public List<NeatGenome> LoadPopulation(XmlReader xr)
        {
            NeatGenomeFactory genomeFactory = (NeatGenomeFactory)CreateGenomeFactory();
            return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
        }

        public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
        {
            NeatGenomeXmlIO.WriteComplete(xw, genomeList, true);
        }

        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            return CreateGenomeDecoder(_lengthCppnInput);
        }

        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new CppnGenomeFactory(InputCount, OutputCount, GetCppnActivationFunctionLibrary(), _neatGenomeParams);
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
        {
            return CreateEvolutionAlgorithm(_populationSize);
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
        {
            // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);

            // Create evolution algorithm.
            return CreateEvolutionAlgorithm(genomeFactory, genomeList);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a pre-built genome population and their associated/parent genome factory.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
        {
            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, _parallelOptions);

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

            // Create the evolution algorithm.
            NeatEvolutionAlgorithm<NeatGenome> ea = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy, complexityRegulationStrategy);

            // Create IBlackBox evaluator.
            DeepBeliefNetworkBiasEvaluator evaluator = new DeepBeliefNetworkBiasEvaluator();

            // Create genome decoder. Decodes to a neural network packaged with an activation scheme that defines a fixed number of activations per evaluation.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = CreateGenomeDecoder(_lengthCppnInput);

            // Create a genome list evaluator. This packages up the genome decoder with the genome evaluator.
            IGenomeListEvaluator<NeatGenome> innerEvaluator = null;
            if (Constants.IS_MULTI_THREADING)
            {
                innerEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator, _parallelOptions);
            }
            else
            {
                innerEvaluator = new SerialGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator);
            }
            
            // Wrap the list evaluator in a 'selective' evaulator that will only evaluate new genomes. That is, we skip re-evaluating any genomes
            // that were in the population in previous generations (elite genomes). This is determiend by examining each genome's evaluation info object.
            IGenomeListEvaluator<NeatGenome> selectiveEvaluator = new SelectiveGenomeListEvaluator<NeatGenome>(
                                                                                    innerEvaluator,
                                                                                    SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());
            // Initialize the evolution algorithm.
            ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);

            // Finished. Return the evolution algorithm
            return ea;
        }

        public AbstractGenomeView CreateGenomeView()
        {
            return new CppnGenomeView(GetCppnActivationFunctionLibrary());
        }

        // TODO: One day perhaps I'll make a GUI for this
        public AbstractDomainView CreateDomainView()
        {
            return null;
        }

        
        /// <summary>
        /// Gets the CPPN length input flag, as loaded from the experiment config XML.
        /// </summary>
        public bool LengthCppnInput
        {
            get { return _lengthCppnInput; }
        }

        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder(bool lengthCppnInput)
        {
            // Create two layer sandwhich substract substrate. Inputs on bottom of cube and Outputs are on the top.
            SubstrateNodeSet inputLayer = new SubstrateNodeSet(Constants.INPUT_AND_OUTPUT_SIZE);
            SubstrateNodeSet outputLayer = new SubstrateNodeSet(Constants.INPUT_AND_OUTPUT_SIZE);

            for (uint height = 0; height < Constants.INPUT_AND_OUTPUT_HEIGHT; height++)
            {
                for (uint width = 0; width < Constants.INPUT_AND_OUTPUT_WIDTH; width++)
                {
                    // start with 1 because of the bias node is 0
                    uint inputID = (height * Constants.INPUT_AND_OUTPUT_WIDTH) + width + 1;
                    // start with INPUT_AND_OUTPUT_SIZE + 1 because id's need to be unique
                    uint outputID = Constants.INPUT_AND_OUTPUT_SIZE + (height * Constants.INPUT_AND_OUTPUT_WIDTH) + width + 1;

                    // Get X and Y positions on the hypercube.
                    double posX = -1.0 + ((width * 1.0D / Constants.INPUT_AND_OUTPUT_WIDTH)  * 2);
                    double posY = -1.0 + ((height * 1.0D / Constants.INPUT_AND_OUTPUT_HEIGHT) * 2);

                    // Add nodes to layers
                    inputLayer.NodeList.Add(new SubstrateNode(inputID, new double[] { posX, posY, -1.0 }));
                    outputLayer.NodeList.Add(new SubstrateNode(outputID, new double[] { posX, posY, 1.0 }));
                }
            }

            List<SubstrateNodeSet> nodeSetList = new List<SubstrateNodeSet>(2);
            nodeSetList.Add(inputLayer);
            nodeSetList.Add(outputLayer);

            // Define connection mappings between layers/sets.
            List<NodeSetMapping> nodeSetMappingList = new List<NodeSetMapping>(1);
            nodeSetMappingList.Add(NodeSetMapping.Create(0, 1, (double?)null));

            // Construct substrate.
            Substrate substrate = new Substrate(nodeSetList, GetCppnActivationFunctionLibrary(), 0, Constants.THRESHOLD_WEIGHT, Constants.MAX_WEIGHT, nodeSetMappingList);

            // Create genome decoder. Decodes to a neural network packaged with an activation scheme that defines a fixed number of activations per evaluation.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = new HyperNeatDecoder(substrate, _activationSchemeCppn, _activationScheme, lengthCppnInput);
            return genomeDecoder;
        }

        public static IActivationFunctionLibrary CreateDeepBeliefNetworkBiaserFunctions()
        {
            List<ActivationFunctionInfo> fnList = new List<ActivationFunctionInfo>(4);
            
            fnList.Add(new ActivationFunctionInfo(0, 0.10, Gaussian.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(1, 0.70, Sine.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(2, 0.10, Linear.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(3, 0.10, BipolarSigmoid.__DefaultInstance));

            return new DefaultActivationFunctionLibrary(fnList);
        }

        IActivationFunctionLibrary GetCppnActivationFunctionLibrary()
        {
            return CreateDeepBeliefNetworkBiaserFunctions();
        }
    }
}
