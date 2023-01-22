namespace Client.Pages;

public partial class Index
{
    bool _processing = false;
    public DummyModel Model { get; set; } = new();

    private Task SubmitAsync()
    {
        _processing = true;

        Model.Response = "Done";

        _processing = false;

        return Task.CompletedTask;
    }
}
public class DummyModel
{
    public string Response { get; set; } = string.Empty;
}
