using EasyBot.Framework;
using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Models.Telegram;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using Tests.States;

namespace Tests
{
    public class TelegramMultipleStateTest : BaseTest
    {
        private BotHandler<TelegramActivity> botHandler;

        [SetUp]
        public void Setup()
        {
            services.AddTelegramCore("634874153:AAHzU5D63iP8a_3Jn_PCjO6NlDcx9_cncak");

            services.AddScoped(typeof(IChatState<>), typeof(TestState1<>));
            services.AddScoped(typeof(TestState1<>));
            services.AddScoped(typeof(TestState2<>));
            services.AddScoped(typeof(TestState3<>));

            var provider = services.BuildServiceProvider();
            botHandler = provider.GetRequiredService<BotHandler<TelegramActivity>>();
        }

        [Test]
        public async Task Handle()
        {
            var json = File.ReadAllText("Jsons/Telegram/TextMessage.json");
            var message = JsonConvert.DeserializeObject<TelegramActivity>(json);

            await botHandler.Handle(message);
            await botHandler.Handle(message);
            await botHandler.Handle(message);
            await botHandler.Handle(message);
        }
    }
}
