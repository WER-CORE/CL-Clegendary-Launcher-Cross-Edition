using System.Net.Http;
using DiscordRPC;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CL.Script
{
    class DiscordController
    {
        public static DiscordRpcClient clientdsc = new DiscordRpcClient("1210664596289884200");

        public static async Task Initialize(string textDetails)
        {

            clientdsc.Initialize();
            clientdsc.SetPresence(new RichPresence()
            {
                State = textDetails,
                Details = "Український лаунчер майнкрафт",
                Timestamps = Timestamps.Now,

                Assets = new Assets()
                {
                    LargeImageKey = "frame_73",
                }
            });
        }
        public static async Task UpdatePresence(string textDetails)
        {
            clientdsc.UpdateDetails("Український лаунчер майнкрафт");
            clientdsc.UpdateState($"{textDetails}");
            clientdsc.UpdateStartTime();
        }
        public static void Deinitialize()
        {
            clientdsc.Dispose();
        }
    }
}
