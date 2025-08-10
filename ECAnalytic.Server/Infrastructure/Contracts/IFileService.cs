namespace ECAnalytic.Server.Infrastructure.Contracts
{
	public interface IFileService
	{
		public Task SaveToDirAsync(IFormFileCollection file, string fileNameWithPath);
	}
}
