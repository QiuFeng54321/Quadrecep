using CommandLine;

namespace Qua2Qbm
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input map to be processed.")]
        public string InputFile { get; set; }
        [Option('o', "output", Required = true, HelpText = "Output map.")]
        public string OutputFile { get; set; }
    }
}