using EasyBot.Framework.Abstractions;
using EasyBot.Framework.Models;
using System.Threading.Tasks;

namespace Tests.Mock
{
    class MockConverter : IConverter<ChatActivity>
    {
        public async Task<ChatActivity> Convert(ChatActivity model)
        {
            return model;
        }

        public async Task<ChatActivity> ConvertBack(ChatActivity model)
        {
            return model;
        }
    }
}
