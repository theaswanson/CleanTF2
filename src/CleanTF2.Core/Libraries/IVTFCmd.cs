namespace CleanTF2.Core.Libraries
{
    public interface IVTFCmd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">Input file path.</param>
        /// <param name="folder">Input directory search string.</param>
        /// <param name="format">Ouput format to use on non-alpha textures.</param>
        /// <param name="alphaFormat">Ouput format to use on alpha textures.</param>
        /// <param name="exportFormat">Ouput format to use on alpha textures.</param>
        /// <param name="flag">Output flags to set.</param>
        /// <param name="version">Ouput version.</param>
        /// <returns></returns>
        Task Run(string? file = null, string? folder = null, string? format = null, string? alphaFormat = null, string? exportFormat = null, string? flag = null, string? version = null);
    }
}
