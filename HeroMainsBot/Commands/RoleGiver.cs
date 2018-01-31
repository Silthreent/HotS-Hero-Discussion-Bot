using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace HeroMainsBot.Commands
{
    [Group("role")]
    public class RoleGiver : ModuleBase<SocketCommandContext>
    {
        [Command("get")]
        public async Task GetRole(string role)
        {
            var gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name == role);
            if(gRole == null)
            {
                await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync("The role you requested does not exist");
                await Context.Message.DeleteAsync();
                return;
            }

            if(gRole.Color.RawValue != Discord.Color.Default.RawValue)
            {
                await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync("Ineligible role to give");
                await Context.Message.DeleteAsync();
                return;
            }

            await (Context.User as IGuildUser).AddRoleAsync(gRole);
            await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync("You have been given the role " + gRole.Name + "!");
            await Context.Message.DeleteAsync();
        }

        [Command("remove")]
        public async Task RemoveRole(string role)
        {
            var gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name == role);
            if(gRole == null)
            {
                await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync("The role you mentioned does not exist");
                await Context.Message.DeleteAsync();
                return;
            }

            if(gRole.Color.RawValue != Discord.Color.Default.RawValue)
            {
                await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync("Ineligible role to remove");
                await Context.Message.DeleteAsync();
                return;
            }

            await (Context.User as IGuildUser).RemoveRoleAsync(gRole);
            await Context.Message.DeleteAsync();
        }
    }
}
