DeepHyperNEAT
=============

Exploring the idea of using HyperNEAT to create the initial weight biases for the Deep Belief Network algorithm.

[TODO: License information. Obviously these two codebases we're integrating are not ours.]


INSTRUCTIONS TO RUN:
====================

- Run 'generate_files.py' script in the 'domain_code' folder to
  generate the inputs file that HyperNEAT will use and a RNG
  file that the first layer of the DBN will load. 

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
  location. Additionally, if you change the sizes of the input
  and output of the substrate that you will query you need to
  change the 'generate_files.py' script as well as perhaps the
  dbn itself.

- The DBN itself can be run without HyperNEAT vt
  changing the RUNNING_DIR variable in 
  global_settings.py/global_settings.xml to
  use the EXECUTION_RELATIVE_DIR instead of the
  HYPERNEAT_RELATIVE_DIR. 

  Oh and how to run the DBN? You run the DBN.py file.

Sorry this documentation is not very good but this project is more 
experimentation then anything actually usable. 





