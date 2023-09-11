using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReenbitAPI.Models;
using ReenbitAPI.Services;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReenbitAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormFileController : ControllerBase
    {
        private static string storedEmail = "plskrv8@gmail.com";
        IBlobService _blobservice;
        public FormFileController(IBlobService blobService)
        {
            this._blobservice = blobService;
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] UploadForm form)
        {
            if(form.Email is null)
            {
                return BadRequest();
            }
            string filename = $"{Guid.NewGuid()}{Path.GetExtension(form.File.FileName)}";
            storedEmail = form.Email;
            await _blobservice.UploadBlob(filename, "reactrotnetpract", form.File);
            return Ok("Uploaded successfully.");
        }

        [HttpGet("GetEmail")]
        public IActionResult GetEmail()
        {
            string email = storedEmail;
            return Ok(email);
        }
    }
}
