using EasyBot.Framework.Models;
using System.Threading.Tasks;

namespace EasyBot.Framework.Abstractions
{
    public interface IConverter<TModel>
    {
        Task<ChatActivity> ConvertBack(TModel model);
        Task<TModel> ConvertBack(ChatActivity model);
    }
}
