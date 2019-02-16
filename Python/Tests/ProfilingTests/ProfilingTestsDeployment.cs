// Python Tools for Visual Studio
// Copyright(c) 2019 Intel Corporation.  All rights reserved.
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
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PythonTools.Infrastructure;
using Microsoft.PythonTools.Profiling;
using Microsoft.PythonTools.Profiling.ExternalProfilerDriver;

using Trace = System.Diagnostics.Trace;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;

namespace ProfilingTestsDeployment {
    [TestClass]
    public class ProfilingTestsDeployment
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void DoDeployment(TestContext context) {
            AssertListener.Initialize();
        }

        [TestMethod]
        public void TestParsing()
        {
            string filename = TestData.GetPath(@"TestData\ExternalProfilerDriverData\zlib_example.csv");
            Assert.IsTrue(File.Exists(filename));
            int expected_sample_count = 5;

            var samples = VTuneToDWJSON.ParseFromFile(filename).ToList();
            Assert.AreEqual(samples.Count, expected_sample_count);

            Assert.IsInstanceOfType(samples[0], typeof(SampleWithTrace));

            string known_module = "libz.so.1";
            
            // LCR this throws an exception, works on VSCode test
            var dict = VTuneToDWJSON.ModuleFuncDictFromSamples(samples);
            Assert.IsTrue(dict.ContainsKey(known_module));

            foreach (var m in dict)
            {
                Dictionary<string, FuncInfo> v = m.Value;
                Trace.WriteLine($"Main Key: {m.Key}");
                foreach (var vkk in v)
                {
                    Trace.WriteLine($"Key: {vkk.Key}, Value: [{vkk.Value.FunctionName}, {vkk.Value.SourceFile}, {vkk.Value.LineNumber}]");
                }
            }

            // LCR this assert doesnt work ? This works in my VScode test.
            // Assert.ThrowsException<ArgumentException>(() => VTuneToDWJSON.AddLineNumbers(ref dict, "/etc/test"));
            // int initial_count = dict.Count;

            // LCR havent tested this one which will read windows pdbs.  
            // VTuneToDWJSON.AddLineNumbers(ref dict, "C:\\Users\\clairiky\\Documents\\zlib-1.2.11");
            // Assert.AreEqual(initial_count, dict.Count);


            foreach (var m in dict)
            {
                Dictionary<string, FuncInfo> v = m.Value;
                Trace.WriteLine($"Main Key: {m.Key}");
                foreach (var vkk in v)
                {
                    Trace.WriteLine($"Key: {vkk.Key}, Value: [{vkk.Value.FunctionName}, {vkk.Value.SourceFile}, {vkk.Value.LineNumber}]");
                }
            }

        }

        [TestMethod]
        public void TestParseFromFile()
        {
            string filename = TestData.GetPath(@"TestData\ExternalProfilerDriverData\r_stacks_0000.csv");
            Assert.IsTrue(File.Exists(filename));
            //int expected_sample_count = 5;
            //LCR This function doesnt exists in the PTVS version of VTuneStackParser. If we dont need it we can remove this test.
            //var samples = VTuneStackParser.ParseFromFile(filename).ToList();
            //Assert.AreEqual(samples.Count, expected_sample_count);
        }

#if false // LCR this will be the tests that we need for SymbolReader.
        [TestMethod]
        [DeploymentItem("something.pdb")]
        public void LoadTest()
        {

            Assert.IsTrue(true);

           /* string known_filename = "something.pdb";
            Assert.IsTrue(File.Exists(known_filename));
            SymbolReader symreader = SymbolReader.Load(known_filename);
            Assert.IsTrue(symreader != null);

            
            const int expected_symbol_count = 150;            
            var syms = symreader.FunctionLocations(known_filename).ToList();
            Assert.AreEqual(syms.Count, expected_symbol_count); */

        }
#endif
    }
}
