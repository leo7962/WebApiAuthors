namespace WebApiAuthors.Services;

public interface IService
{
    public Guid GetTransient();
    public Guid GetScoped();
    public Guid GetSingleton();
}

public class ServicioA : IService
{
    private readonly ILogger<ServicioA> _logger;
    private readonly ServiceScoped _serviceScoped;
    private readonly ServiceSingleton _serviceSingleton;
    private readonly ServiceTransient _serviceTransient;


    public ServicioA(ILogger<ServicioA> logger, ServiceTransient serviceTransient, ServiceScoped serviceScoped,
        ServiceSingleton serviceSingleton)
    {
        _logger = logger;
        _serviceTransient = serviceTransient;
        _serviceScoped = serviceScoped;
        _serviceSingleton = serviceSingleton;
    }

    public Guid GetTransient()
    {
        return _serviceTransient.Guid;
    }

    public Guid GetScoped()
    {
        return _serviceScoped.Guid;
    }

    public Guid GetSingleton()
    {
        return _serviceSingleton.Guid;
    }
}

public class ServiceTransient
{
    public Guid Guid = Guid.NewGuid();
}

public class ServiceScoped
{
    public Guid Guid = Guid.NewGuid();
}

public class ServiceSingleton
{
    public Guid Guid = Guid.NewGuid();
}
