//using Amazon.S3;
//using Amazon.S3.Model;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Minishop.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class FilesController : ControllerBase
//    {
//        private readonly IAmazonS3 _amazonS3;

//        public FilesController(IAmazonS3 amazonS3)
//        {
//            _amazonS3 = amazonS3;
//        }

//        [HttpPost]
//        public async Task<IActionResult> UploadFileAsync(IFormFile file, string bucketName, string prefix)
//        {
//            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
//            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
//            var request = new PutObjectRequest()
//            {
//                BucketName = bucketName,
//                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
//                InputStream = file.OpenReadStream(),
//            };
//            request.Metadata.Add("Content-Type", file.ContentType);
//            await _amazonS3.PutObjectAsync(request);
//            return Ok($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
//        }
//    }
//}
