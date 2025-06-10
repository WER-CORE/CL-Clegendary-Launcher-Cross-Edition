using System.Net.Http;
using DiscordRPC;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CL.Script
{
    // Клас для керування Discord Rich Presence
    class DiscordController
    {
        public static DiscordRpcClient clientdsc = new DiscordRpcClient("1210664596289884200");

        /// <summary>
        /// Ініціалізує клієнт Discord Rich Presence із зазначеними текстовими даними.
        /// </summary>
        /// <param name="textDetails"></param>
        /// <returns></returns>
        public static void Initialize(string textDetails)
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

        /// <summary>
        /// Оновлює присутність Discord Rich Presence із зазначеними текстовими даними.
        /// </summary>
        /// <param name="textDetails"></param>
        /// <returns></returns>
        public static void UpdatePresence(string textDetails)
        {
            clientdsc.UpdateDetails("Український лаунчер майнкрафт");
            clientdsc.UpdateState($"{textDetails}");
            clientdsc.UpdateStartTime();
        }

        /// <summary>
        /// Завершує роботу клієнта Discord Rich Presence та звільняє ресурси.
        /// </summary>
        public static void Deinitialize()
        {
            clientdsc.Dispose();
        }
    }
}
