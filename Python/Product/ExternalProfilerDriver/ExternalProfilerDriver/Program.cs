using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalProfilerDriver
{
    class Program
    {
        static int Main(string[] args)
        {

#if false
            CommandLineApplication clapp = new CommandLineApplication(throwOnUnexpectedArg: false);
	    // CommandArgument names = null;

       CommandOption vtpathRequested = clapp.Option("-v|--vtunepath", "Displays the path where VTune is installed, if any", CommandOptionType.NoValue);

        clapp.HelpOption("-?|-h|--help");
	    clapp.OnExecute(() => {

            if (vtpathRequested.HasValue())
            {
                string ev = GetVTunePath();
                Console.WriteLine(string.Format("The value of the envvar is [ {0} ]", ev));
            }
            else
            {
                Console.WriteLine("Need a command line to profile");
            }

	      return 0;
	    });
	    
	    int ret = clapp.Execute(args);
#else
            int ret = 0;
#endif


#if false
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = @"c:\program files\dotnet\dotnet.exe",
                Arguments = "--info",
                CreateNoWindow = false,
            };

            Process p = new Process()
            {
                StartInfo = psi,
                EnableRaisingEvents = true,
            };
            p.Exited += (object sender, EventArgs a) =>
            {
                Console.WriteLine("Just exited the process!!!!!");
            };
            p.Start();
            Console.WriteLine(string.Format("Hoping to have found the executable: [ {0} ]", typeof(content.Program).Assembly.Location));

            p.WaitForExit();
#endif
#if false
            VTuneInvoker inv = new VTuneInvoker();
            Console.WriteLine(string.Format("The answer from the invoker is [{0}]", inv.Report()));
#endif
            string vtuneExec = VTuneInvoker.VTunePath();
            Console.WriteLine(string.Format("Trying out the command line: [{0}]", vtuneExec));

#if false
            VTuneCPUUtilizationSpec cpuUtilSpec = new VTuneCPUUtilizationSpec();
            Console.WriteLine($"Trying out the command line: [{cpuUtilSpec.FullCLI()}]");
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = vtuneExec,
                Arguments = "--version",
                CreateNoWindow = false,
            };

            Process p = new Process()
            {
                StartInfo = psi,
                EnableRaisingEvents = true,
            };
            p.Exited += (object sender, EventArgs a) =>
            {
                Console.WriteLine("Just exited the process!!!!!");
            };
            p.Start();
            p.WaitForExit();
#endif
#if false
            VTuneInvoker invoker = new VTuneInvoker("testinvoker");
            VTuneCollectSpec collector = new VTuneCollectHotspotsSpec(invoker);
            invoker.Start();
            Console.WriteLine($"Should have created directory: {invoker.BaseOutDir}, verifying");
            if (Directory.Exists(invoker.BaseOutDir))
            {
                Console.WriteLine("It's there! :)");
            }
            else
            {
                Console.WriteLine("Doesn't work");
            }
            Console.WriteLine($"Trying out spec: {collector.UserDataDirCLI()}");
#endif
            //VTuneToDWJSON.CSReportToDWJson(@"c:\users\perf\Downloads\seventh.csv");

#if false
            VTuneCPUUtilizationParser.CPURecordsFromFilename(@"c:\users\perf\Downloads\samplereport.csv");
#endif
            Console.WriteLine("Hello, world!");

            Console.WriteLine("Press any key....");
            Console.ReadKey();
            return ret;
        }
    }
}
