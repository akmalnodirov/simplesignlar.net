using Core.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core;

public static class ClientsService
{
    public static readonly Dictionary<string, ClientData> ConnectedClients = new Dictionary<string, ClientData>();
}
