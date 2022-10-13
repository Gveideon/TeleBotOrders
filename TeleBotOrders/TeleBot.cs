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
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

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
            OrderInMenu,
            OrderCancel,
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
        private Dictionary<long, Order> _currentOrders;
        private List<Cafe> _currentCafes;
        private Dictionary<long, List<Dish>> dishes;
        private List<long> _usersIds;
        private int _currentDishIndex = 0;
        private int _indexCafe = 1;
        private int _currentDishesLength;
        private int _countUserInOrder = 0;

        public TeleBot() 
        {
            Token = "5773151578:AAH2GEcv7Ey3LKcnM_Z0lJpmH0NiXf1Dttk";
            _bot = new TelegramBotClient(Token);
            _cts = new CancellationTokenSource();
            _usersIds = new List<long>();
            _currentOrders = new Dictionary<long, Order>(1);
            dishes = new Dictionary<long, List<Dish>>();
            _coopUsers = new List<User>();
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
            foreach (var id in _usersIds)
            {
                dishes.Add(id,new List<Dish>());
            }
            _typeHandler = TypeHandler.None;
        }
        public void Stop()
        {
            _cts.Cancel();
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {

                if (update.Message != null)
                {
                    _id = update.Message.Chat.Id;
                }
                if (_countUserInOrder <= 0 && _currentOrders.Count() != 0)
                {

                    var order = _currentOrders.FirstOrDefault().Value;
                    order.Menu = _currentCafes[_indexCafe-1].Menu;
                    order.Cafe = _currentCafes[_indexCafe-1];
                    var text = "";
                    foreach (var dish in order.Dishes)
                    {
                        if(dish.Count !=0)
                            text += $"Блюдо: {dish.Name}, {dish.Count} шт. \n\r";
                    }
                    foreach (var id in _usersIds)
                    {
                        if (_coopUsers.Find(x => x.Id == id) == null)
                        {
                            continue;
                        }
                        await botClient.SendTextMessageAsync(id, $"Заказ завершен. Общая сумма:{order.TotalAmount} \n\r" + text);
                        if (id != _initUser.Id)
                        {
                            await botClient.SendTextMessageAsync(id, $"Оплачивает {_initUser.Name}, номер: {_initUser.PhoneNumber}");
                        }
                    }
                    DBController.AddNewOrder(order);
                    _coopUsers.Clear();
                    _currentOrders.Clear();
                    return;
                }
                if (update.Message == null && update.CallbackQuery != null)
                {
                    var call = update.CallbackQuery;
                    var message = call.Message;
                    var chatId = message.Chat.Id;

                    if (call.Data == "last_dish")
                    {
                        if (_currentCafes == null)
                        {
                            _currentCafes = DBController.GetAllCafes().ToList();
                        }
                        if (_currentDishesLength == 0)
                        {
                            _currentDishesLength = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList().Count();
                        }
                        if (dishes[chatId].Count() == 0)
                        {
                            dishes[chatId] = DBController.GetDishes(_currentCafes[_indexCafe-1].Menu.Id).ToList();
                        }
                        if (_currentDishIndex > 0)
                        {
                            _currentDishIndex--;
                            InlineKeyboardMarkup inlineKeyboard = new(new[]
                            {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "last_dish"),
                                InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "next_dish"),
                            },
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "remove_dish"),
                                InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "add_dish"),
                                InlineKeyboardButton.WithCallbackData(text: "Заказать", callbackData: "take_order")
                            },
                        });
                            var text = $"{_currentDishIndex + 1}) {dishes[chatId][_currentDishIndex].Name} — цена: {dishes[chatId][_currentDishIndex].Price}, скидка: {dishes[chatId][_currentDishIndex].Discount}, заказано(шт.):{dishes[chatId][_currentDishIndex].Count} \n\r Описание {dishes[chatId][_currentDishIndex].Description}";
                            await _bot.EditMessageMediaAsync(
                            chatId: chatId,
                            messageId: call.Message.MessageId,
                            media: new InputMediaPhoto(new InputMedia($"{dishes[chatId][_currentDishIndex].PathImage}")) { ParseMode = ParseMode.Html, Caption = "Переход!_)" },
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
                            await _bot.EditMessageMediaAsync(
                            chatId: chatId,
                            messageId: call.Message.MessageId,
                            media: new InputMediaPhoto(new InputMedia($"{dishes[chatId][_currentDishIndex].PathImage}")) { ParseMode = ParseMode.Html, Caption = text },
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
                        }
                        return;
                    }
                    if (call.Data == "next_dish")
                    {
                        if (_currentCafes == null)
                        {
                            _currentCafes = DBController.GetAllCafes().ToList();
                        }
                        if (_currentDishesLength == 0)
                        {
                            _currentDishesLength = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList().Count();
                        }
                        if (dishes[chatId].Count == 0)
                        {
                            dishes[chatId] = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList();
                        }
                        if (_currentDishIndex < dishes[chatId].Count() - 1)
                        {
                            InlineKeyboardMarkup inlineKeyboard = new(new[]
                            {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "last_dish"),
                                InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "next_dish"),
                            },
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "remove_dish"),
                                InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "add_dish"),
                                InlineKeyboardButton.WithCallbackData(text: "Заказать", callbackData: "take_order")
                            },
                        });
                            _currentDishIndex++;
                            var text = $"{_currentDishIndex + 1}) {dishes[chatId][_currentDishIndex].Name} — цена: {dishes[chatId][_currentDishIndex].Price}, скидка: {dishes[chatId][_currentDishIndex].Discount}, заказано(шт.):{dishes[chatId][_currentDishIndex].Count} \n\r Описание {dishes[chatId][_currentDishIndex].Description}";
                            await _bot.EditMessageMediaAsync(
                            chatId: chatId,
                            messageId: call.Message.MessageId,
                            media: new InputMediaPhoto(new InputMedia($"{dishes[chatId][_currentDishIndex].PathImage}")) { ParseMode = ParseMode.Html, Caption = "Переход!_)" },
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
                            await _bot.EditMessageMediaAsync(
                            chatId: chatId,
                            messageId: call.Message.MessageId,
                            media: new InputMediaPhoto(new InputMedia($"{dishes[chatId][_currentDishIndex].PathImage}")) { ParseMode = ParseMode.Html, Caption = text },
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
                        }
                        return;
                    }
                    if (call.Data == "remove_dish")
                    {
                        if (_currentCafes == null)
                        {
                            _currentCafes = DBController.GetAllCafes().ToList();
                        }
                        if (_currentDishesLength == 0)
                        {
                            _currentDishesLength = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList().Count();
                        }
                        if (dishes[chatId].Count() == 0)
                        {
                            dishes[chatId] = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList();
                        }
                        InlineKeyboardMarkup inlineKeyboard = new(new[]
                        {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "last_dish"),
                            InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "next_dish"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "remove_dish"),
                            InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "add_dish"),
                            InlineKeyboardButton.WithCallbackData(text: "Заказать", callbackData: "take_order")
                        },
                    });
                        if (dishes[chatId][_currentDishIndex].Count > 0)
                            dishes[chatId][_currentDishIndex].Count--;
                        else
                            return;

                        var text = $"{_currentDishIndex + 1}) {dishes[chatId][_currentDishIndex].Name} — цена: {dishes[chatId][_currentDishIndex].Price}, скидка: {dishes[chatId][_currentDishIndex].Discount}, заказано(шт.):{dishes[chatId][_currentDishIndex].Count} \n\r Описание {dishes[chatId][_currentDishIndex].Description}";
                        await _bot.EditMessageMediaAsync(
                            chatId: chatId,
                            messageId: call.Message.MessageId,
                            media: new InputMediaPhoto(new InputMedia($"{dishes[chatId][_currentDishIndex].PathImage}")) { ParseMode = ParseMode.Html, Caption = "Переход!_)" },
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
                        await _bot.EditMessageMediaAsync(
                        chatId: chatId,
                        messageId: call.Message.MessageId,
                        media: new InputMediaPhoto(new InputMedia($"{dishes[chatId][_currentDishIndex].PathImage}")) { ParseMode = ParseMode.Html, Caption = text },
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
                        return;
                    }
                    if (call.Data == "add_dish")
                    {
                        if (_currentCafes == null)
                        {
                            _currentCafes = DBController.GetAllCafes().ToList();
                        }
                        if (_currentDishesLength == 0)
                        {
                            _currentDishesLength = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList().Count();
                        }
                        if (dishes[chatId].Count() == 0)
                        {
                            dishes[chatId] = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList();
                        }
                        InlineKeyboardMarkup inlineKeyboard = new(new[]
                        {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "last_dish"),
                            InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "next_dish"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "remove_dish"),
                            InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "add_dish"),
                            InlineKeyboardButton.WithCallbackData(text: "Заказать", callbackData: "take_order")
                        },
                    });
                        dishes[chatId][_currentDishIndex].Count++;
                        var text = $"{_currentDishIndex + 1}) {dishes[chatId][_currentDishIndex].Name} — цена: {dishes[chatId][_currentDishIndex].Price}, скидка: {dishes[chatId][_currentDishIndex].Discount}, заказано(шт.):{dishes[chatId][_currentDishIndex].Count} \n\r Описание {dishes[chatId][_currentDishIndex].Description}";
                        await _bot.EditMessageMediaAsync(
                            chatId: chatId,
                            messageId: call.Message.MessageId,
                            media: new InputMediaPhoto(new InputMedia($"{dishes[chatId][_currentDishIndex].PathImage}")) { ParseMode = ParseMode.Html, Caption = "Переход!_)" },
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
                        await _bot.EditMessageMediaAsync(
                        chatId: chatId,
                        messageId: call.Message.MessageId,
                        media: new InputMediaPhoto(new InputMedia($"{dishes[chatId][_currentDishIndex].PathImage}")) { ParseMode = ParseMode.Html, Caption = text },
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
                        return;
                    }
                    if (call.Data == "take_order")
                    {
                        if (_currentCafes == null)
                        {
                            _currentCafes = DBController.GetAllCafes().ToList();
                        }
                        if (_currentDishesLength == 0)
                        {
                            _currentDishesLength = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList().Count();
                        }
                        if (dishes[chatId].Count() == 0)
                        {
                            dishes[chatId] = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList();
                        }
                        if (_currentOrders.Count == 0)
                        {
                            //_coopUsers = new List<User>();
                            //await botClient.SendTextMessageAsync(message.Chat, "начинается инициализация заказа");
                            //_initUser = DBController.FindUserByIndex(chatId);
                            //_initUser.IsInit = true;

                            ///* нужно продумать момент с несколькими заказами */
                            ////_currentOrders.Add( chatId, new Order {Name =$"заказ {DBController.CountOrders()}", Users = new List<User> { _initUser} });
                            //_currentOrders = new Dictionary<long, Order>(1);
                            //_currentOrders.Add(chatId, new Order { Name = $"заказ {DBController.CountOrders()}", Users = new List<User> { _initUser } });
                            //InlineKeyboardMarkup inlineKeyboard = new(new[]
                            //{
                            //    new []
                            //    {
                            //        InlineKeyboardButton.WithCallbackData(text: "Да", callbackData: "join_to_order_yes"),
                            //        InlineKeyboardButton.WithCallbackData(text: "Нет", callbackData: "join_to_order_no"),
                            //    },
                            //});
                            //foreach (var id in _usersIds)
                            //{
                            //    if (id != chatId)
                            //    {
                            //        var sentMessage = await botClient.SendTextMessageAsync(
                            //        chatId: id,
                            //        text: $"Пользователь {_initUser.Name} инициировал заказ, хотите присоединиться?",
                            //        replyMarkup: inlineKeyboard,
                            //        cancellationToken: cancellationToken);
                            //    }
                            //}
                            //_currentOrders[chatId].Menu = _currentCafes[_indexCafe].Menu;
                            //_currentOrders[chatId].Cafe = _currentCafes[_indexCafe];
                            await botClient.SendTextMessageAsync(message.Chat, "Вариант с заказои сразу через кафе пока в разработке, для осуществления заказала пожалуйста наберите /order");
                        }
                        else
                        {
                            var order = _currentOrders.FirstOrDefault().Value;
                            if(order.Dishes == null)
                                order.Dishes = new List<Dish>();
                            order.Dishes.AddRange(dishes[chatId]);
                            var userSum = dishes[chatId].Select(x => x.Count * x.Price).Sum();
                            order.TotalAmount += userSum;
                            _countUserInOrder--;
                            var text = "";
                            var index = 0;
                            foreach (var dish in dishes[chatId])
                            {
                                if (dish.Count == 0)
                                    continue;
                                text += $"{++index}) {dish.Name} цена: {dish.Price} {dish.Count} шт. \n\r";
                            }
                            text += $"Ваша сумма заказа: {userSum}";
                            await botClient.SendTextMessageAsync(message.Chat, text);
                            await botClient.SendTextMessageAsync(message.Chat, $"Осталось {_countUserInOrder}, пожалуйста подождите пока все завершат заказ.");
                            if (_countUserInOrder <= 0 && _currentOrders.Count() != 0)
                            {

                                order.Menu = _currentCafes[_indexCafe - 1].Menu;
                                order.Cafe = _currentCafes[_indexCafe - 1];
                                text = "";
                                foreach (var dish in order.Dishes)
                                {
                                    if (dish.Count != 0)
                                        text += $"Блюдо: {dish.Name}, {dish.Count} шт. \n\r";
                                }
                                foreach (var id in _usersIds)
                                {
                                    if (_coopUsers.Find(x => x.Id == id) == null)
                                    {
                                        continue;
                                    }
                                    await botClient.SendTextMessageAsync(id, $"Заказ завершен. Общая сумма:{order.TotalAmount} \n\r" + text);
                                    if (id != _initUser.Id)
                                    {
                                        await botClient.SendTextMessageAsync(id, $"Оплачивает {_initUser.Name}, номер: {_initUser.PhoneNumber}");
                                    }
                                }
                                DBController.AddNewOrder(order);
                                _coopUsers.Clear();
                                _currentOrders.Clear();
                                return;
                            }
                        }
                        return;
                    }
                    if (_typeHandler == TypeHandler.ChooseCafe)
                    {
                        if (_currentCafes == null)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "простите ничего не нашел");
                            return;
                        }
                        await botClient.SendTextMessageAsync(message.Chat, "Ищу меню ");
                        var text = "";
                        InlineKeyboardMarkup inlineKeyboard = new(new[]
                        {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "last_dish"),
                            InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "next_dish"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "remove_dish"),
                            InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "add_dish"),
                            InlineKeyboardButton.WithCallbackData(text: "Заказать", callbackData: "take_order")
                        },
                    });
                        if (!int.TryParse(call.Data, out _indexCafe))
                            _indexCafe = 1;
                        dishes[chatId] = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList();
                        _currentDishesLength = dishes[chatId].Count();
                        _currentDishIndex = 0;
                        text = $"{_currentDishIndex + 1}) {dishes[chatId][_currentDishIndex].Name} — цена: {dishes[chatId][_currentDishIndex].Price}, скидка: {dishes[chatId][_currentDishIndex].Discount}, заказано(шт.):{dishes[chatId][_currentDishIndex].Count} \n\r Описание {dishes[chatId][_currentDishIndex].Description}";
                        var photo = await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: $"{dishes[chatId][_currentDishIndex].PathImage}",
                            caption: text,
                            replyMarkup: inlineKeyboard,
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken);
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
                        _user = new User { Id = _id, Name = "Anonim", PhoneNumber = "0" };
                        if (DBController.AddNewUser(_user))
                            await botClient.SendTextMessageAsync(message.Chat, "спасибо за регистрацию, я вас запомнил!)");
                        else
                            await botClient.SendTextMessageAsync(message.Chat, "что то пошло не так !(");
                        return;
                    }

                    if (_initUser != null && _initUser.Id != chatId && call.Data == "join_to_order_yes" && _typeHandler == TypeHandler.OrderChooseMenu && _coopUsers.Find(x => x.Id == chatId) == null)
                    {
                        var user = DBController.FindUserByIndex(chatId);
                        if (_coopUsers.FirstOrDefault(x => x.Id == user.Id) == null)
                        {
                            _coopUsers.Add(user);
                            _countUserInOrder++;
                        }
                        await botClient.SendTextMessageAsync(message.Chat, "Вы присоеденились к заказу");
                        await botClient.SendTextMessageAsync(message.Chat, "Подождите пока инициатор выберет кафе!)");
                        return;
                    }
                    if (_initUser != null && _initUser.Id != chatId && call.Data == "join_to_order_yes" && _typeHandler != TypeHandler.OrderChooseMenu)
                    {
                        if (_currentCafes == null || _coopUsers == null)
                        {
                            await botClient.SendTextMessageAsync(chatId, "простите ничего не нашел");
                            return;
                        }
                        var user = DBController.FindUserByIndex(chatId);
                        if (_coopUsers.FirstOrDefault(x => x.Id == user.Id) == null)
                        {
                            _coopUsers.Add(user);
                            _countUserInOrder++;
                        }

                        await botClient.SendTextMessageAsync(chatId, "Ищу меню ");
                        var text = "";
                        InlineKeyboardMarkup inlineKeyboard = new(new[]
                        {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "last_dish"),
                            InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "next_dish"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "remove_dish"),
                            InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "add_dish"),
                            InlineKeyboardButton.WithCallbackData(text: "Заказать", callbackData: "take_order")
                        },
                    });
                        if (_indexCafe < 1)
                            _indexCafe = 1;
                        dishes[chatId] = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList();
                        _currentDishesLength = dishes[chatId].Count();
                        _currentDishIndex = 0;
                        text = $"{_currentDishIndex + 1}) {dishes[chatId][_currentDishIndex].Name} — цена: {dishes[chatId][_currentDishIndex].Price}, скидка: {dishes[chatId][_currentDishIndex].Discount}, заказано(шт.):{dishes[chatId][_currentDishIndex].Count} \n\r Описание {dishes[chatId][_currentDishIndex].Description}";
                        var photo = await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: $"{dishes[chatId][_currentDishIndex].PathImage}",
                        caption: text,
                        replyMarkup: inlineKeyboard,
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                        return;
                    }
                    if (_typeHandler == TypeHandler.OrderChooseMenu)
                    {
                        if (_initUser.Id == chatId)
                        {
                            foreach (var id in _usersIds)
                            {
                                if (_coopUsers.Find(x => x.Id == id) == null && _initUser.Id != id)
                                {
                                    continue;
                                }
                                if (_initUser.Id != chatId)
                                {
                                    await botClient.SendTextMessageAsync(message.Chat, "Вы присоеденились к заказу");
                                }
                                if (_currentCafes == null)
                                {
                                    await botClient.SendTextMessageAsync(id, "простите ничего не нашел");
                                    return;
                                }
                                await botClient.SendTextMessageAsync(id, "Ищу меню ");
                                var text = "";
                                InlineKeyboardMarkup inlineKeyboard = new(new[]
                                {
                                new []
                                {
                                    InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "last_dish"),
                                    InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "next_dish"),
                                },
                                new []
                                {
                                    InlineKeyboardButton.WithCallbackData(text: "-", callbackData: "remove_dish"),
                                    InlineKeyboardButton.WithCallbackData(text: "+", callbackData: "add_dish"),
                                    InlineKeyboardButton.WithCallbackData(text: "Заказать", callbackData: "take_order")
                                },
                            });
                                if (!int.TryParse(call.Data, out _indexCafe))
                                    _indexCafe = 1;
                                dishes[chatId] = DBController.GetDishes(_currentCafes[_indexCafe - 1].Menu.Id).ToList();
                                _currentDishesLength = dishes[chatId].Count();
                                _currentDishIndex = 0;
                                text = $"{_currentDishIndex + 1}) {dishes[chatId][_currentDishIndex].Name} — цена: {dishes[chatId][_currentDishIndex].Price}, скидка: {dishes[chatId][_currentDishIndex].Discount}, заказано(шт.):{dishes[chatId][_currentDishIndex].Count} \n\r Описание {dishes[chatId][_currentDishIndex].Description}";
                                var photo = await botClient.SendPhotoAsync(
                                chatId: id,
                                photo: $"{dishes[chatId][_currentDishIndex].PathImage}",
                                caption: text,
                                replyMarkup: inlineKeyboard,
                                parseMode: ParseMode.Html,
                                cancellationToken: cancellationToken);
                            }
                            _typeHandler = TypeHandler.None;
                        }
                        return;
                    }
                }

                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message != null)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    if (message.Text.ToLower() == "/start")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать в бот. Здесь можно кооперироваться с колеггами по работе и совершать общий заказ еды!_) Для справки наберите /help");
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
                        if (_currentOrders.Count() != 0)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Идет другой заказ пожалуйста подождите");
                            return;
                        }
                        _countUserInOrder = 1;
                        _typeHandler = TypeHandler.OrderChooseMenu;
                        _coopUsers = new List<User>();

                        await botClient.SendTextMessageAsync(message.Chat, "начинается инициализация заказа");
                        _initUser = DBController.FindUserByIndex(chatId);
                        _initUser.IsInit = true;
                        _coopUsers.Add(_initUser);
                        /* нужно продумать момент с несколькими заказами */
                        //_currentOrders.Add( chatId, new Order {Name =$"заказ {DBController.CountOrders()}", Users = new List<User> { _initUser} });
                        _currentOrders = new Dictionary<long, Order>(1);
                        _currentOrders.Add(chatId, new Order { Name = $"заказ {DBController.CountOrders()}", Users = new List<User> { _initUser } });
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
                            text: text,
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
                                if (DBController.AddNewUser(_user))
                                    await botClient.SendTextMessageAsync(message.Chat, "спасибо за регистрацию, я вас запомнил!)");
                                else
                                    await botClient.SendTextMessageAsync(message.Chat, "что то пошло не так !(");
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
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(" we in error handler");
                MessageBox.Show(exception.Message);
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ex = JsonSerializer.Serialize(exception);
            Console.WriteLine(ex);
            Debug.WriteLine(ex);
            Debug.WriteLine(" we in error handler");
            MessageBox.Show(ex);
            Start();
            foreach (var ID in _usersIds)
            {
                await botClient.SendTextMessageAsync(ID, "Что то пошло не так, просим прощеня");
                await botClient.SendTextMessageAsync(ID, "/help");
            }
            
        }

    }
}
