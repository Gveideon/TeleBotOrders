using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Text.Json.Serialization;
using System.Diagnostics;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBotOrders
{
    internal class TeleBot
    {
        enum TypeHandler 
        {
            None,
            RegistrName,
            RegistrPhone,
            OrderBegin,
            OrderChooseMenu,
            ChooseCafe,
        }
        public string Token { get; set; }
        private ITelegramBotClient _bot;
        private CancellationTokenSource _cts;
        private User _user;
        private User _initUser;
        private List<User> _coopUsers;
        private long _id = -1;
        private TypeHandler _typeHandler = TypeHandler.None;
        private Order _currentOrder;
        private List<Cafe> _currentCafes;
        private List<long> _usersIds;

        public TeleBot() 
        {
            Token = "5773151578:AAH2GEcv7Ey3LKcnM_Z0lJpmH0NiXf1Dttk";
            _bot = new TelegramBotClient(Token);
            _cts = new CancellationTokenSource();
            _usersIds = new List<long>();
        }
        public void Start() 
        {
            Console.WriteLine("Запущен бот " + _bot.GetMeAsync().Result.FirstName);
            Debug.WriteLine("Запущен бот " + _bot.GetMeAsync().Result.FirstName);
            var cancellationToken = _cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            _bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            _usersIds = DBController.GetAllUsersId().ToList();
        }
        public void Stop()
        {
            _cts.Cancel();
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Message != null)
            {
                _id = update.Message.Chat.Id;
            }
            if (update.CallbackQuery != null)
            {
                var call = update.CallbackQuery;
                var message = call.Message;
                var chatId = message.Chat.Id;
                Debug.WriteLine(call.From.Id);
                Debug.WriteLine(message.Text);
                if(_typeHandler == TypeHandler.ChooseCafe)
                {
                    if (_currentCafes == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "простите ничего не нашел");
                        return;
                    }
                    await botClient.SendTextMessageAsync(message.Chat, "Ищу меню ");
                    var text = "";
                    var index = 1;
                    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                    var indexMenu = Convert.ToInt32(call.Data);
                    var dishes = DBController.GetDishes(_currentCafes[indexMenu - 1].Menu.Id);
                    foreach (var dish in dishes)
                    {
                        text = $"{index}) {dish.Name} price: {dish.Price}, discount: {dish.Discount} \n\r Description {dish.Description}";
                        await botClient.SendTextMessageAsync(message.Chat, text);
                        var photo = await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: $"{dish.PathImage}",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                    }
                    return;
                }
                if (call.Data == "registr_yes")
                {
                    _user = new User();
                    _typeHandler = TypeHandler.RegistrName;
                    _user.Id = _id;
                    var sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Введите свое имя",
                        cancellationToken: cancellationToken);
                    await _bot.EditMessageReplyMarkupAsync(call.Message.Chat.Id, call.Message.MessageId);
                    return;
                }
                if (call.Data == "registr_no")
                {
                    var sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Теперь ты аноним!)",
                        cancellationToken: cancellationToken);
                    await _bot.EditMessageReplyMarkupAsync(call.Message.Chat.Id, call.Message.MessageId);
                    _user = new User { Id = _id, Name = "Anonim", PhoneNumber = "0"  };
                    if (DBController.AddNewUser(_user))
                        await botClient.SendTextMessageAsync(message.Chat, "спасибо за регистрацию, я вас запомнил!)");
                    else
                        await botClient.SendTextMessageAsync(message.Chat, "что то пошло не так !(");
                    return;
                }
                //if (_typeHandler == TypeHandler.OrderBegin)
                //{
                    if(_initUser.Id != chatId && call.Data == "join_to_order_yes")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Вы присоеденились к заказу");
                        _coopUsers.Add(DBController.FindUserByIndex(chatId));
                        await botClient.SendTextMessageAsync(message.Chat, "Подождите пока инициатор выберет кафе!)");
                    return;
                    }
                    
                //}
                if(_typeHandler == TypeHandler.OrderChooseMenu)
                {
                    if (_initUser.Id == chatId)
                    {
                        foreach (var id in _usersIds)
                        {
                            if (_currentCafes == null)
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "простите ничего не нашел");
                                return;
                            }
                            await botClient.SendTextMessageAsync(message.Chat, "Ищу меню ");
                            var text = "";
                            var index = 1;
                            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                            var indexMenu = Convert.ToInt32(call.Data);
                            var dishes = DBController.GetDishes(_currentCafes[indexMenu - 1].Menu.Id);
                            foreach (var dish in dishes)
                            {
                                text = $"{index}) {dish.Name} price: {dish.Price}, discount: {dish.Discount} \n\r Description {dish.Description}";
                                await botClient.SendTextMessageAsync(message.Chat, text);
                                var photo = await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: $"{dish.PathImage}",
                                parseMode: ParseMode.Html,
                                cancellationToken: cancellationToken);
                            }
                            return;
                        }
                    }
                }
            }

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message != null)
            {
                var message = update.Message;
                var chatId = message.Chat.Id;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать в бот. Здесь можно кооперироваться с колеггами по работе и совершать общий заказ еды!_)");
                    InlineKeyboardMarkup inlineKeyboard = new(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "Да", callbackData: "registr_yes"),
                            InlineKeyboardButton.WithCallbackData(text: "Нет", callbackData: "registr_no"),
                        },
                    });

                    var sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Хотите зарегестрироваться?",
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);

                    return;
                }
                if (message.Text.ToLower() == "/help")
                {
                    await botClient.SendTextMessageAsync(message.Chat,
@"Команда /start инициализирует начало работы с ботом.
Команда /help вызывает справку \n\r
Команда /reg инициализирует регестрацию 
Команда /edit_profile позволяет изменить профиль 
Команда /order создает новый заказ 
Команда /cafe показывает доступные для заказа кафе и рестораны
Команда /menu показывает меню открытого кафе
Команда /cancel_order отменяет заказ
Команда /cancel_init вы перестаете быть инициатором заказа 
Команда /assign_init  назначить кого-то инициатором");
                    return;
                }
                if (message.Text.ToLower() == "/reg")
                {
                    _user = new User();
                    _typeHandler = TypeHandler.RegistrName;
                    _user.Id = _id;
                    await botClient.SendTextMessageAsync(message.Chat, "Введите свое имя");
                    return;
                }
                if (message.Text.ToLower() == "/edit_profile") 
                { 
                    await botClient.SendTextMessageAsync(message.Chat, "в разработке");
                }
                if (message.Text.ToLower() == "/order")
                {
                    _typeHandler = TypeHandler.OrderBegin;
                    _coopUsers = new List<User>();
                    await botClient.SendTextMessageAsync(message.Chat, "начинается инициализация заказа");
                    //if(_user == null)
                        _initUser = DBController.FindUserByIndex(chatId);
                        _initUser.IsInit = true;
                    //else
                        /* нужно продумать момент с несколькими заказами */
                    _currentOrder = new Order {Name =$"заказ {DBController.CountOrders()}"};
                    InlineKeyboardMarkup inlineKeyboard = new(new[]
                   {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "Да", callbackData: "join_to_order_yes"),
                            InlineKeyboardButton.WithCallbackData(text: "Нет", callbackData: "join_to_order_no"),
                        },
                    });
                    foreach (var id in _usersIds)
                    {
                        if (id != chatId)
                        {
                            var sentMessage = await botClient.SendTextMessageAsync(
                            chatId: id,
                            text: $"Пользователь {_initUser.Name} инициировал заказ, хотите присоединиться?",
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
                        }
                    }
                    _typeHandler = TypeHandler.OrderChooseMenu;
                    await botClient.SendTextMessageAsync(message.Chat, "Ищу кафе");
                    var text = "";
                    var index = 1;
                    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                    _currentCafes = DBController.GetAllCafes().ToList();
                    foreach (var cafe in _currentCafes)
                    {
                        text += $"{index}) {cafe.Name} \n\r";
                        buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"{index}", callbackData: $"{index}"));
                        index++;
                    }
                     inlineKeyboard = new(new[]
                    {
                        buttons.ToArray()
                    });
                    await botClient.SendTextMessageAsync(chatId: chatId,
                        text: text,
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
                
                return;
                }
                if (message.Text.ToLower() == "/cafe")
                {
                    _typeHandler = TypeHandler.ChooseCafe;
                    await botClient.SendTextMessageAsync(message.Chat, "Ищу кафе");
                    var text = "";
                    var index = 1;
                    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                    _currentCafes = DBController.GetAllCafes().ToList();
                    foreach (var cafe in _currentCafes)
                    {
                        text += $"{index}) {cafe.Name} \n\r";
                        buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"{index}", callbackData: $"{index}"));
                        index++;
                    }
                    InlineKeyboardMarkup inlineKeyboard = new(new[]
                    {
                        buttons.ToArray()
                    });
                    await botClient.SendTextMessageAsync(chatId: chatId,
                        text:text,
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
                    return;
                }
                if (message.Text.ToLower() == "/menu")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "в разработке");
                }
                if (message.Text.ToLower() == "/cancel_order")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "в разработке");
                }
                if (message.Text.ToLower() == "/cancel_init")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "в разработке");
                }
                if (message.Text.ToLower() == "/assign_init")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "в разработке");
                }
                switch (_typeHandler)
                {
                    case TypeHandler.RegistrName:
                        {
                            _user.Name = message.Text;
                            _typeHandler = TypeHandler.RegistrPhone;
                            await botClient.SendTextMessageAsync(message.Chat, "спасибо, а теперь пожалуйста введите свой номер!)");
                            break;
                        }
                    case TypeHandler.RegistrPhone:
                        {
                            _user.PhoneNumber = message.Text;
                            _typeHandler = TypeHandler.None;
                            if(DBController.AddNewUser(_user))
                                await botClient.SendTextMessageAsync(message.Chat, "спасибо за регистрацию, я вас запомнил!)");
                            else
                                await botClient.SendTextMessageAsync(message.Chat,"что то пошло не так !(");
                            break;
                        }
                    default:
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Простите, но я вас не понимаю");
                            break;
                        }
                }

            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ex = JsonSerializer.Serialize(exception);
            Console.WriteLine(ex);
            Debug.WriteLine(ex);
        }

    }
}
