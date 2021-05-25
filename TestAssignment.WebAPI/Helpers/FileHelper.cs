using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace TestAssignment.WebAPI.Helpers
{
    public class FileHelper
    { 
        public static bool CheckFile(IFormFile file)
        {
            //Also checking if the file size is less then 2MB
            if (file != null && file.Length <= 2097152 && file.Length > 0)
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                return (extension == ".txt");
            }
            return false;
        }
        public static async Task<string> ReadFormFileAsync(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
