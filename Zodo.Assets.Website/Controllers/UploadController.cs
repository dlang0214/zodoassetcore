using HZC.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Zodo.Assets.Website.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public UploadController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Image()
        {

            var files = Request.Form.Files;
            if (files.Count == 0 || files[0].Length == 0)
            {
                return Json(new
                {
                    code = -1,
                    message = ""
                });
            }

            var file = files[0];

            var webRootPath = _hostingEnvironment.WebRootPath;
            //string contentRootPath = _hostingEnvironment.ContentRootPath;

            var fileExt = GetFileExt(file.FileName);
            var newFileName = $"{Guid.NewGuid()}.{fileExt}";
            var folderName = DateTime.Today.ToString("yyyyMM");
            var newFolderName = Path.Combine(webRootPath, "upload", folderName);

            if (!Directory.Exists(newFolderName))
            {
                Directory.CreateDirectory(newFolderName);
            }

            var relationPath = Path.Combine("upload", folderName, newFileName);

            var filePath = Path.Combine(webRootPath, relationPath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new
            {
                Code = 200,
                Message = "",
                Body = "/" + relationPath.Replace('\\', '/')
            });
        }

        private string GetFileExt(string fileName)
        {
            return fileName.Split('.').Last();
        }

        [HttpPost]
        public JsonResult Base64(string image)
        {
            try
            {
                //var match = Regex.Match(image, "data:image/([a-zA-Z]+);base64,([\\w\\W]*)$");
                const string ext = "jpg";
                //if (match.Success)
                //{
                //    image = match.Groups[2].Value;
                //    ext = match.Groups[1].Value;
                //}
                //else
                //{
                //    return Json(ResultUtil.Do<string>(ResultCodes.验证失败, "", "无效的图片参数"));
                //}

                var arr = Convert.FromBase64String(image);

                var fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + "." + ext;
                var rootPath = $"{Directory.GetCurrentDirectory()}//wwwroot//upload//{DateTime.Today:yyyyMM}";

                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }
                System.IO.File.WriteAllBytes($"{rootPath}//{fileName}", arr);
                return Json(ResultUtil.Success<string>($"/upload/{DateTime.Today:yyyyMM}/{fileName}"));
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }
    }
}