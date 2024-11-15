using EasyBot.Framework.Models;
using System.Threading.Tasks;

namespace EasyBot.Framework.Abstractions
{
    public interface IChatStateStorage<TModel>
    {
        Task<IChatState<TModel>> GetValue(string key);
        Task SaveChanges(string key, IChatState<TModel> value);
    }
}
