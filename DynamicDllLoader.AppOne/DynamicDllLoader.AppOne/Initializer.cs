namespace DynamicDllLoader.AppOne;

public class Initializer : IInitializer
{
    public void Start()
    {
        Console.WriteLine("App One started updated");
    }

    public void Stop()
    {
        Console.WriteLine("App One stopped");
    }
}
