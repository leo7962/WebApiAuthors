namespace WebApiAuthors.Services;

public class WriteFile : IHostedService
{
    private readonly IWebHostEnvironment _env;
    private readonly string fileName = "archivo1.txt";
    private Timer _timer;

    public WriteFile(IWebHostEnvironment env)
    {
        _env = env;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        Write("Proceso iniciado");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Dispose();
        Write("Proceso finalizado");
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        Write("Proceso en ejecución: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt zz"));
    }

    private void Write(string message)
    {
        var path = $@"{_env.ContentRootPath}\wwwroot\{fileName}";
        using (var writer = new StreamWriter(path, true))
        {
            writer.WriteLine(message);
        }
    }
}
