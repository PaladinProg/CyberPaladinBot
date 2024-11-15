using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO; 
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Linq;
using System.Speech.Synthesis;
using MySql.Data.MySqlClient;
using System.Data;

namespace CyberPaladinBot
{
    class Program
    {
        //создание логов

        static ITelegramBotClient botclient;//обьявление телеграм клиента
        static void Main(string[] args)
        {
            try
            {
                botclient = new TelegramBotClient("1107323100:AAF23RxMiSX0GX9JTjddLg1w_M4SLO48SXg") { Timeout = TimeSpan.FromSeconds(10) };//таймаут нужен чтобы снизить нагрузку бота на телеграм, 
                                                                                                                                           //если не поставить этот параметр то бот будет создавать большую нагрузку и может быть заблокирован
                var me = botclient.GetMeAsync().Result;
                Console.WriteLine($"Bot ID: {me.Id} Bot Name: {me.FirstName}");//проверка работоспособости бота
                AddToDatabase.AddToBot_Activicy("Подключение к боту прошло успешно");

                botclient.OnMessage += Bot_OnMessage; //добавляем метод в котором будет функционал
                botclient.StartReceiving();

                Console.ReadKey();
                botclient.StopReceiving();
            }
            catch (Exception ex)
            {
                AddToDatabase.AddToError(ex.Message, "bot");
            }
        }


        private static int? GetPort(IReadOnlyList<string> text)
        {
            int port;
            if (text.Count == 4 && int.TryParse(text[3], out port))
                return port;
            return null;
        }

