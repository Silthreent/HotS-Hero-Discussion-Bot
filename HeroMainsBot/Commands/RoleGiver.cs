using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace HeroMainsBot.Commands
{
    [Group("role")]
    public class RoleGiver : ModuleBase<SocketCommandContext>
    {
        [Command("get")]
        public async Task GetRole(string role)
        {
            var gRole = FindRole(role, Context.Message);
            if(gRole == null)
                return;

            if(gRole.Color.RawValue != Color.Default.RawValue)
            {
                await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync("Ineligible role to give");
                await Context.Message.DeleteAsync();
                return;
            }

            await (Context.User as IGuildUser).AddRoleAsync(gRole);
            Console.WriteLine("Gave " + Context.User.Username + " the role " + gRole.Name);
            await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync("You have been given the role " + gRole.Name + "!");
            await Context.Message.DeleteAsync();
        }

        [Command("remove")]
        public async Task RemoveRole(string role)
        {
            var gRole = FindRole(role, Context.Message);
            if(gRole == null)
                return;

            if(gRole.Color.RawValue != Color.Default.RawValue)
            {
                await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync("Ineligible role to remove");
                await Context.Message.DeleteAsync();
                return;
            }

            await (Context.User as IGuildUser).RemoveRoleAsync(gRole);
            Console.WriteLine("Removed " + gRole.Name + " from " + Context.User.Username);
            await Context.Message.DeleteAsync();
        }

       SocketRole FindRole(string role, SocketUserMessage message)
       {
            var gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name.ToLower() == role.ToLower());
            if(gRole == null)
            {
                gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name.ToLower().Trim(new char[] { ' ', '\'', '.' }) == role.ToLower());
                if(gRole == null)
                {
                    gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name.ToLower().Replace("the ", "") == role.ToLower());
                    if(gRole == null)
                    {
                        gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name.ToLower().Replace("the lost ", "") == role.ToLower());
                        if(gRole == null)
                        {
                            Console.WriteLine("Could not find role " + role);
                            message.Author.GetOrCreateDMChannelAsync().Result.SendMessageAsync("The role you requested does not exist");
                            message.DeleteAsync();

                            return null;
                        }
                    }
                }
            }

            return gRole;
       }
    }
}
