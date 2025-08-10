using ECAnalytic.Server.Infrastructure.Contracts;

namespace ECAnalytic.Server.Services
{
	public class FileService(ILogger<FileService> logger) : IFileService
	{
		public async Task SaveToDirAsync(IFormFileCollection files, string fileNameWithPath)
		{
			using (var stream = File.Create(fileNameWithPath))
			{
				foreach (IFormFile file in files)
					try
					{
						await file.CopyToAsync(stream);
					}
					catch (Exception e)
					{
						logger.LogWarning(e.Message);
					}
			}
			logger.LogInformation("File creation finished");
		}
	}
}
