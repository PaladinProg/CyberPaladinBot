using EasyBot.Framework;
using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using Tests.Mock;
using Tests.States;

namespace Tests
{
    class MultipleStateTests : BaseTest
    {
        private BotHandler<ChatActivity> botHandler;
        private MockConnector<ChatActivity> connector;
        private TestState1<TextActivity> state1;
        private TestState2<TextActivity> state2;
        private TestState3<TextActivity> state3;

        [SetUp]
        public void Init()
        {
            services.AddScoped(typeof(IChatState<>), typeof(TestState1<>));
            services.AddScoped(typeof(TestState1<>));
            services.AddScoped(typeof(TestState2<>));
            services.AddScoped(typeof(TestState3<>));

            var provider = services.BuildServiceProvider();

            botHandler = provider.GetRequiredService<BotHandler<ChatActivity>>();
            connector = provider.GetRequiredService<IConnector<ChatActivity>>() as MockConnector<ChatActivity>;

            state1 = provider.GetRequiredService<TestState1<TextActivity>>();
            state2 = provider.GetRequiredService<TestState2<TextActivity>>();
            state3 = provider.GetRequiredService<TestState3<TextActivity>>();
        }

        [Test]
        public async Task Handle()
        {
            var message = new TextActivity
            {
                Chat = new Chat { Id = "26" },
                Text = "test",
            };

            var messages = connector.Messages.Cast<TextActivity>();

            await botHandler.Handle(message);
            Assert.AreEqual(state1.GetType().Name, messages.Last().Text);

            await botHandler.Handle(message);
            Assert.AreEqual(state2.GetType().Name, messages.Last().Text);

            await botHandler.Handle(message);
            Assert.AreEqual(state3.GetType().Name, messages.Last().Text);

            await botHandler.Handle(message);
            Assert.AreEqual(state1.GetType().Name, messages.Last().Text);
        }

    }
}
