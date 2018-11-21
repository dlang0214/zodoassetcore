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
            string _origName;       // 原始文件名
            string _ext;            // 扩展名
            string _dateFolder;     // 服务器端的日期文件夹
            string _absSaveFolder;  // 服务器端文件保存文件夹
            string _fileName;       // 生成的新文件名
            string _filePath;       // 新文件的相对路径

            var fileName = file.Name;
            _ext = GetFileExt(fileName);
            if (!opts.Exts.Contains(_ext))
            {
                return new FileUploadResult { Code = (int)ResultCodes.验证失败, Message = "不受支持的文件类型" };
            }
            
            _origName = GetFileName(fileName);
            _dateFolder = DateTime.Today.ToString("yyMM");
            _absSaveFolder = Path.Combine(_hostingEnvironment.WebRootPath, basePath, _dateFolder);
            _fileName = Guid.NewGuid().ToString("N") + "." + _ext;
            _filePath = Path.Combine(basePath, _dateFolder, _fileName);

            if (!Directory.Exists(_absSaveFolder))
            {
                Directory.CreateDirectory(_absSaveFolder);
            }

            using (var stream = new FileStream(Path.Combine(_absSaveFolder, _fileName), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new FileUploadResult
            {
                Code = 200,
                Message = "",
                OrigName = _origName,
                Ext = _ext,
                Path = _filePath,
                Name = _fileName
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
        public string[] Exts { get; set; } = new string[] { "png", "jpg", "jpeg", "gif", "doc", "docx", "xls", "xls", "zip", "rar", "7z" };
    }
}
