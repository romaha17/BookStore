namespace BookStore.Application.Settings;

public class OpenAISettings
{
    public string OpenAIKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o";
}
