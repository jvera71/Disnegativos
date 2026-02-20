using Disnegativos.Shared.Interfaces;
using Microsoft.Maui.Networking;

namespace Disnegativos.Services;

public class ConnectivityService : IConnectivityService
{
    private readonly IConnectivity _connectivity;

    public bool IsOnline => _connectivity.NetworkAccess == NetworkAccess.Internet;

    public event EventHandler<bool>? ConnectivityChanged;

    public ConnectivityService(IConnectivity connectivity)
    {
        _connectivity = connectivity;
        _connectivity.ConnectivityChanged += (s, e) =>
        {
            ConnectivityChanged?.Invoke(this, IsOnline);
        };
    }
}
