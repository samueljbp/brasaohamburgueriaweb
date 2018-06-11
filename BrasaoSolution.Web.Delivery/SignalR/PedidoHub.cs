using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.Identity;
using BrasaoSolution.Model;
using BrasaoSolution.Repository.Context;
using BrasaoSolution.Helper.Extentions;

namespace BrasaoSolution.Web.SignalR
{
    public class PedidoHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnected()
        {
            Clients.User(Context.User.Identity.Name).atualizaSituacaoPedido(1);
            return base.OnConnected();
        }

        [AllowCrossSiteJsonAttribute]
        [HubName("HubMessage")]
        public class MyHub : Hub
        {
            public void SendMessage(string usuario, string codPedido, string situacao)
            {
                if (!String.IsNullOrEmpty(usuario))
                {
                    Clients.User(usuario).messageAdded(codPedido, situacao);
                }
                else
                {
                    Clients.All.messageAdded(codPedido, situacao);
                }
            }
        }

    }
}