'''
CAP 6616 Midterm Deliverable.

==============================
global_settings_reader.py
==============================
This file reads our global_settings.xml file which both C# and Python will be using to determine how they should run. We are interfacing
two completely different pieces of software together so it helps to keep everything central.
'''

import xml.etree.ElementTree as ET
import csv
import StringIO

global BIASING_STRATEGY_SETTING
global BIAS_INITIAL_MATRIX_TYPE
global AUGMENT_UNSUPERVISED_OUTPUT_MATRIX_TYPE
global IS_MULTI_THREADING_SETTING
global HIDDEN_LAYER_SIZE_SETTING
global DBN_PLOTS_DIR
global HYPERNEAT_FITNESS_DIR
global THREAD_ID

'''
We will be writing out files left and right and need to know where they go.
The relative locations will change depending on if we're running from hyperNEAT or
from command-line/IDE
'''
DBN_PLOTS_DIR         = None
HYPERNEAT_FITNESS_DIR = None
THREAD_ID             = None

'''
These are our enum values for our BIASING_STRATEGY
'''
BIAS_INITIAL_MATRIX_TYPE                = 0
AUGMENT_UNSUPERVISED_OUTPUT_MATRIX_TYPE = 1

'''
This is our 'constant' we will use when determining what biasing strategy we're using
'''
BIASING_STRATEGY_SETTING = BIAS_INITIAL_MATRIX_TYPE

'''
This is our 'constant' we will use when determining if we're running the experiment as part of
HyperNEAT and need to launch multiple simultaneous scripts to perform evaluation in parallel.
'''
IS_MULTI_THREADING_SETTING = False

'''
This is our 'constant' we will use when determining how many hidden layers we will be using and
their effective sizes.
'''
HIDDEN_LAYER_SIZE_SETTING = [1000, 1000, 1000]

'''
Read in all of our directory paths
'''
HYPERNEAT_RUNNING_DIR     = '../../../../../../shared/'
EXECUTION_RELATIVE_DIR              = "../../shared/"
RUNNING_DIR = HYPERNEAT_RUNNING_DIR

print "Name", __name__
print ""
if (__name__ == "__main__"):
    tree = ET.parse('aglobal_settings.xml')
else:
    try:
        tree = ET.parse(RUNNING_DIR + '../domain_code/global_settings.xml')
    except IOError:
        '''
        Dynamically figure out the right path. No more nasty configuration file changes. WOOO.
        '''
        if (RUNNING_DIR == EXECUTION_RELATIVE_DIR):
            RUNNING_DIR = HYPERNEAT_RUNNING_DIR
        else:
            RUNNING_DIR = EXECUTION_RELATIVE_DIR
        tree = ET.parse(RUNNING_DIR + '../domain_code/global_settings.xml')


DBN_PLOTS_DIR             = RUNNING_DIR + 'dbn_plots/'
HYPERNEAT_FITNESS_DIR     = RUNNING_DIR + 'hyperNEAT_fitnesses/' #fitness' + str(THREAD_ID) + '.csv'
REDUCED_DATASET_DIR       = RUNNING_DIR + 'datasets/'
HYPERNEAT_OUTPUT_DIR      = RUNNING_DIR + 'hyperNEAT_outputs/' #outputs" + str(THREAD_ID) + ".csv"
DBN_RNG_WEIGHTS_DIR       = RUNNING_DIR + 'dbn_rng_weights/'

'''
Read in all our our settings
'''
root = tree.getroot()

for setting in root.findall('Setting'):
    if (setting.attrib["Name"] == "biasing_strategy"): BIASING_STRATEGY_SETTING = setting.text
    if (setting.attrib["Name"] == "is_multi_threading"):
        if int(setting.text) == 1:
            IS_MULTI_THREADING_SETTING = True
        else:
            IS_MULTI_THREADING_SETTING = False
    if (setting.attrib["Name"] == "hidden_layer_size"):
        HIDDEN_LAYER_SIZE_SETTING = []
        hidden_layer_size_string = setting.text
        for row in csv.reader(StringIO.StringIO(hidden_layer_size_string)):
            for col in row:
                HIDDEN_LAYER_SIZE_SETTING.append(int(col))

'''
Verify them
'''
print "================"
print "READ IN SETTINGS"
print "================"
print "is_multi_threading: ", IS_MULTI_THREADING_SETTING
print "hidden_layer_size: ", HIDDEN_LAYER_SIZE_SETTING
print "biasing_strategy: ", BIASING_STRATEGY_SETTING