using AutoMapper;
using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Models;
using EasyBot.Framework.Models.Telegram;
using System;
using System.Threading.Tasks;

namespace EasyBot.Framework.Converters
{
    public class TelegramConverter : IConverter<TelegramActivity>
    {
        private readonly IMapper mapper;

        public TelegramConverter(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<ChatActivity> ConvertBack(TelegramActivity model)
        {
            if (!string.IsNullOrEmpty(model.Text))
            {
                return mapper.Map<TextActivity>(model);
            }

            throw new NotSupportedException("Не поддерживается конвертирование");
        }

        public async Task<TelegramActivity> ConvertBack(ChatActivity model)
        {
            if (model is TextActivity textMessage)
            {
                return mapper.Map<TelegramActivity>(textMessage);
            }

            throw new NotSupportedException("Не поддерживается конвертирование");
        }
    }
}
