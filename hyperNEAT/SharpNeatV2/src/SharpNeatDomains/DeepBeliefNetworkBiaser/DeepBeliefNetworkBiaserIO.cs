using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SharpNeat.Phenomes.NeuralNets;

namespace SharpNeat.Domains.DeepBeliefNetworkBiaser
{
	public static class DirectoryExtensions
	{
		public static void Empty(this System.IO.DirectoryInfo directory)
		{
		    foreach(System.IO.FileInfo file in directory.GetFiles()) file.Delete();
		    foreach(System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
		}
	}

    public static class DeepBeliefNetworkBiaserIO
    {
		/*
		/// <summary>
        /// Read in our fixed matrix which will be our inputs. We do this once.  We're more concerned about evolving a network.
        /// </summary>
        static List<double> ReadInputMatrix()
        {
            var inputMatrix = new List<double>();

            try
            {
				  CSVHelper csv = new CSVHelper(File.ReadAllText(Constants.INPUT_FILENAME));
				  foreach(string line in csv)
				  {	
	                double lineItem = double.Parse(line);
	                inputMatrix.Add(lineItem);
				  }
				  
                if (inputMatrix.Count != Constants.INPUT_AND_OUTPUT_SIZE) throw new Exception(String.Format("There were not {0} elements in the input file", Constants.INPUT_AND_OUTPUT_SIZE));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debugger.Break();
				//Environment.Exit(0);
            }

            return inputMatrix;
        }
		*/

        /// <summary>
        /// We evolved something in HyperNEAT. We need to send it over to python to visualize it and use it in the DBN. We pass it by
        /// csv file
        /// </summary>
        public static void WriteOutputMatrix(FastConnection[] connectionList, int width, int height)
        {
			StringBuilder csvBuilder = new StringBuilder();
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int matrixIndex = x * height + y;
                    
					csvBuilder.Append(connectionList[matrixIndex]._weight);
					
					if (matrixIndex == connectionList.Length - 1)
					{
					}
                    // Else write a semi-colon to delimit the number
                    else if (y != height - 1)
                    {
                        csvBuilder.Append(';');
                    }
				}
				
				if (x * height + (height - 1) != connectionList.Length - 1)
				{
					csvBuilder.Append('\n');
				}
			}
			string csv = csvBuilder.ToString();
			
			
			/*
            int threadCount = 1;//Environment.ProcessorCount; 
            List<StringBuilder> subMatrixAsString = new List<StringBuilder>();
            for (int i = 0; i < threadCount; i++)
            {
                subMatrixAsString.Add(new StringBuilder());
            }
            int subMatrixLength = outputMatrix.Length / threadCount;


            /// <summary>
            /// Map Step 
            /// 
            /// Divide up translating 
            ///</summary>
            Parallel.For(0, threadCount, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },
                thread =>
                {
                    for (int i = 0; i < subMatrixLength; i++)
                    {
                        int subMatrixIndex = (thread * subMatrixLength) + i;

                        subMatrixAsString[thread].Append(outputMatrix[subMatrixIndex]);

                        // If you're at the end of the file don't write anything
                        if (subMatrixIndex == outputMatrix.Length - 1)
                        {
                        }
                        // Else write a semi-colon to delimit the number
                        else
                        {
                            subMatrixAsString[thread].Append(";");
                        }
                    }
                });
            /// <summary>
            /// Reduce Step
            /// 
            /// Combine all of your Stringbuilders together to obtain the comma seperated value string
            /// </summary>
            string csv = subMatrixAsString.Select(stringBuilder => stringBuilder.ToString())
                                          .Aggregate(new StringBuilder(), (ag, n) => ag.Append(n)).ToString();

			 */
            // Write this to a file
			string outputFile = Constants.GET_OUTPUT_FILENAME(Thread.CurrentThread.ManagedThreadId);
            System.IO.File.WriteAllText(outputFile, csv);



        }

        public static void RunPythonScriptMultithreaded(string cmd = "python")
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = cmd;
            start.Arguments = Constants.PYTHON_DBN_FILENAME + " " + Thread.CurrentThread.ManagedThreadId;
            start.UseShellExecute = false;
            start.CreateNoWindow = true;

            // If the script crashes we can find out why by uncommenting these
            start.RedirectStandardInput = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            using (Process process = Process.Start(start))
            {
			    // If the script crashes we can find out why by uncommenting these
                    using (StreamReader reader = process.StandardError)
                    {
                        string result = reader.ReadToEnd();
                        Console.Write(result);
                    }
                process.WaitForExit();
            }
        }

        public static Tuple<double, double> ReadFitness()
        {
            using (TextReader reader = File.OpenText(Constants.GET_FITNESS_FILENAME(Thread.CurrentThread.ManagedThreadId)))
            {
				double fitness = double.Parse(reader.ReadLine());
				double altFitness = double.Parse(reader.ReadLine());
				
				return new Tuple<double,double>(fitness,altFitness);
            }
        }
    }
	
	public class CSVHelper : List<string>
	{
	  protected string csv = string.Empty;
	  public CSVHelper(string csv)
	  {
	    this.csv = csv;
			
		var lines = csv.Split( new string[]{ System.Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).ToList().Where(s => !string.IsNullOrEmpty(s));
	    foreach (string line in lines)
	    {
	      string[] values = line.Split(new char[] { ';' });
				
	      for (int i = 0; i < values.Length; i++)
	      {
	      	this.Add(values[i].ToString().Trim(new char[] { ' ' }));
	      }
	    }
	  }
	}
}
