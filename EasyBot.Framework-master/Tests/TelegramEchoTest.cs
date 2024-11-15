using EasyBot.Framework;
using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Models;
using EasyBot.Framework.Models.Telegram;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;
using Tests.States;

namespace Tests
{
    class TelegramEchoTest : BaseTest
    {
        private BotHandler<TelegramActivity> botHandler;

        [SetUp]
        public void Setup()
        {
            services.AddTelegramCore("634874153:AAHzU5D63iP8a_3Jn_PCjO6NlDcx9_cncak");

            services.AddScoped(typeof(IChatState<>), typeof(EchoState<>));

            var provider = services.BuildServiceProvider();
            botHandler = provider.GetRequiredService<BotHandler<TelegramActivity>>();
        }

        [Test]
        public async Task Handle()
        {
            var message = new TelegramActivity
            {
                Chat = new EasyBot.Framework.Models.Telegram.Chat
                {
                    Id = 239678169
                },
                Text = "test",
            };

            await botHandler.Handle(message);
        }
    }
}
