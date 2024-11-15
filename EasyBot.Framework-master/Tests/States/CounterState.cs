using EasyBot.Framework;
using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Models;
using System.Threading.Tasks;

namespace Tests.States
{
    public class CounterState<TModel> : IChatState<TModel>
    {
        public int Counter { get; set; }

        public async Task<IChatState<TModel>> HandleActivity(ChatActivity activity, ChatContext<TModel> context)
        {
            await context.SendAsync(new TextActivity
            {
                Chat = activity.Chat,
                Text = $"Count: {Counter++}"
            });

            return this;
        }
    }
}


