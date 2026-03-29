namespace BookStore.Application.Settings;

public class AwsSettings
{
    public string Region { get; set; } = string.Empty;
    public string BucketName { get; set; } = "bookstore-media228";
    public string BaseUrl { get; set; } = string.Empty;
}
