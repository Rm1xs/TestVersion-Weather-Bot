using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WeatherNet;
using WeatherNet.Clients;

namespace TestBot
{
    class Program
    {
        static ITelegramBotClient botClient;

        public static void Main()
        {
            botClient = new TelegramBotClient("906660577:AAH_K8oEGMakY7kChFDfNG0M_SSke8zQ370");
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private const string FirstOptionText = "Kharkiv";
        private const string SecondOptionText = "Kiev";
        public static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");
                if (e.Message.Text == "/weather")
                {
                    string weburl = "http://api.openweathermap.org/data/2.5/weather?q=" + FirstOptionText + "&appid=0de8fd56cc26fb5bc873d8d2b33a3eb1" + "&mode=xml";
                    var xml = await new WebClient().DownloadStringTaskAsync(new Uri(weburl));
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    //wind
                    string szTemp4 = doc.DocumentElement.SelectSingleNode("wind").SelectSingleNode("speed").Attributes["value"].Value;
                    double conv2 = Convert.ToDouble(szTemp4, NumberFormatInfo.InvariantInfo);
                    //clouds
                    string szTemp2 = doc.DocumentElement.SelectSingleNode("clouds").Attributes["value"].Value;
                    double conv = Convert.ToDouble(szTemp2, NumberFormatInfo.InvariantInfo);
                    //tempereture
                    string szTemp = doc.DocumentElement.SelectSingleNode("temperature").Attributes["value"].Value;
                    double a = Convert.ToDouble(szTemp, NumberFormatInfo.InvariantInfo);
                    double temp = a - 273.16;
                    //time
                    string szTemp3 = doc.DocumentElement.SelectSingleNode("lastupdate").Attributes["value"].Value;
                    var upd = szTemp3.Replace("T", " ");
                    var res = "*" + temp.ToString("N2") + "*" + " °C in Kharkiv \n" + "Clouds: " + "*" + szTemp2 + "*" + "% \n" + "Wind speed: " + "*" + conv2 + "*" + " m/s" + "\n" + "Request time : " + DateTime.Now + "\n" + "Server update: " + "*" + upd + "*";
                    await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: res, parseMode: ParseMode.Markdown, disableNotification: true);
                    await botClient.SendPhotoAsync(chatId: e.Message.Chat,
                    photo: "https://meteopost.com/load/maps/o-006.png",
                    caption: "<i>Source</i>: <a href=\"https://meteopost.com/weather/maps\">Meteopost</a>",
                    parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Check site", "https://meteopost.com/")));
                }
                if (e.Message.Text == "/info")
                {
                    await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: "This bot show weather information!");
                }
                if (e.Message.Text != "/weather" && e.Message.Text != "/info")
                {
                    switch (e.Message.Text)
                    {
                        case FirstOptionText:
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, "You chose the option", replyMarkup: new ReplyKeyboardRemove());
                            break;
                        case SecondOptionText:
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, "You chose the Kiev option", replyMarkup: new ReplyKeyboardRemove());
                            string weburl = "http://api.openweathermap.org/data/2.5/weather?q=" + SecondOptionText + "&appid=0de8fd56cc26fb5bc873d8d2b33a3eb1" + "&mode=xml";
                            var xml = await new WebClient().DownloadStringTaskAsync(new Uri(weburl));
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(xml);
                            //wind
                            string szTemp4 = doc.DocumentElement.SelectSingleNode("wind").SelectSingleNode("speed").Attributes["value"].Value;
                            double conv2 = Convert.ToDouble(szTemp4, NumberFormatInfo.InvariantInfo);
                            //clouds
                            string szTemp2 = doc.DocumentElement.SelectSingleNode("clouds").Attributes["value"].Value;
                            double conv = Convert.ToDouble(szTemp2, NumberFormatInfo.InvariantInfo);
                            //tempereture
                            string szTemp = doc.DocumentElement.SelectSingleNode("temperature").Attributes["value"].Value;
                            double a = Convert.ToDouble(szTemp, NumberFormatInfo.InvariantInfo);
                            double temp = a - 273.16;
                            //time
                            string szTemp3 = doc.DocumentElement.SelectSingleNode("lastupdate").Attributes["value"].Value;
                            var upd = szTemp3.Replace("T", " ");
                            var res = "*" + temp.ToString("N2") + "*" + " °C in Kiev \n" + "Clouds: " + "*" + szTemp2 + "*" + "% \n" + "Wind speed: " + "*" + conv2 + "*" + " m/s" + "\n" + "Request time : " + DateTime.Now + "\n" + "Server update: " + "*" + upd + "*";

                            await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: res, parseMode: ParseMode.Markdown, disableNotification: true);
                            await botClient.SendPhotoAsync(chatId: e.Message.Chat,
                            photo: "https://meteopost.com/load/maps/t-006.png",
                            caption: "<i>Source</i>: <a href=\"https://meteopost.com/weather/maps\">Meteopost</a>",
                            parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Check site", "https://meteopost.com/")));
                            break;

                        default:
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, "Hi, select city!",
                                replyMarkup: new ReplyKeyboardMarkup(new[]
                                {
                    new KeyboardButton(FirstOptionText),
                    new KeyboardButton(SecondOptionText),
                                }));
                            break;
                    }
                }
            }
        }
    }

    [Serializable, XmlRoot("Current")]
    public class Current
    {
        public City city { get; set; }
        public Temperature temperature { get; set; }

    }
    [XmlRoot(ElementName = "city")]
    public class City
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("id")]
        public int id { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public Sun sun { get; set; }
    }
    [XmlRoot(ElementName = "coor")]
    public class Coord
    {
        [XmlAttribute("lat")]
        public double lat { get; set; }
        [XmlAttribute("lon")]
        public double lon { get; set; }
    }
    [XmlRoot(ElementName = "sun")]
    public class Sun
    {
        [XmlAttribute("set")]
        public DateTime set { get; set; }
        [XmlAttribute("rise")]
        public DateTime rise { get; set; }
    }
    [XmlRoot(ElementName = "temperature")]
    public class Temperature
    {
        [XmlAttribute("value")]
        public double value { get; set; }
        [XmlAttribute("unit")]
        public string unit { get; set; }
        [XmlAttribute("max")]
        public double max { get; set; }
        [XmlAttribute("min")]
        public double min { get; set; }
    }
}
