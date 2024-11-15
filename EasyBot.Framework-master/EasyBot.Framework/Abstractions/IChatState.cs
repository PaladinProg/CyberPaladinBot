using EasyBot.Framework.Models;
using System.Threading.Tasks;

namespace EasyBot.Framework.Abstractions
{
    public interface IChatState<TModel>
    {
        Task<IChatState<TModel>> HandleActivity(ChatActivity activity, ChatContext<TModel> context);
    }
}
