namespace MovieDatabase.Services
{
    public class FileService
    {
        public async Task SaveFileAsync(string fileName, string contents)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            await File.WriteAllTextAsync(filePath, contents);
        }

        public async Task<string?> LoadFileAsync(string fileName)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            if (!File.Exists(filePath))
            {
                return null;
            }

            return await File.ReadAllTextAsync(filePath);
        }
    }
}
