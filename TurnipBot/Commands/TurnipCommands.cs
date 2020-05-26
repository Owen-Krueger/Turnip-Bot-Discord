using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TurnipBot.DataAccess;
using TurnipBot.Models;
using TurnipBot.Services;

namespace TurnipBot.Commands
{
    public class TurnipCommands
    {
        private readonly TurnipCalculationService _turnipCalculationService;
        private readonly TurnipRepository _turnipRepository;

        public TurnipCommands()
        {
            _turnipCalculationService = new TurnipCalculationService();
            _turnipRepository = new TurnipRepository();
        }

        [Command("prices"), Aliases("urls", "list_prices", "predictions", "all_prices", "all_predictions", "list_predictions")]
        [Description("Get current turnip predictions for this week")]
        public async Task Prices(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            List<TurnipInfo> entries = _turnipRepository.GetAllTurnipsTableEntries();
            StringBuilder sb = new StringBuilder();
            foreach (TurnipInfo entry in entries)
            {
                sb.Append($"{entry.Name}: {UriConstructorService.GenerateTurnipUrl(entry)}");
            }

            await ctx.RespondAsync(sb.ToString());
        }

        [Command("buy"), Aliases("buy_price", "add_buy", "add_buy_price", "update_buy_price", "update_buy", "change_buy", "change_buy_price")]
        [Description("Adds or updates the turnip purchasing price from Sunday")]
        public async Task BuyPrice(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string response;

            int price = Convert.ToInt32(ctx.RawArgumentString);

            if (_turnipCalculationService.AddOrUpdateBuyPriceInTable(Convert.ToInt32(ctx.Member.Discriminator), ctx.Member.Username, price))
            {
                response = $"Recorded {ctx.Member.Username}'s buy price as {price} bells.";
            }
            else
            {
                response = "Could not record the price. It's probably Owen's fault.";
            }

            await ctx.RespondAsync(response);
        }

        [Command("pattern"), Aliases("add_pattern", "update_pattern", "change_pattern")]
        [Description("Set the pattern from last week's turnip sales. Defaults to 'Unknown'")]
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

        [Command("first"), Aliases("first_flag", "add_first", "add_first_flag", "update_first", "update_first_flag")]
        [Description("Add or remove the 'first time' flag for the prediction")]
        public async Task FirstTime(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string response;

            bool firstTime = Boolean.TryParse(ctx.RawArgumentString, out firstTime) ? firstTime : true;

            if (_turnipCalculationService.AddFirstTimeToRecord(Convert.ToInt32(ctx.User.Discriminator), ctx.Member.Username, firstTime))
            {
                response = $"Recorded {ctx.User.Username}'s first time flag as {firstTime}.";
            }
            else
            {
                response = "Could not record the first time flag. It's probably Owen's fault.";
            }

            await ctx.RespondAsync(response);
        }

        [Command("delete"), Aliases("delete_all", "remove", "remove_all", "clear", "clear_table")]
        [Description("Delete everything from the turnips table")]
        [Hidden]
        public async Task Delete(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string response;

            await ctx.RespondAsync("Are you sure you want to delete everything in the turnips table?");

            var interactivityModule = ctx.Client.GetInteractivityModule();

            var message = await interactivityModule.WaitForMessageAsync(m => m.Content.Contains("Yes", StringComparison.InvariantCultureIgnoreCase) || m.Content.Contains("No", StringComparison.InvariantCultureIgnoreCase), TimeSpan.FromSeconds(10));

            if (message.Message.Content.Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
            {
                _turnipRepository.DeleteAllTurnipTableEntries();
                response = "Successfully deleted everything.";
            }
            else
            {
                response = "Nothing has been deleted.";
            }

            await ctx.RespondAsync(response);
        }
    }

    [Group("sell", CanInvokeWithoutSubcommand = true)]
    public class SellCommands
    {
        private readonly TurnipCalculationService _turnipCalculationService;

        public SellCommands()
        {
            _turnipCalculationService = new TurnipCalculationService();
        }

        public async Task ExecuteGroupAsync(CommandContext ctx, int price)
        {
            await ctx.TriggerTypingAsync();
            string response;
            string periodOfDay = DateTime.Now.Hour < 12 ? "morning" : "afternoon";

            try
            {
                if (_turnipCalculationService.AddOrUpdateSellPriceInDB(Convert.ToInt32(ctx.Member.Discriminator), ctx.Member.Username, price))
                {
                    response = $"Recorded {ctx.Member.Username}'s sell price for {DateTime.Now.DayOfWeek} {periodOfDay} as {price} bells.";
                }
                else
                {
                    response = "Couldn't record price. It's probably Owen's fault.";
                }
            }
            catch
            {
                response = "Crashed parsing your price. What on earth did you do??";
            }

            await ctx.RespondAsync(response);
        }

        [Command("sell-date")]
        public async Task SellWithDate(CommandContext ctx, int price, DateTime dateTime)
        {
            await ctx.TriggerTypingAsync();
            string response;

            string periodOfDay = dateTime.Hour < 12 ? "morning" : "afternoon";
            if (_turnipCalculationService.AddOrUpdateSellPriceInDB(Convert.ToInt32(ctx.Member.Discriminator), ctx.Member.Username, price))
            {
                response = $"Recorded {ctx.Member.Username}'s sell price for {DateTime.Now.DayOfWeek} {periodOfDay} as {price} bells.";
            }
            else
            {
                response = "Couldn't record price. It's probably Owen's fault.";
            }

            await ctx.RespondAsync(response);
        }
    }
}
