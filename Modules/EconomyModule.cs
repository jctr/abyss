using System;
using System.Linq;
using System.Threading.Tasks;
using Abyss.Attributes;
using Abyss.Persistence.Relational;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Abyss.Modules
{
    [Name("Currency")]
    public class EconomyModule : AbyssModuleBase
    {
        [Command("send")]
        [EconomicImpact(EconomicImpactType.UserCoinNeutral)]
        public async Task<DiscordCommandResult> SendMoneyAsync(IMember user, decimal amount)
        {
            var db = Context.Services.GetRequiredService<AbyssPersistenceContext>();
            if (!await db.SubtractCurrencyAsync(Context.Author.Id, amount))
            {
                return Reply("You don't have enough money!");
            }

            var target = await db.GetUserAccountsAsync(user.Id);
            target.Coins += amount;
            await db.SaveChangesAsync();
            return Reply($"Gave {amount} :coin: to {user.Mention}.");
        }
        [Command("top")]
        public async Task<DiscordCommandResult> TopAsync()
        {
            var players = await Context.Services.GetRequiredService<AbyssPersistenceContext>().UserAccounts
                .OrderByDescending(c => c.Coins).Take(5).ToListAsync();

            return Reply(new LocalEmbed()
                .WithTitle($"Richest users, as of {Markdown.Timestamp(DateTimeOffset.Now)}")
                .WithColor(GetColor())
                .WithDescription(string.Join("\n", players.Select((c, pos) =>
                {
                    return $"{pos + 1}) **{Context.Bot.GetUser(c.Id)}** - {c.Coins} coins";
                })))
            );
        }
        
        [Command("coins")]
        public async Task<DiscordCommandResult> CoinsAsync()
        {
            var coins = await Context.Services.GetRequiredService<AbyssPersistenceContext>()
                .GetUserAccountsAsync(Context.Author.Id);
            return Reply($"You have {coins.Coins} :coin: coins. Play some games to earn more.");
        }
    }
}