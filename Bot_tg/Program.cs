using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using System.Timers;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Bot_tg
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Program
    {
        static string NewTimeTable_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tb.xls");
        static string OldimeTable_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "t6.xls");
        public static Information info = new Information();
        public static bool ok = true;
        static string[] Curs = { "Выберите свой курс", "Выберите cвою ОП", "выберите свою группу", "Отлично! Теперь жди от нас сообщений,студент." };
        static string path = NewTimeTable_path;
        static TelegramBotClient botClient = new TelegramBotClient("5120683940:AAFLiyPv2qDWMFiiLSHy99Xq5GuYmmD7L0I");
        static Dictionary<long, UserStates> clientStates = new Dictionary<long, UserStates>();
        static ReplyKeyboardMarkup StartMenu = new(new[]
            {
            new KeyboardButton[] { "1 курс", "2 курс" },
            new KeyboardButton[] { "3 курс", "4 курс" },
            })
        {
            ResizeKeyboard = true
        };

        public static ReplyKeyboardMarkup Menu(UserStates _UserStates, Information _info) //создание меню на основе выбранных ранее параметрах
        {
            if (_UserStates.PE != null) //проверка выбрал ли пользователь ОП
            {
                if (_UserStates.Group != null) // проверка выбрал ли пользователь группу
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] //создаем меню из 2 кнопок
    {
        new KeyboardButton[] { "Выслать актуальное расписание" },
        new KeyboardButton[]{"back"}
    })
                    {
                        ResizeKeyboard = true
                    };
                    return replyKeyboardMarkup;//возвращаем ее
                }
                else // если пользователь выбрал ОП и курс, но не выбрал группу
                {
                    var i = _info.Course[_UserStates.Course][_UserStates.PE].Keys; //получаем спискок групп в заданной ОП
                    string[] button = new string[i.Count]; //создаем массив из групп
                    i.CopyTo(button, 0); //копируем группы в массив
                    string[] DownButtons = new string[i.Count / 2]; //создаем массив из текста нижних кнопок меню
                    string[] UpButtons = new string[i.Count - DownButtons.Length]; //создаем массив из текста верних кнопок меню
                    Array.Copy(button, UpButtons, i.Count - DownButtons.Length); //переносим  в массив верхних кнопок значения первой половины строк
                    Array.Copy(button, i.Count - DownButtons.Length, DownButtons, 0, DownButtons.Length);//переносим  в массив нижних кнопок значения второй половины строк
                    KeyboardButton[] Down = new KeyboardButton[DownButtons.Length + 1]; //создаем нижние кнопки 
                    KeyboardButton[] Up = new KeyboardButton[UpButtons.Length];//создаем верхние кнопки 
                    for (int s = 0; s < DownButtons.Length; s++) //переносим текст нижних кнопок в кнопки
                    {
                        Down[s] = DownButtons[s];
                    }
                    Down[Down.Length - 1] = new KeyboardButton("back"); //добавляем в угол нижнего меню кнопку назад
                    for (int s = 0; s < UpButtons.Length; s++)//переносим текст верхних кнопок в кнопки
                    {
                        Up[s] = UpButtons[s];
                    }

                    ReplyKeyboardMarkup replyMarkup = new(new[] //создаем меню из вернхних и нижних рядов кнопок
                    {
                        Up,
                        Down,
                    })
                    {
                        ResizeKeyboard = true
                    };
                    return replyMarkup; //возвращаем меню
                }
            }
            else //если у пользователя не выбрана ОП, но выбран курс
            {
                var i = _info.Course[_UserStates.Course].Keys;//получаем спискок ОП в заданном курсе
                string[] button = new string[i.Count]; //создаем массив из Оп
                i.CopyTo(button, 0);//копируем ОП в массив
                string[] DownButtons = new string[i.Count / 2];//создаем массив из текста нижних кнопок меню
                string[] UpButtons = new string[i.Count - DownButtons.Length];//создаем массив из текста верних кнопок меню
                Array.Copy(button, DownButtons, i.Count / 2);//переносим  в массив верхних кнопок значения первой половины строк
                Array.Copy(button, i.Count / 2, UpButtons, 0, UpButtons.Length);//переносим  в массив нижних кнопок значения второй половины строк
                KeyboardButton[] Down = new KeyboardButton[DownButtons.Length + 1];//создаем нижние кнопки 
                KeyboardButton[] Up = new KeyboardButton[UpButtons.Length];//создаем верхние кнопки 
                for (int s = 0; s < DownButtons.Length; s++)//переносим текст нижних кнопок в кнопки
                {
                    Down[s] = DownButtons[s];
                }
                Down[Down.Length - 1] = new KeyboardButton("back");//добавляем в угол нижнего меню кнопку назад
                for (int s = 0; s < UpButtons.Length; s++)//переносим текст верхних кнопок в кнопки
                {
                    Up[s] = UpButtons[s];
                }

                ReplyKeyboardMarkup replyMarkup = new(new[] //создаем меню из вернхних и нижних рядов кнопок
                {
                        Up,
                        Down,
                    })
                {
                    ResizeKeyboard = true
                };
                return replyMarkup; //возвращаем меню
            }
        }
        public static void MakeJson(ref Information _info, string path)
        {
            Excel curr = new Excel(path, 1);
            curr.Close();
            int[] course = { 1, 2, 4 };
            int index = 1;
            foreach (int i in course)
            {
                curr = new Excel(path, i);
                _info.AddCours(index);
                foreach (var PE in curr.CreatePrograms())
                {
                    _info.AddEducationProgram(index, PE);
                    foreach (var _groups in curr.CreateGroups(PE))
                        _info.AddGroup(index, PE, _groups);
                }
                curr.Close();
                index++;
            }
        }
        static ReplyKeyboardMarkup Klava(int a)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
            new KeyboardButton[] { "1 курс", "2 курс" },
            new KeyboardButton[] { "3 курс", "4 курс" },
            })
            {
                ResizeKeyboard = true
            };
            switch (a)
            {
                case 0:
                    replyKeyboardMarkup = new(new[]
            {
            new KeyboardButton[] { "1 курс", "2 курс" },
            new KeyboardButton[] { "3 курс", "4 курс" },
            })
                    {
                        ResizeKeyboard = true
                    };
                    break;
                case 1:
                    replyKeyboardMarkup = new(new[]
         {
            new KeyboardButton[] { "ПИ", "БИ","И","Ю" },
            new KeyboardButton[] { "Э", "УБ","ИЯ","back" },
            })
                    {
                        ResizeKeyboard = true
                    };
                    break;
                case 2:
                    replyKeyboardMarkup = new(new[]
           {
            new KeyboardButton[] { "21-3", "9" },
            new KeyboardButton[] { "10", "back" },
            })
                    {
                        ResizeKeyboard = true
                    };
                    break;
                case 3:
                    replyKeyboardMarkup = new(new[]
          {
            new KeyboardButton[] { "Выслать актуальное расписание" },
            new KeyboardButton[] { "Изменить свой поток" },
            })
                    {
                        ResizeKeyboard = true
                    };
                    break;
            }
            return replyKeyboardMarkup;
        }
        static async Task Main(string[] args)
        {
            try
            {


                using var cts = new CancellationTokenSource();
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { } // receive all update types
                };
                botClient.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken: cts.Token);
                var me = await botClient.GetMeAsync();
                Console.WriteLine($"Start listening for @{me.Username}");
                if (ok)
                {
                    string fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "position.json");
                    string jsonString = System.IO.File.ReadAllText(fileName);
                    if (jsonString.Length > 0)
                        clientStates = JsonSerializer.Deserialize<Dictionary<long, UserStates>>(jsonString)!;
                    fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "info.json");
                    jsonString = System.IO.File.ReadAllText(fileName);
                    if (jsonString.Length > 0)
                        info = JsonSerializer.Deserialize<Information>(jsonString)!;
                    else
                        MakeJson(ref info, path);
                    ok = false;
                }
                System.Timers.Timer test = new System.Timers.Timer(3600000);
                //test.Elapsed += new ElapsedEventHandler(DowloandTimeTable);
                test.Start();
                Console.ReadLine();

                // Send cancellation request to stop bot
                cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        public static async void SendToStudentsAsync()
        {
            bool correct = false;
            string OP;
            Regex remove = new Regex(@"\-[0-9]*");
            Excel _new1 = null;
            Excel _old1 = null;
            do
            {
                try
                {
                    _new1 = new Excel(NewTimeTable_path, 1);
                    _old1 = new Excel(OldimeTable_path, 1);
                    correct = true;
                }
                catch
                {
                    _new1?.Close();
                    _old1?.Close();
                    Thread.Sleep(300);
                }
            } while (!correct);
            correct = false;
            List<string> deff1 = Excel.CompareTimeTable(_new1.CreateTable(), _old1.CreateTable());
            _new1.Close();
            _old1.Close();
            foreach (var deff in deff1)
            {
                OP = remove.Replace(deff, "");
                foreach (long id in info.Course[1][OP][deff])
                {
                    using Stream stream = System.IO.File.OpenRead(NewTimeTable_path);
                    await botClient.SendDocumentAsync(
                        chatId: id,
                        document: new InputOnlineFile(content: stream, fileName: "NewTimeTable.xls"),
                        caption: "NewTimeTable" + deff);
                    stream.Close();
                }
            }
            correct = false;
            Excel _new2 = null;
            Excel _old2 = null;
            do
            {
                try
                {
                    _new2 = new Excel(NewTimeTable_path, 2);
                    _old2 = new Excel(OldimeTable_path, 2);
                    correct = true;
                }
                catch
                {
                    _new2?.Close();
                    _old2?.Close();
                    Thread.Sleep(300);
                }
            } while (!correct);
            List<string> deff2 = Excel.CompareTimeTable(_new2.CreateTable(), _old2.CreateTable());
            _new2.Close();
            _old2.Close();

            foreach (var deff in deff2)
            {
                OP = remove.Replace(deff, "");
                foreach (long id in info.Course[2][OP][deff])
                {
                    using Stream stream = System.IO.File.OpenRead(NewTimeTable_path);
                    await botClient.SendDocumentAsync(
                        chatId: id,
                        document: new InputOnlineFile(content: stream, fileName: "NewTimeTable.xls"),
                        caption: "NewTimeTable" + deff);
                    stream.Close();
                }
            }
            correct = false;
            Excel _new3 = null;
            Excel _old3 = null;
            do
            {
                try
                {
                    _new3 = new Excel(NewTimeTable_path, 4);
                    _old3 = new Excel(OldimeTable_path, 4);
                    correct = true;
                }
                catch
                {
                    _new3?.Close();
                    _old3?.Close();
                    Thread.Sleep(300);
                }
            } while (!correct);
            correct = false;
            List<string> deff3 = Excel.CompareTimeTable(_new3.CreateTable(), _old3.CreateTable());
            _old3.Close();
            _new3.Close();
            foreach (var deff in deff3)
            {
                OP = remove.Replace(deff, "");
                foreach (long id in info.Course[3][OP][deff])
                {
                    using Stream stream = System.IO.File.OpenRead(NewTimeTable_path);
                    await botClient.SendDocumentAsync(
                        chatId: id,
                        document: new InputOnlineFile(content: stream, fileName: "NewTimeTable.xls"),
                        caption: "NewTimeTable" + deff);
                    stream.Close();
                }
            }
            if (deff1.Count + deff2.Count + deff3.Count > 0)
            {
                do
                {
                    try
                    {
                        string line = "";
                        WebClient Br = new WebClient();
                        var link = $"href=\"(.*?)\" class=\"link mceDataFile\">Расписание занятий \\(неделя (.*?) c {GetMondey()}\\)(.*?)</a>";
                        line = Br.DownloadString("http://students.perm.hse.ru/timetable/");
                        Match match = Regex.Match(line, link);
                        string LinkToTable = $"http:{match.Groups[1].Value}";
                        if (!LinkToTable.Contains(@"http://www.hse.ru/")) LinkToTable = LinkToTable.Replace("http:/", @"http://www.hse.ru/");
                        Br.DownloadFile(LinkToTable, OldimeTable_path);
                        correct = true;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(300);

                    }
                } while (!correct);

            }
        }

        public static string GetMondey()
        {
            DateTime date = DateTime.Now;
            date = date.AddHours(2);
            while (date.DayOfWeek != DayOfWeek.Monday)
                if (date.DayOfWeek == DayOfWeek.Sunday)
                    date = date.AddDays(1);
                else date = date.AddDays(-1);
            return date.ToShortDateString();
        }

        private static async void DowloandTimeTable(object sender, ElapsedEventArgs ev)
        {
            bool okey = false;
            do
            {
                try
                {
                    string line = "";
                    WebClient Br = new WebClient();
                    var link = $"href=\"(.*?)\" class=\"link mceDataFile\">Расписание занятий \\(неделя (.*?) c {GetMondey()}\\)(.*?)</a>";
                    line = Br.DownloadString("http://students.perm.hse.ru/timetable/");
                    Match match = Regex.Match(line, link);
                    //Console.WriteLine($"{link}");
                    //Console.WriteLine($"http:{match.Groups[1].Value}");
                    string LinkToTable = $"http:{match.Groups[1].Value}";
                    if (!LinkToTable.Contains(@"http://www.hse.ru/")) LinkToTable = LinkToTable.Replace("http:/", @"http://www.hse.ru/");
                    Br.DownloadFile(LinkToTable, NewTimeTable_path);
                    Console.WriteLine("Прошло 1000 тиков");
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                        Br.DownloadFile(LinkToTable, OldimeTable_path);
                    SendToStudentsAsync();
                    okey = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Файл занят");

                }
            } while (!okey);
        }
        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;
            if (update.Message!.Type != MessageType.Text)
                return;
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            var state = clientStates.ContainsKey(chatId) ? clientStates[chatId] : null;
            if (state == null)
            {
                switch (messageText)
                {
                    case "1 курс":
                        state = new UserStates { Course = 1 };
                        clientStates.Add(chatId, state);
                        await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Curs[1],
                    replyMarkup: Menu(state, info),
                    cancellationToken: cancellationToken);
                        break;
                    case "2 курс":
                        state = new UserStates { Course = 2 };
                        clientStates.Add(chatId, state);
                        await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Curs[1],
                    replyMarkup: Menu(state, info),
                    cancellationToken: cancellationToken);
                        break;
                    case "3 курс":
                        state = new UserStates { Course = 3 };
                        clientStates.Add(chatId, state);
                        await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Curs[1],
                    replyMarkup: Menu(state, info),
                    cancellationToken: cancellationToken);
                        break;
                    //case "4 курс":
                    //    state = new UserStates { Cours = 4 };
                    //    clientStates.Add(chatId, state);
                    //    await botClient.SendTextMessageAsync(
                    //chatId: chatId,
                    //text: Curs[0],
                    //replyMarkup: Menu(state, info),
                    //cancellationToken: cancellationToken);

                    //    break;
                    default:
                        await botClient.SendTextMessageAsync(
                   chatId: chatId,
                   text: Curs[0],
                   replyMarkup: Klava(0),
                   cancellationToken: cancellationToken);
                        break;
                }
            }
            else
            {
                if (state.Course != 0 && state.PE == null)
                {
                    if (messageText == "back")
                    {
                        state = null;
                        clientStates.Remove(chatId);
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: Curs[0],
                            replyMarkup: Klava(0),
                            cancellationToken: cancellationToken);
                    }
                    else if (info.Course[state.Course].Keys.Contains(messageText))
                    {
                        state.PE = messageText;
                        await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Curs[2],
                    replyMarkup: Menu(state, info),
                    cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Curs[1],
                    replyMarkup: Menu(state, info),
                    cancellationToken: cancellationToken);
                    }
                }
                else if (state.Course != 0 && state.PE != null && state.Group == null)
                {
                    if (messageText == "back")
                    {
                        state.PE = null;
                        await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Curs[1],
                    replyMarkup: Menu(state, info),
                    cancellationToken: cancellationToken);
                    }
                    else if (info.Course[state.Course][state.PE].Keys.Contains(messageText))
                    {
                        state.Group = messageText;
                        await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Curs[3],
                    replyMarkup: Menu(state, info),
                    cancellationToken: cancellationToken);
                        info.Course[state.Course][state.PE][state.Group].Add(chatId);
                        string fileName = "info.json";
                        using FileStream createStream = System.IO.File.Create(fileName);
                        await JsonSerializer.SerializeAsync(createStream, info);
                        await createStream.DisposeAsync();
                        var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic) };
                        string jsonString = JsonSerializer.Serialize(info, options);
                        Console.WriteLine(jsonString);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Curs[2],
                    replyMarkup: Menu(state, info),
                    cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    if (messageText == "back")
                    {
                        info.Course[state.Course][state.PE][state.Group].Remove(chatId);
                        state.Group = null;
                        await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Curs[2],
                    replyMarkup: Menu(state, info),
                    cancellationToken: cancellationToken);
                        string fileName = "info.json";
                        using FileStream createStream = System.IO.File.Create(fileName);
                        await JsonSerializer.SerializeAsync(createStream, info);
                        await createStream.DisposeAsync();
                        var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic) };
                        string jsonString = JsonSerializer.Serialize(info, options);
                        Console.WriteLine(jsonString);
                    }
                    if (messageText == "Выслать актуальное расписание")
                    {
                        bool correct = false;
                        do
                        {
                            try
                            {
                                using Stream stream = System.IO.File.OpenRead(OldimeTable_path);
                                await botClient.SendDocumentAsync(
                                   chatId: chatId,
                                   document: new InputOnlineFile(content: stream, fileName: "NewTimeTable.xls"),
                                   caption: "Вот актуальное насписание, студент!");
                                stream.Close();
                                correct = true;
                            }
                            catch
                            {
                                Thread.Sleep(300);
                            }
                        } while (!correct);

                    }
                }
            }
            string position = "position.json";
            using FileStream createStream1 = System.IO.File.Create(position);
            await JsonSerializer.SerializeAsync(createStream1, clientStates);
            await createStream1.DisposeAsync();
            var options1 = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic) };
            string jsonString1 = JsonSerializer.Serialize(clientStates, options1);
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
        }

    }
}
