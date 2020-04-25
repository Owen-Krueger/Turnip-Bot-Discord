using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TurnipBot.Models;
using TurnipBot.Services;

namespace TurnipBot.Commands
{
    public class TurnipCommands : IModule
    {
        TurnipCalculationService _turnipCalculationService;

        public TurnipCommands()
        {
            _turnipCalculationService = new TurnipCalculationService();
        }

        [Command("prices")]
        [Description("Get current turnip predictions for this week")]
        public async Task Alive(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            TurnipInfo info = new TurnipInfo()
            {
                Name = "Owen",
                BuyPrice = 100,
                SellPrices = new List<int>()
                {
                    95, 100, 105
                }
            };

            await ctx.RespondAsync("Owen: " + UriConstructorService.GenerateTurnipUrl(info, true));
        }

        [Command("sell")]
        [Description("Test")]
        public async Task New(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string message;
            int price;
            try
            {
                if (ctx.RawArgumentString == null)
                {
                    message = "You must enter a price after the command. Try '/sell 1' or something.";
                }
                else
                {
                    price = Convert.ToInt32(ctx.RawArgumentString);

                    if (_turnipCalculationService.AddOrUpdateTurnipPriceToDB(Convert.ToInt32(ctx.Member.Discriminator), ctx.Member.Username, price))
                    {
                        message = "Sucessfully recorded price!";
                    }
                    else
                    {
                        message = "Couldn't record price. It's probably Owen's fault.";
                    }
                }
            }
            catch
            {
                message = "Crashed parsing your price. What on earth did you do??";
            }

            await ctx.RespondAsync(message);
        }

        [Command("buy")]
        public async Task Sell(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            int price = Convert.ToInt32(ctx.RawArgumentString);

            _turnipCalculationService.AddBuyPriceToTable(Convert.ToInt32(ctx.Member.Discriminator), ctx.Member.Username, price);
        }

        [Command("pattern")]
        public async Task Pattern(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            bool success;

            if (Enum.TryParse(ctx.RawArgumentString, out PatternEnum pattern))
                success = _turnipCalculationService.AddPatternToRecord(Convert.ToInt32(ctx.User.Discriminator), pattern);
            else
                success = false;

            if (!success)
                await ctx.RespondAsync("Pattern not recognized. Values accepted: 'Unknown', 'Decreasing', 'LargeSpike', 'SmallSpike', 'Fluctuating'");
        }

        [Command("firstTime")]
        public async Task FirstTime(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            bool success;

            if (Boolean.TryParse(ctx.RawArgumentString, out bool firstTime))
                success = _turnipCalculationService.AddFirstTimeToRecord(Convert.ToInt32(ctx.User.Discriminator), firstTime);
            else
                success = false;

            if (!success)
                await ctx.RespondAsync("FirstTime boolean not recognized. Values accepted: 'True', 'False'");
        }
    }
}
