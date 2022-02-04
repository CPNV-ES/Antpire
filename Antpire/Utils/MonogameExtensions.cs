using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Antpire.Utils; 
public static class MonogameExtensions {
    
    /// <summary>
    /// Returns a dictionnary of all available content in the specified directory 
    /// </summary>
    /// <param name="contentFolder">The folder to search content in</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks>From https://stackoverflow.com/a/4052657</remarks>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static Dictionary<string, object> LoadContent(this ContentManager contentManager, string contentFolder) {
        //Load directory info, abort if none
        var dir = new DirectoryInfo(Path.Combine(contentManager.RootDirectory, contentFolder));
        if (!dir.Exists)
            throw new DirectoryNotFoundException();
        
        //Init the resulting list
        var result = new Dictionary<string, object>();

        //Load all files that matches the file filter
        var files = dir.GetFiles("*.*");
        foreach (var file in files) {
            var key = Path.GetFileNameWithoutExtension(file.Name);

            result[contentFolder + "/" + key] = contentManager.Load<object>(
                Path.GetRelativePath(
                    contentManager.RootDirectory,
                    file.FullName.Replace(".xnb", "") 
                )
            );
        }   
        
        return result;
    } 
}
