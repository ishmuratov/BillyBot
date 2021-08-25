using PictureSearchExample;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleTelegaBot
{
    class Program
    {
        private static readonly string APIKey = FileWorker.ReadFromTXTFile("key.txt");
        private static readonly TelegramBotClient Bot = new TelegramBotClient(APIKey);
        private static readonly BashORG bash = new BashORG();
        private static bool needPicture;
        private static ResponseCache DBCache;

        static void Main(string[] args)
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnMessageEdited += Bot_OnMessage;
            LoadCache();
            Bot.StartReceiving();
            Console.WriteLine("Bot is running...");
            Console.WriteLine($"Started: {DateTime.Now}");
            Console.WriteLine("Press <ENTER> for exit.");
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void LoadCache()
        {
            DBCache = (ResponseCache)FileWorker.LoadFromFile("cache.dat");
            if (DBCache == null)
            {
                DBCache = new ResponseCache();
            }
        }

        private static string GetPictureLink(string word)
        {
            Random rnd = new Random();
            List<string> answer = GetAnswerFromCache(word);
            if (answer.Count == 0)
            {
                var request = WebRequest.Create($"https://yandex.by/images/search?text={word}");
                using (var responses = request.GetResponse())
                {
                    using (var streams = responses.GetResponseStream())
                    using (var readers = new StreamReader(streams))
                    {
                        //в переменной html наш сайт
                        string html = readers.ReadToEnd();
                        string[] rowAnswer = html.Split(',');
                        foreach (string anyLine in rowAnswer)
                        {
                            if (anyLine.Contains(".jpg\""))
                            {
                                string imageLink = getBetween(anyLine, "http", ".jpg");
                                answer.Add(imageLink);
                            }
                        }
                    }
                }
                if (answer.Count > 0)
                {
                    SaveInCache(word, answer);
                }
            }
            if (answer.Count > 0)
            {
                return answer[rnd.Next(answer.Count)];
            }
            else
            {
                return "Yandex заблокировал запрос!";
            }
        }

        private static List<string> GetAnswerFromCache(string _name)
        {
            YandexImageResponse myResponse = DBCache.FindCacheOReturnNull(_name);
            if (myResponse != null)
            {
                return myResponse.ImageLinks;
            }
            else
            {
                return new List<string>();
            }
        }

        private static void SaveInCache(string _name, List<string> _answer)
        {
            YandexImageResponse newResponse = new YandexImageResponse(_name, _answer);
            DBCache.CacheData.Add(newResponse);
            FileWorker.SaveToFile(DBCache, "cache.dat");
        }

        private static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart);
                End = strSource.IndexOf(strEnd) + strEnd.Length;
                return strSource.Substring(Start, End - Start).Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (e.Message.Text.ToLower() == "picture" && !needPicture)
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Введите слово и я найду картинку!");
                    needPicture = true;
                }
                else if (e.Message.Text.ToLower() == "меню")
                {
                    needPicture = false;
                    ReturnMainMenu(e);
                }
                else if (e.Message.Text.ToLower() == "cryptocoins")
                {
                    ReturnCryptoCoinsButtons(e);
                }
                else if (e.Message.Text.ToLower() == "bitcoin")
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Bitcoin price: {CryptoCoins.GetPrice("bitcoin")}");
                }
                else if (e.Message.Text.ToLower() == "ethereum")
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Ethereum price: {CryptoCoins.GetPrice("ethereum")}");
                }
                else if (e.Message.Text.ToLower() == "litecoin")
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Litecoin price: {CryptoCoins.GetPrice("litecoin")}");
                }
                else if (e.Message.Text.ToLower() == "get cache")
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, ReturnCache());
                }
                else if (e.Message.Text == "Выслать цитату Bash.im")
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, bash.GetRandom());
                }
                else
                {
                    if (needPicture)
                    {
                        ReturnPicture(e);
                    }
                    else
                    {
                        ReturnMainMenu(e);
                    }
                }
            }
        }

        private static void ReturnMainMenu(MessageEventArgs e)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Выслать цитату Bash.im" },
                        new KeyboardButton[] { "CryptoCoins"},
                        new KeyboardButton[] { "Picture" }
                    },
                        resizeKeyboard: true
                    );

            Bot.SendTextMessageAsync(chatId: e.Message.Chat.Id, text: "Нажмите кнопку:", replyMarkup: replyKeyboardMarkup);
            needPicture = false;
        }

        private static void ReturnCryptoCoinsButtons(MessageEventArgs e)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Bitcoin" },
                        new KeyboardButton[] { "Ethereum" },
                        new KeyboardButton[] { "LiteCoin" },
                        new KeyboardButton[] { "Меню" }
                    },
                    resizeKeyboard: true
                );

            Bot.SendTextMessageAsync(chatId: e.Message.Chat.Id, text: "Choose coin:", replyMarkup: replyKeyboardMarkup);
        }

        private static void ReturnPicture(MessageEventArgs e)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                        new KeyboardButton[][]
                        {
                            new KeyboardButton[] { "Меню" }
                        },
                            resizeKeyboard: true
                        );

            Bot.SendTextMessageAsync(chatId: e.Message.Chat.Id, text: "А вот и картинка:", replyMarkup: replyKeyboardMarkup);
            if (CheckRequest(e.Message.Text))
            {
                string link = GetPictureLink(e.Message.Text.ToLower());
                if (link.Contains("http"))
                    Bot.SendPhotoAsync(e.Message.Chat.Id, photo: link);
                else
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, link);
            }
            else
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Слишком большой запрос! Ужмитесь... :)");
        }

        private static string ReturnCache()
        {
            if (DBCache == null)
                return "Кэш не создан!";
            if (DBCache.CacheData.Count == 0)
                return "В кэше ничего нет!";
            else
            {
                string answer = "";
                foreach (YandexImageResponse anyResponse in DBCache.CacheData)
                {
                    answer += anyResponse.Name + Environment.NewLine;
                }
                return answer;
            }
        }

        private static bool CheckRequest(string _word)
        {
            if (_word.Length == 30)
                return false;
            else
                return true;
        }
    }
}
