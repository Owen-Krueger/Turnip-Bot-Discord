﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TurnipBot.DataAccess;
using TurnipBot.Models;
using TurnipBot.Services;

namespace TurnipBot.Commands
{
    public class TurnipCommands : IModule
    {
        private readonly TurnipCalculationService _turnipCalculationService;
        private readonly TurnipRepository _turnipRepository;

        public TurnipCommands()
        {
            _turnipCalculationService = new TurnipCalculationService();
            _turnipRepository = new TurnipRepository();
        }

        [Command("prices")]
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

        [Command("sell")]
        public async Task Sell(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string response;
            int price;
            string periodOfDay = DateTime.Now.Hour < 12 ? "morning" : "afternoon";

            try
            {
                if (ctx.RawArgumentString == null)
                {
                    response = "You must enter a price after the command. Try '/sell 1' or something.";
                }
                else
                {
                    price = Convert.ToInt32(ctx.RawArgumentString);

                    if (_turnipCalculationService.AddOrUpdateSellPriceInDB(Convert.ToInt32(ctx.Member.Discriminator), ctx.Member.Username, price))
                    {
                        response = $"Recorded {ctx.Member.Username}'s sell price for {DateTime.Now.DayOfWeek} {periodOfDay} as {price} bells.";
                    }
                    else
                    {
                        response = "Couldn't record price. It's probably Owen's fault.";
                    }
                }
            }
            catch
            {
                response = "Crashed parsing your price. What on earth did you do??";
            }

            await ctx.RespondAsync(response);
        }

        [Command("buy")]
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

        [Command("first")]
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
    }
}
