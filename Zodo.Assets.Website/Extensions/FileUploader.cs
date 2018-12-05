using HZC.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Zodo.Assets.Website.Extensions
{
    public class FileUploader
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileUploader(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<FileUploadResult> Upload(IFormFile file, string basePath = "upload", FileUploadOption opts = null)
        {
            if (opts == null)
            {
                opts = new FileUploadOption();
            }

            if (file == null || file.Length == 0)
            {
                return new FileUploadResult { Code = (int)ResultCodes.数据不存在, Message = "文件不存在" };
            }

            if (file.Length > opts.SizeLimit)
            {
                return new FileUploadResult { Code = (int)ResultCodes.验证失败, Message = "文件大小超过限制" };
            }

            var fileName = file.Name;
            var ext = GetFileExt(fileName);
            if (!opts.Exts.Contains(ext))
            {
                return new FileUploadResult { Code = (int)ResultCodes.验证失败, Message = "不受支持的文件类型" };
            }
            
            var origName = GetFileName(fileName);
            var dateFolder = DateTime.Today.ToString("yyMM");
            var absSaveFolder = Path.Combine(_hostingEnvironment.WebRootPath, basePath, dateFolder);
            var newFileName = Guid.NewGuid().ToString("N") + "." + ext;
            var filePath = Path.Combine(basePath, dateFolder, newFileName);

            if (!Directory.Exists(absSaveFolder))
            {
                Directory.CreateDirectory(absSaveFolder);
            }

            using (var stream = new FileStream(Path.Combine(absSaveFolder, newFileName), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new FileUploadResult
            {
                Code = 200,
                Message = "",
                OrigName = origName,
                Ext = ext,
                Path = filePath,
                Name = newFileName
            };
        }

        private string GetFileExt(string filename)
        {
            return filename.Split('.').Last();
        }

        private string GetFileName(string fullFilename)
        {
            return fullFilename.Split('/').Last();
        }
    }

    public class FileUploadResult : Result
    {
        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OrigName { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string Ext { get; set; }
        
        /// <summary>
        /// 上传后的文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上传后的文件路径
        /// </summary>
        public string Path { get; set; }
    }

    public class FileUploadOption
    {
        /// <summary>
        /// 大小限制
        /// </summary>
        public int SizeLimit { get; set; } = 2 * 1024 * 1024;

        /// <summary>
        /// 扩展名限制
        /// </summary>
        public string[] Exts { get; set; } = new[] { "png", "jpg", "jpeg", "gif", "doc", "docx", "xls", "xls", "zip", "rar", "7z" };
    }
}
