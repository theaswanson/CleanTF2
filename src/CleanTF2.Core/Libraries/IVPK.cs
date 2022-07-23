namespace CleanTF2.Core.Libraries
{
    public interface IVPK
    {
        Task Run(string tf2Directory, string directoryToPack, bool produceMultiChunkVPK = false);
    }
}
