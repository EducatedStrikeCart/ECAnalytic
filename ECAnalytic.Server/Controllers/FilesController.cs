using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mime;
using static Microsoft.EntityFrameworkCore.Query.Internal.ExpressionTreeFuncletizer;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECAnalytic.Server.Controllers
{


	[Route("api/[controller]")]
	[ApiController]
	public class FilesController : ControllerBase
	{
		private IConfiguration _config;
		private ILogger<FilesController> _logger;
		//public IFormFile DataCsv { get; set; }

		public FilesController(IConfiguration config, ILogger<FilesController> logger)
		{
			_config = config;
			_logger = logger;
		}

		// POST api/<ValuesController>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]

		public async Task<IActionResult> Post([FromForm] List<IFormFile> files)
		{

			string? requestHeader = Request.ContentType ?? String.Empty;
			string contentType = requestHeader.Split(';')[0]; // Remove multiform boundary indicator
			if (contentType != "multipart/form-data")
			{
				return new UnsupportedMediaTypeResult();
			}

			string mysqlSecureFilePrivPath = _config["MySQLSecureFileDir"] ?? throw new Exception("MySQL secure file directory not configured");
			string path = Path.GetFullPath(mysqlSecureFilePrivPath);
			// [Do not use user supplied filename](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-9.0#security-considerations)
			string fileName = Path.Combine(mysqlSecureFilePrivPath, Guid.NewGuid() + ".csv");

			if (files.Count > 0)
			{
				using (var stream = System.IO.File.Create(fileName))
				{
					foreach (IFormFile file in files)
						try
						{
							await file.CopyToAsync(stream);
						}
						catch (Exception e)
						{
							_logger.LogWarning(e.Message);
						}
				}
				_logger.LogInformation("File creation finished");
				return Ok();
			}
			return new BadRequestResult();
		}
	}
}
