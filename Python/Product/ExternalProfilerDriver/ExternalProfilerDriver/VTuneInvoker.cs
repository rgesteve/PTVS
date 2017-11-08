using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.DotNet.PlatformAbstractions;


namespace ExternalProfilerDriver
{
    public class VTuneInvoker
    {
        private static readonly string _vtuneCl = @"\bin32\amplxe-cl.exe";
        private static readonly string _vtune17Envvar = "VTUNE_AMPLIFIER_2017_DIR";
        private static readonly string _vtune18Envvar = "VTUNE_AMPLIFIER_2018_DIR";

        public static string VTunePath()
        {
            // expecting something like "C:\\Program Files (x86)\\IntelSWTools\\VTune Amplifier XE 2017";
            string envvarval;
            if (RuntimeEnvironment.OperatingSystemPlatform == Platform.Windows ||
                 RuntimeEnvironment.OperatingSystemPlatform == Platform.Linux)
            {
                envvarval = Environment.GetEnvironmentVariable(_vtune17Envvar);
                if (envvarval == null)
                {
                    envvarval = Environment.GetEnvironmentVariable(_vtune18Envvar);
                }
                if (envvarval == null)
                {
                    throw new VTuneNotInstalledException();
                }

            }
            else
            {
                envvarval = "OS not supported"; // should this throw an exception?
            }
            if (File.Exists(envvarval + _vtuneCl)) // not exactly sure why Path.Combine doesn't work here
            {
                return envvarval + _vtuneCl;
            }
            else
            {
                // TODO: probably should throw an exception here
                return string.Format("{0} does not exist, on path [{1}]", Path.Combine(envvarval, _vtuneCl), envvarval);
            }
        }

        private readonly string _path;        // vtune path
        private string _baseOutDir = "";  // user data dir
        private readonly string _resultDir = "";   // path of directory to store/retrieve collected results
                                                   // empty if collection has not started
        private readonly string _profiledCL;

        // VTune organizes its collections in a two-level hierarchy: BaseOutDir/ResultDir
        public string BaseOutDir { get { return _baseOutDir; } }
        public string ResultDir { get { return _resultDir; } }

        private VTuneCollectSpec CollectSpec { get; set; }
        private IEnumerable<VTuneReportSpec> ReportSpecs { get; set; }

        /// <summary>
        /// A reference to an invocation of VTune that has as base ("user data") directory <paramref name="baseOutDir"/>
        /// </summary>
        /// <param name="baseOutDir"></param>
        /// <param name="vtunePath"></param>
        public VTuneInvoker(string baseOutDir, string vtunePath = "")
        {
            _baseOutDir = baseOutDir;
            _path = vtunePath;
        }

        public string CollectCL()
        {
            return "teststring";
        }

        public string Report()
        {
            //return Path.Combine(_vtunePath, _vtuneCl);
            return "";// throw notimplemented
        }

        public void Start()
        {
            EnsureBaseDir();
            Console.WriteLine("Should be executing....");
        }

        private void EnsureBaseDir()
        {
            string possible = BaseOutDir;
            if (Directory.Exists(possible)) return;

            string filename = Path.GetFileNameWithoutExtension(possible);
            string date = DateTime.Now.ToString("yyyyMMdd");
            string candidatedirname = Path.Combine(Path.GetTempPath(), filename + "_" + date + ".vt");

            int count = 1;
            while (Directory.Exists(candidatedirname))
            {
                candidatedirname = Path.Combine(Path.GetTempPath(), filename + "_" + date + "(" + count + ").vt");
                count++;
            }
            Directory.CreateDirectory(candidatedirname);
            _baseOutDir = candidatedirname;
        }

        public void AddCollectorSpec(VTuneCollectSpec collector)
        {
            CollectSpec = collector;
        }

