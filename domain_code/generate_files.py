import numpy as np

'''
Generate some kind of input for the substrate
'''

''' How big is our substrate output? '''
#HYPERNEAT_SUBSTRATE_INPUT_WIDTH  = 28
#HYPERNEAT_SUBSTRATE_INPUT_HEIGHT = 25
''' Generate an array of 0.5's for that size
    why 0.5? We need some stationary value.   '''
#HYPERNEAT_SUBSTRATE_INPUT = np.empty([HYPERNEAT_SUBSTRATE_INPUT_WIDTH,HYPERNEAT_SUBSTRATE_INPUT_HEIGHT])
#HYPERNEAT_SUBSTRATE_INPUT[:] = 0.5

''' Save inputs '''
#np.savetxt("../inputs.csv",HYPERNEAT_SUBSTRATE_INPUT, delimiter=";")



# ==============================================================================================================================

'''
Seed our RNG and set out our output directory
'''
WEIGHT_DIR       = '../shared/dbn_rng_weights/'
NUMPY_RNG = np.random.RandomState(123)


'''
Define our function to compute an array of the specified size
'''
def ComputeScaledUniformRandoms(neuron_inputs, neuron_outputs):
    low  = -np.sqrt(6. / (neuron_inputs + neuron_outputs))
    high = np.sqrt(6. / (neuron_inputs + neuron_outputs))
    return NUMPY_RNG.uniform(low = low, high = high, size=(neuron_inputs, neuron_outputs))

'''
Compute RNGs and save them in our shared DIR.
'''
np.savetxt(WEIGHT_DIR + "784_1000.csv", ComputeScaledUniformRandoms(784,1000) , delimiter=";")
np.savetxt(WEIGHT_DIR + "1000_784.csv", ComputeScaledUniformRandoms(1000,784) , delimiter=";")
np.savetxt(WEIGHT_DIR + "1000_1000.csv", ComputeScaledUniformRandoms(1000,1000) , delimiter=";")

# ==============================================================================================================================

OUTPUT_DIR = '../shared/hyperNEAT_outputs/'

'''
Create some kind of array to simulate an output from HyperNEAT
'''
output = NUMPY_RNG.poisson(5, 28 * 25)
output = np.reshape(output, (28,25))

np.savetxt(OUTPUT_DIR + "outputs.csv", output , delimiter=";")
