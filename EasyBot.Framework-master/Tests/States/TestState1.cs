using EasyBot.Framework;
using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Tests.States
{
    public class TestState1<TModel> : IChatState<TModel>
    {
        private readonly IServiceProvider provider;

        public TestState1(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task<IChatState<TModel>> HandleActivity(ChatActivity activity, ChatContext<TModel> context)
        {
            var textMessage = activity.CreateActivity(GetType().Name);

            await context.SendAsync(textMessage);
            return provider.GetRequiredService<TestState2<TModel>>();
        }
    }
}


