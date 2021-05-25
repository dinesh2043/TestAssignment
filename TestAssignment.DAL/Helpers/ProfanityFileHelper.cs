using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace TestAssignment.DAL.Helpers
{
    public class ProfanityFileHelper
    {
        //This code to get the BannedWordsFilePath path and it will work in both local environment as well as in the production environment.
        internal static string BannedWordsPath(ILogger logger)
        {
            try
            {
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "BannedWordsFile.json");
                return xmlPath;
            }
            catch (FileNotFoundException ex)
            {
                logger.LogError("File Not Found in the Project Directory.");
                throw ex;
            }
        }
    }
}
