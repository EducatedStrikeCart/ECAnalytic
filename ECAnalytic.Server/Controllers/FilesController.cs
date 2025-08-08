using ECAnalytic.Server.Infrastructure.Contracts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECAnalytic.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[EnableCors("DefaultPolicy")]
	public class FilesController(IConfiguration config, ILogger<FilesController> logger, IFileService fileService) : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			return new BadRequestResult();
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]

		public async Task<ActionResult> Post(IFormFileCollection files)
		{
			string? requestHeader = Request.ContentType ?? String.Empty;
			string contentType = requestHeader.Split(';')[0]; // Remove multiform boundary indicator
			if (contentType != "multipart/form-data") return new UnsupportedMediaTypeResult();

			string mysqlSecureFilePrivPath = config["MySQLSecureFileDir"] ?? throw new Exception("MySQL secure file directory not configured");
			
			// [Do not use user supplied filename](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-9.0#security-considerations)
			string fileNameWithPath = Path.Combine(mysqlSecureFilePrivPath, Guid.NewGuid() + ".csv");
			
			await fileService.SaveToDirAsync(files, fileNameWithPath);
			
			return NoContent();
		}
	}
}
