namespace Acount.Helpers
{
    public class DocumentSettings
    {
        public static string UploudFile(IFormFile file, string folderName)
        {
            string projectOnePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Acount");
            string folderPeth = Path.Combine(projectOnePath, "wwwroot\\files", folderName);

            string fileName = $"{Guid.NewGuid()}{file.FileName}";

            string filePeth = Path.Combine(folderPeth, fileName);

            using var fs = new FileStream(filePeth, FileMode.Create);

            file.CopyTo(fs);
            return fileName;

        }
        public static void DeleteFile(string folderName, string fileName)
        {
            string folderPeth = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName, fileName);
            if (File.Exists(folderPeth))
                File.Delete(folderPeth);


        }
    }
}
