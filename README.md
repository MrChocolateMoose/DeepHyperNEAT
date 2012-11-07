DeepHyperNEAT
=============

Exploring the idea of using HyperNEAT to create the initial weight biases for the Deep Belief Network algorithm.

[TODO: License information. Obviously these two codebases we're integrating are not ours.]

PREREQUISITES TO RUN:
====================

- Mono
- Python >= 2.7 (?)

All of the necessary files have been generated so you can download
the code and immediately start using the code but you can optionally
run these initial scripts:

- OPTIONAL: Run 'generate_files.py' script in the 'domain_code'
  folder to generate RNG files that the 1st, 2nd, and 3rd layer
  of the DBN will load. This is OPTIONAL because I have already
  generated files and commited them but you can run it again.

- OPTIONAL: Run 'dataset_partioner.py' to reduce the MNIST dataset
  to a size which allows it to be run in several minutes for each
  HyperNEAT population member. A ~100 times reduced dataset has
  been include in the shared folder by default.


INSTRUCTIONS TO RUN:
====================

- Compile HyperNEAT in Mono (or Visual Studio) by opening the
  SharpNeat.sln file in the 'hyperNEATSharpNeatV2/src' folder

- Set the 'SharpNeatGUI' project as the default startup project
  and run the program.

- The 'Deep Belief Network Biaser' experiment should be loaded
  by default. Click (1) "Load Domain Default Parameters", 
  (2) "Create Random Population", and then (3) Start/Continue.

- Results are communication between the DBN and HyperNEAT are 
  stored in the 'shared' folder.
	- A visualization of each layer at every epoch, as well
	  as a visualization of the Substrate output that
	  the DBN is using can be found in the dbn_plots folder.

	- A dump of the fitness and alternative fitness can be
	  found in the 'hyperNEAT_fitnesses' folder.

	- A dump of the output of the substrate that HyperNEAT
	  generated can be found in hyperNEAT_outputs.

- Global Settings of how to configure the system can be found
  in 'DeepBeliefNetworkBaiserDefinitions.cs' which resides in
  the 'SharpNeatDomains/DeepBeliefNetworkBiaser' folder and in 
  'global_settings.py' / 'global_settings.xml' in the
  'domain_code' folder. Eventually there will be one configuration
  location.


INSTRUCTIONS TO RUN:
====================

- The DBN itself can be run without HyperNEAT. You do not need to 
  worry about reconfiguring relative paths. I handle this all by
  dynamically figuring out whether you are running from HyperNEAT
  or stand-a-lone and setting all of the variables accordingly.
  The DBN can be run by executing the DBN.py file.

Sorry this documentation is not very good but this project is more 
experimentation then anything actually usable. 