        private static string NextResultDirInDir(string basedir)
        {
            if (!Directory.Exists(basedir))
            {
                throw new ArgumentException($"Expected directory {basedir} does not exist");
            }

            int latest = 0;
            IEnumerable<string> previous = Directory.GetDirectories(basedir, "r*hs");
            if (previous.Count() != 0)
            {
                latest = previous
                           .Select(x => { var n = new FileInfo(x).Name; return n.Substring(1, n.Length - 3); })
                           .Select(x => Int32.Parse(x))
                           .Max();
                latest += 1;
            }

            var latestReportName = "r" + latest.ToString("D3") + "hs"; // what happens if there's none?
            return latestReportName;
        }
    }

    public class VTuneNotInstalledException : Exception
    {
        public override string Message
        {
            get
            {
                return "Only VTune 2017 or 2018 supported, see https://software.intel.com/en-us/intel-vtune-amplifier-xe";
            }
        }
    }

    public abstract class VTuneSpec
    {
        protected VTuneInvoker _invoker;
        public VTuneSpec(VTuneInvoker invoker)
        {
            // should verify this is not null
            _invoker = invoker;
        }
        public abstract string FullCLI();
        public string UserDataDirCLI()
        {
            return "-user-data-dir=" + _invoker.BaseOutDir; // should be outputdir in PTVS
        }

    }

    public abstract class VTuneCollectSpec : VTuneSpec
    {
        public VTuneCollectSpec(VTuneInvoker invoker) : base(invoker)
        {
            _invoker.AddCollectorSpec(this);
        }
        public abstract string AnalysisName { get; }
        public string CLISpec
        {
            get
            {
                return "-collect" + " " + AnalysisName; // is there a symbolic constant for space?
            }
        }
    }

    public class VTuneCollectHotspotsSpec : VTuneCollectSpec
    {
        public VTuneCollectHotspotsSpec(VTuneInvoker _invoker) : base(_invoker) { }
        public override string AnalysisName { get { return "hotspots"; } }
        public override string FullCLI()
        {
            string ret;
            ret = CLISpec;
            return ret;
        }
    }

    public abstract class VTuneReportSpec : VTuneSpec
    {
        private string _reportOutFile = "<unassigned>";
        public string ReportOutputFile { get { return _reportOutFile; } }
        protected VTuneReportSpec(VTuneInvoker invoker, string reportName) : base(invoker)
        {
            _reportPath = reportName;
        }
        public abstract string ReportName { get; }
        private string _reportPath;
        public string CLISpec
        {
            get
            {
                return "-report" + " " + ReportName;
            }
        }
        public override string FullCLI()
        {
            return "-format=csv -csv-delimiter=comma" + " " + "-report-output=" + ReportOutputFile;
        }
    }

    public class VTuneCollectCallstacksSpec : VTuneReportSpec
    {
        public VTuneCollectCallstacksSpec(VTuneInvoker invoker, string reportName = "") : base(invoker, reportName) { }
        public override string ReportName { get { return "callstacks"; } }

        public override string FullCLI()
        {
            string ret;
            ret = CLISpec;
            return ret;
        }
    }

    public class VTuneCPUUtilizationSpec : VTuneReportSpec
    {
        private Dictionary<string, string> _knobs;
        public VTuneCPUUtilizationSpec(VTuneInvoker invoker, string reportName = "") : base(invoker, reportName)
        {
            _knobs = new Dictionary<string, string>();
            _knobs.Add("column-by", "CPUTime"); // these are case-sensitive
            _knobs.Add("query-type", "overtime");
            _knobs.Add("bin_count", "15");
            _knobs.Add("group-by", "Process/Thread");
        }

        public override string ReportName { get { return "time"; } }

        public override string FullCLI()
        {
            StringBuilder ret = new StringBuilder(CLISpec);
            ret.Append(" ");
            foreach (KeyValuePair<string, string> kv in _knobs)
            {
                ret.Append($"-r-k {kv.Key}={kv.Value}"); //should these be quoted?
                ret.Append(" ");
            }
            ret.Append(base.FullCLI());
            return ret.ToString();
        }
    }
}
