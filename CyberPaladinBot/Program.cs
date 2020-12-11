using MihaZupan;
using System;
using Telegram.Bot;     
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Linq;
using NLog;
using System.Speech.Synthesis;

namespace CyberPaladinBot
{
    class Program
    {
        static ITelegramBotClient botclient;//обьявление телеграм клиента
        static void Main(string[] args)
        {
            botclient = new TelegramBotClient("1107323100:AAF23RxMiSX0GX9JTjddLg1w_M4SLO48SXg") { Timeout = TimeSpan.FromSeconds(10) };//таймаут нужен чтобы снизить нагрузку бота на телеграм, 
                                                                                                                                       //если не поставить этот параметр то бот будет создавать большую нагрузку и может быть заблокирован
            var me = botclient.GetMeAsync().Result;
            Console.WriteLine($"Bot ID: {me.Id} Bot Name: {me.FirstName}");//проверка работоспособости бота
            logger.Info("Подключение к боту произошло успешно");

            botclient.OnMessage += Bot_OnMessage; //добавляем метод в котором будет функционал
            botclient.StartReceiving();

            Console.ReadKey();
            botclient.StopReceiving();
        }
        //создание логов
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static async void Bot_OnMessage(object sender, MessageEventArgs e)//используем асинхронный метод чтобы поток данных не блокировался во время его получения
        {
            var text = e?.Message?.Text;//получение текста от пользователя
            var message = e.Message;
            string name = $"{message.From.FirstName} {message.From.LastName}";//получение имени пользователя
            if (message.Type == MessageType.Text)
            {//проверка на тип получаемого сообщения, он должен быть текстовым

                Console.WriteLine($"Поступила инормация: '{text}' из чата: '{e.Message.Chat.Id}' от пользователя: {name}");//проверка того что сообщения доходят до бота 

                logger.Info($"Поступило сообщение: '{text}' от пользователя:  {name}");//запись в лог о получении сообщения

                switch (message.Text)
                {
                    case "/start"://команда старта бота(запускается автоматичекски при первом обращении к боту)
                        await botclient.SendTextMessageAsync(//операция вывода текущей погоды
                            chatId: e.Message.Chat,
                            text: $"Доброго времени суток! Для ознакомления с возможностями этого бота, введите команду /help"
                        ).ConfigureAwait(false);
                        break;
                    case "/help"://показ всех доступных команд
                        await botclient.SendTextMessageAsync(//операция вывода текущей погоды
                            chatId: e.Message.Chat,
                            text: $"Доступные команды:\n/time - вывод текущего времени\n/weather - температура в Москве на данный момент\n/currency - выводиит актуальный курс доллара и евро к рублю\n/communication - вывод контактных данных для связи с разработчиком\n/joke - ультрасмешной анекдот\nЛюбой остальной текст будет озвучиватся роботизированным голосом"
                        ).ConfigureAwait(false);
                        break;
                    case "/time"://команда вывода текущего времени
                        DateTime date1 = DateTime.Now;
                        await botclient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"Время по Москве: {date1.ToLongTimeString()}"
                        ).ConfigureAwait(false);
                        break;
                    case "/weather"://команда вывода погоды
                        string url = "http://api.openweathermap.org/data/2.5/weather?q=Moscow&units=metric&appid=5c63497e9d0e4b4a8f836f11c482f278";//берём данные их открытого источника OpenWeather, к сожалению этот сервис не предоставляет информацию по горорду Санкт-Петербург, Поэтому взял Москву
                                                                                                                                                   //units=metric переводит температуру в градусы цельсия
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);//обьект request для запроса
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();//response считывает ответ с request
                        string response;
                        using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))//using для очистки используемых ресурсов
                        {
                            response = streamReader.ReadToEnd();//считывание текста с этого response
                        }
                        WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);//поскольку данные записываются в json нужно преобразовать их в обьект c#

                        await botclient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"Температура в Москве: {weatherResponse.Main.Temp}°С"
                            ).ConfigureAwait(false);
                        break;
                    case "/currency"://команда вывода актуального курса валют к рублю
                        WebClient client = new WebClient();
                        var xml = client.DownloadString("https://www.cbr-xml-daily.ru/daily.xml");//берём данные с Центробанка РФ
                        XDocument xdoc = XDocument.Parse(xml);
                        var el = xdoc.Element("ValCurs").Elements("Valute");
                        string dollar = el.Where(x => x.Attribute("ID").Value == "R01235").Select(x => x.Element("Value").Value).FirstOrDefault();//берём данные по доллару
                        string eur = el.Where(x => x.Attribute("ID").Value == "R01239").Select(x => x.Element("Value").Value).FirstOrDefault();//берём данные по евро

                        await botclient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"Курс валют к рублю на данный момент:\n Евро: {eur} Доллар: {dollar}"
                        ).ConfigureAwait(false);
                        break;
                    case "/joke"://команда вывода анекдотов
                        string[] arr = { "Мать, короче, письмо присылает пацану на кичу – типа, сынок, ты как на крытую пошёл, по хозяйству жопа. Типа уже весна пришла, надо огород перекопать, картошку и другую муйню посадить, сама ни фига не справляюсь, а ни одна сука не поможет. Пацан ей отписывает – типа, мать, оно и классно, ты огород лучше ваще не трогай, так перебейся, а то такое выкопаешь, что не тока мне срок накинут, а и тебя на крытую отправят. Матушка такая через неделю пишет – фигассе, родной, после малявы твоей последней менты вдруг набежали, весь огород перекопали, только не нашли ни хрена. Злые такие были, маты до неба гнули. А пацан ей так отвечает: типа, ну, мать, чем смог – помог, а картошку как-нить уже сама посадишь.",
                        "Короче, сидят два пацана на крыше. Типа косячок на двоих пустили, повтыкали, потом один другому говорит так задумчиво: \n– Маза есть, короче: шмаль жизнь в два раза в натуре уменьшает. \nВторой в непонятках: \n– Это типа как? \nПервый такой: \n– Ну вот тебе лет скока ? \n– Ну, типа, двадцать пять.\n- Ну и вот.А без шмали б уже полсотни было.",
                        "Типа идёт «Что? Где? Когда?». Ведущий такой: \n– А ща вопрос будет задавать реальный пацан Вован. \nВован: \n– Уважаемые знатоки, два месяца назад мой дружбан Колян занял у меня пять сотен и не отдал. Месяц назад Колян занял у меня три сотни и не отдал. Неделю назад он занял всего полтинник , но тоже падла не отдал... Короче, вопрос: кто лежит чёрном ящике ? ",
                        "К Бадяну, кароче по малолетке, типа, „фея“ подвалила. Ну и гутарит: \n— Я либа мозги те дам по жизни ащще чумовые, и память такую, как у батанав внатуре, либа … ты будеш правильным пацаном „при делах“. Ну я его, типа, спрашиваю: \n— Ну и чо? \n— Ну чо чо… Я не помню чо.",
                        "Встречаются два конкретно «подсевших» зомби: \n— Слышь, мен (томным голосом). У те есть чем догнаться? \n— Йоу, чувак! Мне тут сталкер один знакомый такую мощную штуку подогнал. Ваще. КЛАСС. \n— А как вставляет? \n— Та пять секунд и просто ОТПАД!\n— АААФИГЕТЬ. А сильно гребет?\n— Да сталкер сказал, что до полново атрыва башки! \n— УУУУ. КАЙФ.\n— Ах. А называется как?\n— А называется… ГРАНАТА.",
                        "Подходит пацан к барыге. Типа:\n— Граната за пяторку нада?\n— Че,  ну ты гооониш. Давай!\n— Нхаа, держиии… А от за кольцо гани две сотни.",
                        "Идёт такой военный по улице. Его мужик какой-то спрашивает:\n– Куда чешешь?\nТот такой:\n– Ты чё, мужик, охерел? Это ж, блин, военная тайна.\nМужик такой:\n– Извиняюсь, военный, попутал, не знал!А чего несёшь такое тяжёлое, что аж запрел?\nВоенный такой:\n– Да, блин, запреешь тут – патроны эти бронебойные на склад переть…",
                        "Значит, кордон армейский на периметре. Прибыл генерал и, короче, спрашивает у салабона одного:\n– Ну, типа, сынок, как служба?\nТот так – уё-моё, да ничё типа так служба, привыкаю…\nГенерал такой:\n– Во, зелёный, зацени: ты ж теперь всех своих от Зоны защищаешь!Как сюда попал?\nТот такой:\n– Ну, короче, я согласный, шо людей в натуре надо от Зоны беречь…\nГенерал лыбится – о, блин, зашибись, сознательный попался!А тот такой дальше:\n– Я ж и военкому сто раз говорил – нефиг мне тут делать!А он, сука…",
                        "Короче, два братана сидят трындят. Один такой:\n– Сышишь, бpатан, а чё я тебе ещё ни одной наколки не сделал? Мы ж тобой с малолетки корешаемся – вот так, как два пальчика рядом! А ну, давай я тебе на память чё-нить наколю.\nВторой такой:\n– А чё, не вопрос! Тока я сам не знаю, чё колоть.\nПервый такой:\n– А давай танк на спине!\nВторой такой:\n– Ну танк, так танк, ничё так тема... Тока чтоб красиво, сечёшь?\nТот такой:\n– Обижаешь! Опупенно будет, зуб даю!\nИ, короче, пpоходит пять минут.Кольщик такой:\n– Зашибись! Готово.\nВторой такой:\n– Фигассе, чё так быстро?\nКольщик такой:\n– А хуле, там всего четыре буквы.",
                        "Папа съел флешку, и теперь он ПАПКА С ФАЙЛАМИ.",
                        "— Почему в Африке так много болезней?\n— Потому что таблетки нужно запивать водой.",
                        "Встречаютя как то мужики. Один другому говорит:\n-Здарова. У меея 2 новости, хорошая и плохая. С какой начать?\nВторой и отвечает:\n-Ну давай с плохой.\n-Жену твою наши в реке утонула.\n-А хорошая какая?\n-Мы с неё ведро раков наловили"
                        };
                        await botclient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: arr[new Random().Next(0, arr.Length)]
                        ).ConfigureAwait(false);
                        break;
                    case "/communication"://команда выводящая контакты разработчика
                        var inlinekeyboard = new InlineKeyboardMarkup(new[]
                        {
                        InlineKeyboardButton.WithUrl("VK","https://vk.com/lukyanchic"),//кнопка с ссылкой на вк
                        InlineKeyboardButton.WithUrl("Telegram","https://t.me/KiberPaladin")//кнопка с ссылкой на телеграм
                    });
                        await botclient.SendTextMessageAsync(message.From.Id, "Связь с разработчиком", replyMarkup: inlinekeyboard).ConfigureAwait(false);
                        break;
                    default://в этом блоке будут озвучиватся текст не являющийся командами
                        SpeechSynthesizer speechSynth = new SpeechSynthesizer(); //создаём объект

                        speechSynth.Volume = 100; //устанавливаем уровень звука

                        speechSynth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Adult); //также можно устанавливать пол и возвраст озвучки
                        speechSynth.Speak(text);
                        break;
                }
            }
            else if (message.Type == MessageType.Voice)//добавляю реакцию на голосовые сообщения
            {
                logger.Info("Поступило голосовое сообщение от пользователя");
                await botclient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"Ненавижу голосовые сообщения >:("
                      ).ConfigureAwait(false);
                Console.WriteLine($"Поступило голосовое сообщение из чата: '{e.Message.Chat.Id}' от пользователя: {name}");//проверка того что сообщения доходят до бота 
            }
            else
                return;
        }
    }
}
