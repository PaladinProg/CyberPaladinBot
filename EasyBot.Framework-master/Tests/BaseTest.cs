using EasyBot.Framework;
using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Tests.Mock;

namespace Tests
{
    public class BaseTest
    {
        protected ServiceCollection services;

        [SetUp]
        public void BaseSetup()
        {
            services = new ServiceCollection();

            services.AddEasyBotFramework();

            services.AddScoped<IConverter<ChatActivity>, MockConverter>();
            services.AddScoped(typeof(IConnector<>), typeof(MockConnector<>));
        }
    }
}
