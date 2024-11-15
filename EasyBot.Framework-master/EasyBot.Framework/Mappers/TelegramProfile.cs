using AutoMapper;
using EasyBot.Framework.Models;
using EasyBot.Framework.Models.Telegram;

namespace EasyBot.Framework.Mappers
{
    class TelegramProfile : Profile
    {
        public TelegramProfile()
        {
            CreateMap<TelegramActivity, TextActivity>().ReverseMap()
                .ForMember(s => s.Chat, s => s.MapFrom(x=> x.Chat))
                .ForMember(s=> s.Text, s=> s.MapFrom(x=> x.Text));

            CreateMap<Models.Telegram.Chat, Models.Chat>().ReverseMap()
                .ForMember(s => s.Id, s => s.MapFrom(x=> x.Id));

        }
    }
}
