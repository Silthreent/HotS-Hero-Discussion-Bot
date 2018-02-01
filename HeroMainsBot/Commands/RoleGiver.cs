using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LiteDB;

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
            string lowerRole = role.ToLower();

            var gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name.ToLower() == lowerRole);
            if(gRole == null)
            {
                gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name.ToLower().Trim(new char[] { ' ', '\'', '.' }) == lowerRole);
                if(gRole == null)
                {
                    gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name.ToLower().Replace("the ", "") == lowerRole);
                    if(gRole == null)
                    {
                        gRole = Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name.ToLower().Replace("the lost ", "") == lowerRole);
                        if(gRole == null)
                        {
                            using(var db = new LiteDatabase("HeroMains.db"))
                            {
                                var collection = db.GetCollection<ShortName>("shorthands");

                                ShortName roleName = collection.FindOne(x => x.Short == lowerRole);
                                if(roleName == null)
                                {
                                    Console.WriteLine("Could not find role " + role);
                                    message.Author.GetOrCreateDMChannelAsync().Result.SendMessageAsync("The role you requested does not exist");
                                    message.DeleteAsync();

                                    return null;
                                }

                                gRole = Context.Guild.Roles.First(x => x.Name == roleName.Real);
                            }
                        }
                    }
                }
            }

            return gRole;
       }
    }

    public class ShortName
    {
        public int Id { get; set; }
        public string Short { get; set; }
        public string Real { get; set; }
    }
}
