using System.Diagnostics;

namespace CleanTF2.Core
{
    public interface IProcessRunner
    {
        Process Start(string fileName, IEnumerable<string> arguments);
    }
}