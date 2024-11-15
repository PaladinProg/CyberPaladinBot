namespace EasyBot.Framework.Models.Telegram
{
    public class TelegramActivity
    {
        public string Text { get; set; }
        public Chat Chat { get; set; }
    }

    public class Chat
    {
        public long Id { get; set; }
    }
}
