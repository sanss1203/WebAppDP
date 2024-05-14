using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WebAppDP.Models
{
    public class ChatHub : Hub
    {
        public void SendMessage(string sender, string message)
        {
            // Mengirim pesan kepada semua klien yang terhubung
            Clients.All.ReceiveMessage(sender, message);
        }
    }

}