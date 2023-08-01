using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Minishop.Services
{
    public class StorageService : IStorageService
    {
        private readonly string bucketName = "minishop-imagens";

        public async Task<string> UploadFile(IFormFile file, int productId)
        {
            var awsAccessKeyId = "AKIAIOSFODNN7EXAMPLE";
            var awsSecretAccessKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
            var awsCredentials = new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey);
            var s3Config = new AmazonS3Config { ServiceURL = "http://localhost:4566", ForcePathStyle = true };
            var s3Client = new AmazonS3Client(awsCredentials, s3Config);

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            var fileName = file.FileName.Replace(" ", "");

            string key = $"{timestamp}{productId}{fileName}";

            using (var stream = new MemoryStream())
            {
                var bytes = stream.ToArray();
                file.CopyTo(stream);

                PutObjectRequest putObject = new()
                {
                    BucketName = bucketName,
                    Key = key,
                    InputStream = stream
                };

                var response = await s3Client.PutObjectAsync(putObject);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    return "Erro ao enviar a imagem";
                }
            }

            //string imageUrl = $"http://localhost:4566/{bucketName}/{key}";
            //return imageUrl;

            return key;

        }
    }
}

