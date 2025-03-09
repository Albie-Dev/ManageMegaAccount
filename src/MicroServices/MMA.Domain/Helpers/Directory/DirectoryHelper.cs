
namespace MMA.Domain
{
    public class DirectoryHelper
    {
        public static string GetSolutionBasePath()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory?.FullName ?? string.Empty;
        }


        public static string GetHtmlTemplatePath(string templateFileName, CMicroserviceType serviceType)
        {
            return Path.Combine(GetSolutionBasePath(), @$"src\Microservices\{serviceType.ToString()}\{serviceType.ToString()}.Service\Templates\Email\Html\{templateFileName}");
        }
    }
}