using Amazon.S3;
using Amazon.S3.Model;
using BookStore.Application.Interfaces;
using BookStore.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookStore.Infrastructure.Services;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucketName;
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(IAmazonS3 s3, IOptions<AwsSettings> options, ILogger<S3StorageService> logger)
    {
        _s3 = s3;
        _bucketName = options.Value.BucketName;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(Stream stream, string key, string contentType, CancellationToken ct = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key.TrimStart('/'),
            InputStream = stream,
            ContentType = contentType
        };

        await _s3.PutObjectAsync(request, ct);
        return key;
    }

    public Task<string> GetPresignedUrlAsync(string key, TimeSpan expiry, CancellationToken ct = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key.TrimStart('/'),
            Expires = DateTime.UtcNow.Add(expiry)
        };

        var url = _s3.GetPreSignedURL(request);
        return Task.FromResult(url);
    }

    public async Task DeleteFileAsync(string key, CancellationToken ct = default)
    {
        try
        {
            await _s3.DeleteObjectAsync(_bucketName, key.TrimStart('/'), ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete S3 object with key {Key}", key);
        }
    }
}
