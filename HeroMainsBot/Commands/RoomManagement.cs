using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;

namespace HeroMainsBot.Commands
{
    [Group("room")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class RoomManagement : ModuleBase<SocketCommandContext>
    {
        [Command("create")]
        [Summary("Creates a new room/role")]
        public async Task CreateRoom([Summary("Name of the room/role")]string name, [Summary("Which role section it should be")]string section)
        {
            Console.WriteLine("Creating room");
            if(Context.Guild.Roles.DefaultIfEmpty(null).FirstOrDefault(x => x.Name == name) != null)
            {
               await Context.Channel.SendMessageAsync("Role already exists");
               return;
            }

            var role = await Context.Guild.CreateRoleAsync(name);
            var type = Context.Guild.Roles.First(x => x.Name == section);
            var chnl = await Context.Guild.CreateTextChannelAsync(name);
            await chnl.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny));
            await chnl.AddPermissionOverwriteAsync(role, new OverwritePermissions(readMessages: PermValue.Allow));
            await chnl.AddPermissionOverwriteAsync(type, new OverwritePermissions(readMessages: PermValue.Allow));

            Console.WriteLine("Created room");
        }
    }
}
