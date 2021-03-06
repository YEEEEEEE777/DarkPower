using System;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using System.Timers;

namespace SpeechInfo
{
    internal class Program
    {
        private static System.Timers.Timer aTimer;
        private static Boolean roshanDead;
        private static int roshanRespawnMinTime;
        private static int roshanRespawnMaxTime;
        private static readonly Ensage.Common.Menu.Menu Menu = new Ensage.Common.Menu.Menu("SpeechInfo", "SInfo", true);

        private static void Main(string[] args)
        {
            Menu.AddItem(new MenuItem("S_RU", "Speech RUS").SetValue(false));
            Menu.AddItem(new MenuItem("S_Rune", "Speech Rune").SetValue(true));
            Menu.AddItem(new MenuItem("S_Rune_T", "Time per S_Rune").SetValue(new Slider(15, 0, 60)).SetTooltip("tick per speech"));
            Menu.AddItem(new MenuItem("S_Midas", "Speech Midas").SetValue(true));
            Menu.AddItem(new MenuItem("S_Midas_T", "Time per S_Midas").SetValue(new Slider(5, 0, 10)).SetTooltip("tick per speech"));
            Menu.AddItem(new MenuItem("S_Roshan", "Speech Roshan").SetValue(true));
            Menu.AddToMainMenu();
            roshanDead = false;
            roshanRespawnMinTime = 480;
            roshanRespawnMaxTime = 660;
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            Game.OnFireEvent += Game_OnGameEvent;
        }

        private static void playSound(string path)
        {
            System.Media.SoundPlayer player =
                new System.Media.SoundPlayer();
            String fullpath = Environment.CurrentDirectory;
            fullpath = fullpath.Remove(fullpath.Length - 10);
            fullpath+= @"\dota\materials\sounds\"+path;
            player.SoundLocation = fullpath;
            player.Load();
            player.Play();
        }

        static void Game_OnGameEvent(FireEventEventArgs args)
        {
            if (args.GameEvent.Name == "dota_roshan_kill")
            {
                roshanDead = true;
                roshanRespawnMinTime = 480;
                roshanRespawnMaxTime = 660;
            }
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (!Game.IsInGame) return;
            var me = ObjectMgr.LocalPlayer;
            if (me == null || me.Hero == null) return;
            Boolean SRU = Menu.Item("S_RU").GetValue<bool>();
            
            if (Menu.Item("S_Rune").GetValue<bool>() && ((Math.Round(Game.GameTime) + Menu.Item("S_Rune_T").GetValue<Slider>().Value) % 120) == 0)
                playSound("CheckRune" + ((SRU)?"RU":"EN") + ".wav");

            if (Menu.Item("S_Midas").GetValue<bool>())
            {
                var Midas = me.Hero.FindItem("item_hand_of_midas");
                if (Midas != null && Math.Round(Midas.Cooldown) == Menu.Item("S_Midas_T").GetValue<Slider>().Value)
                {
                    playSound("UseMidas" + ((SRU) ? "RU" : "EN") + ".wav");
                }
            }

            if (Menu.Item("S_Roshan").GetValue<bool>() && roshanDead)
            {
                if(--roshanRespawnMinTime+5 == 0)
                    playSound("RoshanMbAlive" + ((SRU) ? "RU" : "EN") + ".wav");
                if (--roshanRespawnMaxTime + 5 == 0)
                {
                    playSound("RoshanAlive" + ((SRU) ? "RU" : "EN") + ".wav");
                    roshanDead = false;
                }
            }
        }
    }
}
