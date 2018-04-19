// Python Tools for Visual Studio
// Copyright(c) 2018 Intel Corporation.  All rights reserved.
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if false
using CommandLine;
using CommandLine.Text;
#endif

namespace Microsoft.PythonTools.Profiling.ExternalProfilerDriver {
    class Program {

        static void Main() {
            Console.WriteLine("Testing");
        }

#if false

        // [Argument('p', "path")]
        private static bool ReportVTunePath { get; set; }

        // [Argument('n', "dry-run")]
        private static bool DryRunRequested { get; set; }

        // [Operands]
        private static string[] RestArgs { get; set; }

        static void PrintUsage() {
        }

        static int Main(string[] args)
        {
#if false
            try
            {
                Arguments.Populate();
            } catch (ArgumentException aex) {
                Console.WriteLine($"Incorrect form of arguments: {aex.Message}");
                return 1;
            } catch (Exception ex)
            {
                Console.WriteLine("Unidentified error condition");
                return 1;
            }
#endif

            if (true || ReportVTunePath)
            {
                try
                {
                    Console.WriteLine($"The path of VTune is: {VTuneInvoker.VTunePath()}");
                    return 0;
                } catch (VTuneNotInstalledException ex)
                {
                    Console.WriteLine($"VTune not found in expected path: {ex.Message}");
                    return 1;
                }
            }

            string vtuneExec = VTuneInvoker.VTunePath();

            VTuneCollectHotspotsSpec spec = new VTuneCollectHotspotsSpec()
            {
                WorkloadSpec = String.Join(" ", RestArgs)
            };
            string vtuneCollectArgs = spec.FullCLI();

            VTuneReportCallstacksSpec repspec = new VTuneReportCallstacksSpec();
            string vtuneReportArgs = repspec.FullCLI();

            VTuneCPUUtilizationSpec reptimespec = new VTuneCPUUtilizationSpec();
            string vtuneReportTimeArgs = reptimespec.FullCLI();

            if (!DryRunRequested)
            {
                ProcessAsyncRunner.RunWrapper(vtuneExec, vtuneCollectArgs);
                ProcessAsyncRunner.RunWrapper(vtuneExec, vtuneReportArgs);
                ProcessAsyncRunner.RunWrapper(vtuneExec, vtuneReportTimeArgs);
            }

            string tempOutDir = Environment.GetEnvironmentVariable("USERPROFILE");
            string tempOutReportFName = "r_stacks_0001.csv";

            VTuneReportCallstacksSpec repstackspec = new VTuneReportCallstacksSpec()
            {
                ReportOutputFile = Path.Combine(tempOutDir, tempOutReportFName)
            };

            if (!File.Exists(repstackspec.ReportOutputFile))
            {
                Console.WriteLine("Cannot find the VTune report, something went wrong with the profiler process.");
                return 1;
            } else
            {
                var samples = VTuneToDWJSON.ParseFromFile(repspec.ReportOutputFile);
                foreach (var s in samples)
                {
                    Console.WriteLine("{0} : {1}", s.TOSFrame.Function, s.TOSFrame.CPUTime);
                }
            }

            string tracejsonfname = Path.Combine(tempOutDir, "Sampletest.dwjson");
            string cpujsonfname = Path.Combine(tempOutDir, "Sampletest.counters");

            try
            {
                double timeTotal = VTuneToDWJSON.CSReportToDWJson(repspec.ReportOutputFile, tracejsonfname);
                Console.WriteLine($"Time in seconds accounted: {timeTotal}");
                VTuneToDWJSON.CPUReportToDWJson(reptimespec.ReportOutputFile, cpujsonfname, timeTotal);
            } catch (Exception ex)
            {
                Console.WriteLine($"Errors occurred during the processing: {ex.Message}");
                return 1;
            }

            Console.WriteLine("Done!");
            return 0;
        }
#endif

    }
}