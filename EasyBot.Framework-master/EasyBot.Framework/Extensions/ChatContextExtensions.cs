using EasyBot.Framework.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EasyBot.Framework.Extensions
{
    public static class ChatContextExtensions
    {
        public static async Task SendTextAsync<TModel>(this ChatContext<TModel> context, ChatActivity activity, string text)
        {
            if (activity is TextActivity textActivity)
            {
                var message = textActivity.Copy();
                message.Text = text;

                await context.SendAsync(message);
            }

            throw new ArgumentException("Activity is not TextActivity");
        }

        public static T Copy<T>(this T item)
        {
            var json = JsonConvert.SerializeObject(item);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