        public static List<string> ph = new List<string>(); //переменная хранящая путь к файлу
        static int top = 4; //переменная хранящая позицию ряда
        public static void Find(string pathDir, string file)
        {
            DirectoryInfo d;
            try
            {
                d = new DirectoryInfo(pathDir);
                DirectoryInfo[] w = d.GetDirectories(); //подкаталоги

                foreach (var item1 in w)
                {
                    //проверка на системную директорию, если системная пропускаем
                    if (item1.Attributes.Equals(FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory))
                    {
                        continue; //выходим из цикла
                    }
                    Find(item1.FullName, file); //Рекурсия
                }

                if (Directory.Exists($"{d.FullName}") == true)
                {
                    //получаем коллекцию всех файлов в директории
                    string[] arrFile = Directory.EnumerateFiles(d.FullName.ToString()).ToArray();

                    for (int n = 0; n < arrFile.Length; n++)
                    {
                        Console.SetCursorPosition(0, 0);
                        var ew = Path.GetFileName(arrFile[n]);


                        Console.SetCursorPosition(0, 0); //тут мы возвращаем каретку на позицию начала строки
                        Console.WriteLine(new string(' ', 200));//затираем предложения
                        Console.SetCursorPosition(0, 0);

                        Console.WriteLine(ew);
                        if (ew.Equals(file))
                        {
                            Console.SetCursorPosition(0, top);//устанавливаем каретку в начало и по значению top 
                            Console.WriteLine("Фаил найден!");
                            Console.WriteLine(arrFile[n]);
                            ph.Add(arrFile[n].ToString());

                            top += 5;//увеличиваем tоp 
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Фаил не существует");
                }
            }
            catch (Exception)
            {
                return;
            }
        }


        public static async void Bot_OnMessage(object sender, MessageEventArgs e)//используем асинхронный метод чтобы поток данных не блокировался во время его получения
        {
            try{
            var text = e?.Message?.Text;//получение текста от пользователя
            var message = e.Message;
            string name = $"{message.From.FirstName} {message.From.LastName}";//получение имени пользователя

            var text1 = e?.Message?.Text.Split(' ');
            
                if (message.Type == MessageType.Text)
                {//проверка на тип получаемого сообщения, он должен быть текстовым

                    Console.WriteLine($"Поступила инормация: '{text}' из чата: '{e.Message.Chat.Id}' от пользователя: {name}");//проверка того что сообщения доходят до бота 
                    AddToDatabase.AddToMessages(text, name);

                    switch (message.Text)
                    {
                        case "/start"://команда старта бота(запускается автоматичекски при первом обращении к боту)
                            await botclient.SendTextMessageAsync(//операция вывода текущей погоды
                                chatId: e.Message.Chat,
                                text: $"Доброго времени суток! Для ознакомления с возможностями этого бота, введите команду /help"
                            ).ConfigureAwait(false);
                            AddToDatabase.AddToCommands(text1[0], name);
                            break;
                        case "/help"://показ всех доступных команд
                            await botclient.SendTextMessageAsync(//операция вывода текущей погоды
                                chatId: e.Message.Chat,
                                text: $"Доступные команды:\n/time - вывод текущего времени\n/weather - вывод температуры заданного вами города на данный момент\n/currency - выводиит актуальный курс доллара и евро к рублю\n/communication - вывод контактных данных для связи с разработчиком\n/joke - ультрасмешной анекдот, по комаде /addjoke вы можете добавить свой анекдот в бота, чтобы другие пользователи посмеялись\n/remotepc - режим для удаленного взаимодействия с ПК"
                            ).ConfigureAwait(false);
                            AddToDatabase.AddToCommands(text1[0], name);
                            break;
                        case "/time"://команда вывода текущего времени
                            DateTime date1 = DateTime.Now;
                            await botclient.SendTextMessageAsync(
                                chatId: e.Message.Chat,
                                text: $"Время по Москве: {date1.ToLongTimeString()}"
                            ).ConfigureAwait(false);
                            AddToDatabase.AddToCommands(text1[0], name);
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
                            AddToDatabase.AddToCommands(text1[0], name);
                            break;
                        case "/joke"://команда вывода анекдотов
                            string con = "server=localhost;user=root;password=1234;database=diplom;port=3306";
                            string CommandText = "SELECT * FROM jokes ORDER BY RAND() LIMIT 1";
                            MySqlConnection myCon = new MySqlConnection(con);
                            MySqlCommand myCom = new MySqlCommand(CommandText, myCon);
                            myCon.Open();
                            myCom.ExecuteNonQuery();
                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(myCom);
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);
                            await botclient.SendTextMessageAsync(message.From.Id, dt.Rows[0][1].ToString()).ConfigureAwait(false);
                            AddToDatabase.AddToCommands(text1[0], name);
                            break;
                        case "/communication"://команда выводящая контакты разработчика
                            var inlinekeyboard = new InlineKeyboardMarkup(new[]
                            {
                        InlineKeyboardButton.WithUrl("VK","https://vk.com/kiberpaladin1227"),//кнопка с ссылкой на вк
                        InlineKeyboardButton.WithUrl("Telegram","https://t.me/KiberPaladin")//кнопка с ссылкой на телеграм
                        });
                            await botclient.SendTextMessageAsync(message.From.Id, "Связь с разработчиком", replyMarkup: inlinekeyboard).ConfigureAwait(false);
                            AddToDatabase.AddToCommands(text1[0], name);
                            break;

                        case "/remotepc"://инструкция для удаленной работы с компьютером
                            {
                                await botclient.SendTextMessageAsync(message.From.Id, "Режим удаленной работы с ПК \n " +
                                    "Для включения ПК наберите команду /turnON со след параметрами [ip mac port]. Инструкция по настройке устройства для использования этой функции доступна по команде /WOLInstruction\n" +
                                    "Для получения файла с вашего ПК введите комаду /getfile его имя и расширение. Пример использования: /getfile test.txt. Вы можете указать другой диск для поиска для этого используйте команду в следующем виде: /getfile C:\\ имя_файла.txt. По умолчанию используется диск C:\n" +
                                    "Для выключения введите команду /turnOFF\n" +
                                    "Для запуска приложения или открытия URL страницы введите /run [URL/path]\n" +
                                    "Для получения скриншота экрана введите /getscreen").ConfigureAwait(false);
                            }
                            AddToDatabase.AddToCommands(text1[0], name);
                            break;
                        case "/WOLInstruction"://инструкция для использования wol
                            {
                                await botclient.SendTextMessageAsync(message.From.Id, "Что требуется для работы?\n " +
                                    "Материнская плата компьютера с питанием ATX, 3 - х пиновый WOL коннектор и ATX блок питания. Сетевая карта поддерживающая WOL с подключенным WOL коннектором в материнскую плату, либо соответствующая стандарту PCI 2.2(или позднему).\n" +
                                    "В БИОСе нужно включить поддержку Wake - On - LAN.Опции в БИОСе могут иметь следующие названия:\n" +
                                    "MAC Resume From S3 / S4,\n" +
                                    "MACPME Power Up Control,\n" +
                                    "PME Events Wake Up,\n" +
                                    "Power On By Onboard LAN,\n" +
                                    "Power On By PCI Devices,\n" +
                                    "Power Up By Onboard LAN,\n" +
                                    "Resume by LAN,\n" +
                                    "Resume by PME# Function,\n" +
                                    "Resume By WOL,Resume on LAN,\n" +
                                    "Resume on LAN / PME#,\n" +
                                    "Resume on PCI Event,\n" +
                                    "Resume On PME#,\n" +
                                    "Wake on LAN from S5,\n" +
                                    "Wake On LAN,\n" +
                                    "Wake On PME,\n" +
                                    "Wake Up On LAN,\n" +
                                    "Wake Up On PME,\n" +
                                    "WakeUp by Onboard LAN,\n" +
                                    "Wakeup option,\n" +
                                    "WOL(PME#) From Soft-Off\n" +
                                    "Далее проверьте свойства сетевой карты, (правой кнопкой мыши на меню Пуск, выберите Диспетчер устройств).В Диспетчере устройств откройте свойства вашей Сетевой карты и выберите закладку Управление электропитанием. В этой вкладке должны быть включены следующие пункты: \n" +
                                    "Разрешать отключение этого устройства для экономии энергии, \n" +
                                    "Разрешать этому устройству выводить компьютер из ждущего режима, \n" +
                                    "Разрешать вывод компьютера из ждущего режима только с помощью магического пакета. \n" +
                                    "Некоторый сетевые карты поддерживают дополнительные настройки для включения компьютера (Свойство Wake on magic packet включить). По завершении настройки, выключите компьютер и убедитесь что на сетевой карты сзади компьютера горит индикатор(обычно зеленый светодиод) показывая что сетевая карта готова к приему пакета пробуждения.").ConfigureAwait(false);
                            }
                            break;
                         case  "/turnOFF":
                             System.Diagnostics.Process p = new System.Diagnostics.Process();
                             p.StartInfo.FileName = "cmd.exe";
                             p.StartInfo.Arguments = "/c shutdown -s -t 00";
                             p.Start();
                             AddToDatabase.AddToCommands(text1[0], name);
                             Application.Exit();
                             break;

                        default://основная работа с удалёнными методами происходит в этом блоке (кроме команд remotePC, turnOFF, reboot)

                            if (text1[0] == "/turnON")
                            {
                                try
                                {
                                    if (!WakeOnLan.ValidateMac(text1[2]))
                                        await botclient.SendTextMessageAsync(message.From.Id, "Неверный MAC адрес").ConfigureAwait(false);

                                    else
                                    {
                                        WakeOnLan.Up(text1[1], text1[2], GetPort(text1));
                                        await botclient.SendTextMessageAsync(message.From.Id, "Пакет отправлен").ConfigureAwait(false);

                                    }
                                }
                                catch (Exception ex)
                                {
                                    await botclient.SendTextMessageAsync(message.From.Id, $"Неверный формат ввода");
                                }
                            }
                            else if (text1[0] == "/getscreen")
                            {
                                Graphics graph = null;
                                try
                                {
                                    var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                    Screen.PrimaryScreen.Bounds.Height);

                                    graph = Graphics.FromImage(bmp);
                                    graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                                    bmp.Save("screen.png");
                                    using (FileStream stream = File.OpenRead("screen.png")) // Открываем поток для чтения файла(ов)
                                    {
                                        string ssf = Path.GetFileName("screen.png"); // Получаем имя файла
                                        var Iof = new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream, ssf); // Входные данные для отправки
                                        await botclient.SendPhotoAsync(message.From.Id, Iof); // Отправка файла с параметрами.
                                    }
                                    AddToDatabase.AddToCommands(text1[0], name);
                                }
                                catch (Exception ex)
                                {
                                    AddToDatabase.AddToError(ex.ToString(), name);
                                }
                            }
                            else if (text1[0] == "/getfile")//поиск файла по ПК и вывод найденного файла в бота
                            {
                                try
                                {
                                   
                                   try 
                                   {
                                        if (text1.Count() == 2)
                                        {
                                            Find("C:\\", text1[1]);
                                        }
                                        else if (text1.Count() == 3)
                                        {
                                            Find($"{text1[1]}", $"{text1[2]}");
                                        }
                                        else 
                                        {
                                            await botclient.SendTextMessageAsync(message.From.Id, $"Неверное кол-во аргументов. Используйте команду как описано в инструкции: /remotepc").ConfigureAwait(false);
                                            return;
                                        }
                                        await botclient.SendTextMessageAsync(message.From.Id, "Пожалуйста подождите. Идёт поиск файла...").ConfigureAwait(false);
                                   }
                                   catch(Exception ex) {AddToDatabase.AddToError(ex.Message, name);}

                                    if (ph != null && ph.Count > 1)
                                        await botclient.SendTextMessageAsync(message.From.Id, $"Найдено несколько совпадений").ConfigureAwait(false);
                                    else if (ph.Count == 1)
                                        await botclient.SendTextMessageAsync(message.From.Id, $"Найден файл {ph[0]}").ConfigureAwait(false);
                                    else
                                        await botclient.SendTextMessageAsync(message.From.Id, $"Файлов не найдено").ConfigureAwait(false);

                                    for (int n = 0; n < ph.Count; n++)
                                    {
                                        if (text1.Count() == 2)
                                        {
                                            using (FileStream stream = File.OpenRead(ph[n])) // Открываем поток для чтения файла(ов)
                                            {
                                                string ssf = Path.GetFileName(text1[1].ToString()); // Получаем имя файла
                                                var Iof = new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream, ssf); // Входные данные для отправки
                                                await botclient.SendDocumentAsync(message.From.Id, Iof); // Отправка файла с параметрами.
                                                n++;
                                            }
                                        }
                                        else if (text1.Count() == 3)
                                        {
                                            using (FileStream stream = File.OpenRead(ph[n])) // Открываем поток для чтения файла(ов)
                                            {
                                                string ssf = Path.GetFileName(text1[2].ToString()); // Получаем имя файла
                                                var Iof = new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream, ssf); // Входные данные для отправки
                                                await botclient.SendDocumentAsync(message.From.Id, Iof); // Отправка файла с параметрами.
                                                n++;
                                            }
                                        }
                                    
                                    }
                                    AddToDatabase.AddToCommands(text1[0], name);
                                }
                                catch (Exception ex)
                                {
                                    AddToDatabase.AddToError(ex.ToString(), name);
                                }
                            }
                            else if (text1[0] == "/run")//запуск URL/файла на компьютере
                            {
                                if (text1.Count() == 1)
                                {
                                    await botclient.SendTextMessageAsync(message.From.Id, $"Неверное кол-во аргументов. Используйте команду как в инструкции");
                                    return;
                                }
                                try
                                {
                                    System.Diagnostics.Process k = new System.Diagnostics.Process();
                                    k.StartInfo.FileName = text1[1];
                                    k.Start();
                                }
                                catch (Exception ex)
                                {
                                    await botclient.SendTextMessageAsync(message.From.Id, $"Не удаётся найти указанный файл или путь");
                                    AddToDatabase.AddToError(ex.ToString(), name);
                                }
                                AddToDatabase.AddToCommands(text1[0], name);
                            }
                            else if (text1[0] == "/addjoke")//добавление шутки в бд
                            {
                                if (text1.Count() == 1)
                                {
                                    await botclient.SendTextMessageAsync(message.From.Id, $"Напишите свою шутку");
                                    return;
                                }
                                try
                                {
                                    text = text.Remove(0, 9);
                                    AddToDatabase.AddToJokes(text);
                                    await botclient.SendTextMessageAsync(message.From.Id, $"Анекдот успешно добален в список");
                                }
                                catch (Exception ex)
                                {
                                    AddToDatabase.AddToError(ex.Message, name);
                                }
                            }
                            else if (text1[0] == "/weather") //вывод погоды по заданному городу
                            {
                                if (text1.Count() == 1)
                                {
                                    await botclient.SendTextMessageAsync(message.From.Id, $"Пожалуйста укажите название города");
                                    return;
                                }
                                try
                                {
                                    string url = "http://api.openweathermap.org/data/2.5/weather?q=" + text1[1] + "&units=metric&appid=5c63497e9d0e4b4a8f836f11c482f278";//берём данные их открытого источника OpenWeather, к сожалению этот сервис не предоставляет информацию по горорду Санкт-Петербург, Поэтому взял Москву
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
                                        text: $"Температура в городе {text1[1]}: {weatherResponse.Main.Temp}°С, Влажность {weatherResponse.Main.Humidity}%, Давление {weatherResponse.Main.Pressure * 0.75} мм рт. ст."
                                        ).ConfigureAwait(false);
                                }
                                catch
                                {
                                    await botclient.SendTextMessageAsync(message.From.Id, $"Данных по этму городу к сожалению не найдно");
                                }
                                AddToDatabase.AddToCommands(text1[0], name);
                            }
                            else
                            {
                                SpeechSynthesizer speechSynth = new SpeechSynthesizer(); //создаём объект

                                speechSynth.Volume = 50; //устанавливаем уровень звука

                                speechSynth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Adult); //также можно устанавливать пол и возвраст озвучки
                                speechSynth.Speak(text);
                            }

                            break;
                    }
                }
                else if (message.Type == MessageType.Voice)//добавляю реакцию на голосовые сообщения
                {
                    AddToDatabase.AddToMessages("Голосовое сообщение", name);
                    await botclient.SendTextMessageAsync(
                                chatId: e.Message.Chat,
                                text: $"Ненавижу голосовые сообщения >:["
                          ).ConfigureAwait(false);
                    Console.WriteLine($"Поступило голосовое сообщение из чата: '{e.Message.Chat.Id}' от пользователя: {name}");//проверка того что сообщения доходят до бота 
                }
                else
                    return;
            }
            catch(Exception ex) 
            {
                AddToDatabase.AddToError(ex.Message,"");
            }
            
        } 
    }
}
