using EasyBot.Framework;
using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Models;
using System.Threading.Tasks;

namespace Tests.States
{
    public class EchoState<TModel> : IChatState<TModel>
    {
        public async Task<IChatState<TModel>> HandleActivity(ChatActivity activity, ChatContext<TModel> context)
        {
            await context.SendAsync(activity);

            return this;
        }
    }
}


