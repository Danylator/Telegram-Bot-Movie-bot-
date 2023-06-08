using API_Space;
using Microsoft.VisualBasic;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotMethodsETC
{
    class TelegramBotStarter
    {
        //5920638289:AAGBXSs-9zPV65dhNaFIwlLb1zoKncxngIA
        //t.me/CinemaAPIsSearchBot
        //https://core.telegram.org/bots/api
        TelegramBotClient botClient = new TelegramBotClient("5920638289:AAGBXSs-9zPV65dhNaFIwlLb1zoKncxngIA");
        ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

        Dictionary<long, TelegramUserDatas> usr = new Dictionary<long, TelegramUserDatas>();
        Dictionary<string, List<string>> comnts = new Dictionary<string, List<string>>();
        public async Task Start()
        {
            var cts = new CancellationTokenSource();
            botClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken: cts.Token);
            var botMe = await botClient.GetMeAsync();
            Console.WriteLine($"Bot {botMe} has start to work");
            Console.ReadLine();
        }


        async Task HandlerUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
           
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessageAsync(client, update.Message);
            }
            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(client, update.CallbackQuery);
            }

        }
        async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            if (!usr.ContainsKey(message.Chat.Id))
                usr.Add(message.Chat.Id, new TelegramUserDatas(message.Chat.Username));
            ReplyKeyboardMarkup replyKeyboardMarkup = new
                (
                new[]
        {
                new KeyboardButton[] { "Find Film By Genere and Year"},


        }
                )
            {
                ResizeKeyboard = true
            };



            if (message.Text.ToLower().Contains("start") && !usr[message.Chat.Id].StartedOrNo)
            {

                usr[message.Chat.Id].StartedOrNo = true;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Welcome to the Movie Bot!\nWhat would you like to do?");

                await botClient.SendTextMessageAsync(message.Chat.Id, "Choice option in keyboard: ", replyMarkup: replyKeyboardMarkup);

            }
            if (usr[message.Chat.Id].StartedOrNo)
            {

                if (message.Text == "Find Film By Genere and Year")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "To do this, you need to choose one of the genres suggested below, and write the year of the movie.");
                    usr[message.Chat.Id].FinderStarter = true;
                    if (usr[message.Chat.Id].FinderStarter)
                    {
                        string[] Titles = await API_Methods1.GetAllGeneres();

                        var buttonArrays = new List<InlineKeyboardButton[]>();
                        int am = 3;

                        int numArrays = (int)Math.Ceiling((double)Titles.Length / am);


                        for (int i = 0; i < numArrays; i++)
                        {
                            var buttons = new List<InlineKeyboardButton>();


                            for (int j = i * am; j < (i + 1) * am && j < Titles.Length; j++)
                            {
                                string title = Titles[j];
                                string value = $"{Titles[j]}%^";
                                var button = InlineKeyboardButton.WithCallbackData(title, value);
                                buttons.Add(button);
                            }

                            buttonArrays.Add(buttons.ToArray());
                        }


                        InlineKeyboardMarkup inl = new InlineKeyboardMarkup(buttonArrays.ToArray());
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Choice one of that Generes: ", replyMarkup: inl);
                        return;
                    }
                }
                else if (usr[message.Chat.Id].ComWrite)
                {
                    if (!comnts.ContainsKey(usr[message.Chat.Id].MomentumMovieContainer))
                        comnts.Add(usr[message.Chat.Id].MomentumMovieContainer, new List<string>());
                    comnts[usr[message.Chat.Id].MomentumMovieContainer].Add($"\nUsername - {message.Chat.Username}\nReview: \n{message.Text}\n\nReviev date - {DateTime.Now}\n============================");
                    InlineKeyboardMarkup inl = new(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Look at the reviews", "ComentLook")
                        }, new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Back to menu", "LeveWitoutComment")
                        }
                    });
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Review successfully added!", replyMarkup: inl);
                    return;
                }
                else if (message.Text.Length <= 4 && usr[message.Chat.Id].FinderStarter)
                {
                    int number;
                    bool success = int.TryParse(message.Text, out number);
                    if (success && number >= 1940 && number <= 2023)
                    {
                        usr[message.Chat.Id].FinderStarter = false;
                        usr[message.Chat.Id].ComWrite = false;
                        usr[message.Chat.Id].ComentStarter = false;
                        usr[message.Chat.Id].MovieFindStarter = true;
                        usr[message.Chat.Id].OptionsDataUsedForSearchMovies[1] = number.ToString();
                        if (usr[message.Chat.Id].MovieFindStarter)
                        {
                            string[] Titles = await API_Methods1.GetMovieByNameAndGenere(usr[message.Chat.Id].OptionsDataUsedForSearchMovies[1], usr[message.Chat.Id].OptionsDataUsedForSearchMovies[0]);
                            usr[message.Chat.Id].ComentStarter = false;
                            var buttonArrays = new List<InlineKeyboardButton[]>();

                            if (Titles.Length > 2)
                            {
                                int numArrays = (int)Math.Ceiling((double)Titles.Length / 2);
                                for (int i = 0; i < numArrays; i++)
                                {
                                    var buttons = new List<InlineKeyboardButton>();
                                    for (int j = i * 2; j < (i + 1) * 2 && j < Titles.Length; j++)
                                    {
                                        string title = Titles[j];
                                        string value = $"{Titles[j]}^";
                                        var button = InlineKeyboardButton.WithCallbackData(title, value);
                                        buttons.Add(button);
                                    }

                                    buttonArrays.Add(buttons.ToArray());
                                }
                            }
                            else if (Titles.Length == 1 && Titles[0].Contains("movie was not found"))
                            {
                                InlineKeyboardMarkup jns = new(new[]
                            {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Back to menu", "LeveWitoutComment")
                        }

                    });
                                await botClient.SendTextMessageAsync(message.Chat.Id, "We are sorry! There are no Movies according to the conditions you provided!", replyMarkup: jns);
                                usr[message.Chat.Id].MovieFindStarter = false;
                                usr[message.Chat.Id].OptionsDataUsedForSearchMovies[0] = "";
                                usr[message.Chat.Id].OptionsDataUsedForSearchMovies[1] = "";
                                return;
                            }
                            else
                            {
                                var buttons = new List<InlineKeyboardButton>();

                                for (int i = 0; i < Titles.Length; i++)
                                {
                                    string title = Titles[i];
                                    string value = $"{Titles[i]}^";
                                    var button = InlineKeyboardButton.WithCallbackData(title, value);
                                    buttons.Add(button);
                                }

                                buttonArrays.Add(buttons.ToArray());
                            }

                            InlineKeyboardMarkup inl = new InlineKeyboardMarkup(buttonArrays.ToArray());
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Here are the movies that were found according to the conditions you provided: ", replyMarkup: inl);
                            usr[message.Chat.Id].MovieFindStarter = false;
                            usr[message.Chat.Id].OptionsDataUsedForSearchMovies[0] = "";
                            usr[message.Chat.Id].OptionsDataUsedForSearchMovies[1] = "";
                            return;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Incorrect Year. Try again!");
                        return;
                    }

                }
                else
                {
                    if(!message.Text.ToLower().Contains("start"))
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Command does not exist, please follow the instructions!");
                }
             





                return;
            }

        }
        async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            if (!usr.ContainsKey(callbackQuery.Message.Chat.Id))
                usr.Add(callbackQuery.Message.Chat.Id, new TelegramUserDatas(callbackQuery.Message.Chat.Username));
            string s = await API_Methods1.GetAllGeneresString();
            if (s.Contains((callbackQuery.Data).Replace("%^", "")))
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the year of the movie: ");
                usr[callbackQuery.Message.Chat.Id].FinderStarter = true;
                usr[callbackQuery.Message.Chat.Id].ComWrite = false;
                //Genere saver:
                usr[callbackQuery.Message.Chat.Id].OptionsDataUsedForSearchMovies[0] = (callbackQuery.Data).Replace("%^", "");

            }
            //Used for MOVIES!
            else if ((callbackQuery.Data).Contains("^"))
            {
                usr[callbackQuery.Message.Chat.Id].ComentStarter = true;
                usr[callbackQuery.Message.Chat.Id].MomentumMovieContainer = callbackQuery.Data.Replace("^", "");

                if (usr[callbackQuery.Message.Chat.Id].ComentStarter)
                {
                    InlineKeyboardMarkup inl = new(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Look at the comments", "ComentLook"),
                            InlineKeyboardButton.WithCallbackData("Leave a comment", "ComentLeave")
                        }, new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Back to menu", "LeveWitoutComment")
                        }
                    });
                    string image = await API_Methods2.GetImage(usr[callbackQuery.Message.Chat.Id].MomentumMovieContainer);
                    if (image != "No Image found")
                        await botClient.SendPhotoAsync(
       chatId: callbackQuery.Message.Chat.Id,
       photo: InputFile.FromUri(image),
       caption: $"Movie: {usr[callbackQuery.Message.Chat.Id].MomentumMovieContainer}\nSelect one of the buttons:", replyMarkup: inl,
       parseMode: ParseMode.Html
       );
                    else
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"<<{image}>>\nMovie: {usr[callbackQuery.Message.Chat.Id].MomentumMovieContainer}\nSelect one of the buttons:", replyMarkup: inl);

                    return;
                }

            }
            if ((callbackQuery.Data) == "ComentLook")
            {
                string allcmts = "";
                if (comnts.ContainsKey(usr[callbackQuery.Message.Chat.Id].MomentumMovieContainer))
                {
                    for (int i = 0; i < comnts[usr[callbackQuery.Message.Chat.Id].MomentumMovieContainer].Count; i++)
                    {
                        allcmts += comnts[usr[callbackQuery.Message.Chat.Id].MomentumMovieContainer][i] + "\n";
                    }
                }

                InlineKeyboardMarkup inl = new(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Leave a review", "ComentLeave")
                        }, new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Back to menu", "LeveWitoutComment")
                        }
                    });

                if (allcmts.Length > 1)
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Coments:\n  {allcmts}", replyMarkup: inl);
                else
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"There are no reviews yet, be the first!", replyMarkup: inl);

                usr[callbackQuery.Message.Chat.Id].ComentStarter = false;
            }
            if ((callbackQuery.Data) == "ComentLeave")
            {
                usr[callbackQuery.Message.Chat.Id].ComWrite = true;
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Leave your movie review: ");


                usr[callbackQuery.Message.Chat.Id].ComentStarter = false;

            }
            if ((callbackQuery.Data) == "LeveWitoutComment")
            {


                usr[callbackQuery.Message.Chat.Id].ComWrite = false;
                usr[callbackQuery.Message.Chat.Id].MomentumMovieContainer = "";
                await botClient?.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Choice option in keyboard what you would like to do:");
                usr[callbackQuery.Message.Chat.Id].ComentStarter = false;
            }

            return;
        }
        private Task HandlerError(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            var ErrorMassage = exception switch
            {
                ApiRequestException apiRequestException => $"Error in Tellegram bot API: \n{apiRequestException.ErrorCode}" +
                $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMassage);
            return Task.CompletedTask;
        }


    }
    public class TelegramUserDatas
    {
        public string Login;
        public bool StartedOrNo = false;
        public bool FinderStarter = false;
        public bool MovieFindStarter = false;
        public bool ComentStarter = false;
        public bool ComWrite = false;
        public string MomentumMovieContainer;
        public string[] OptionsDataUsedForSearchMovies = new string[2];
        public TelegramUserDatas(string login)
        {
            this.Login = login;
            MomentumMovieContainer = "";
        }
    }
}