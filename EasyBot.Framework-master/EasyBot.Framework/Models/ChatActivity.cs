using System.IO;

namespace EasyBot.Framework.Models
{
    public abstract class ChatActivity
    {
        public string Id { get; set; }
        public Chat Chat { get; set; }
        public User Sender { get; set; }

        public TextActivity CreateActivity(string text)
        {
            return new TextActivity
            {
                Chat = Chat,
                Sender = null,
                Text = text
            };
        }
    }

    public class TextActivity : ChatActivity
    {
        public string Text { get; set; }
    }

    public class FileActivity : ChatActivity
    {
        public Stream FileStream { get; set; }
    }
}
