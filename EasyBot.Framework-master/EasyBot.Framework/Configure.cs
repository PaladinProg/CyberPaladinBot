using AutoMapper;
using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Connectors;
using EasyBot.Framework.Converters;
using EasyBot.Framework.Models.Telegram;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EasyBot.Framework
{
    public static class Configure
    {
        public static void AddEasyBotFramework(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMemoryCache();
            services.AddScoped(typeof(ChatContext<>));
            services.AddScoped(typeof(BotHandler<>));
            services.AddScoped(typeof(IChatStateStorage<>), typeof(MemoryChatStateStorage<>));

        }

        public static void AddTelegramCore(this IServiceCollection services, string botToken)
        {
            services.Configure<TelegramSettings>(s =>
            {
                s.Token = botToken;
            });
            services.AddScoped<IConverter<TelegramActivity>, TelegramConverter>();
            services.AddScoped<IConnector<TelegramActivity>, TelegramConnector>();
        }
    }
}
