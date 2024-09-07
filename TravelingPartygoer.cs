using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TravelingPartygoer
{
    public class TPModSystem : ModSystem
    {
        public static LocalizedText ArrivedText { get; private set; }
        bool tMArrivedOnce;
        public override void OnModLoad()
        {
            ArrivedText = Language.GetOrRegister(Mod.GetLocalizationKey("TPModSystem.ArrivedText"));
            base.OnModLoad();
        }
        public override void PostUpdateNPCs()
        {
            if (!tMArrivedOnce && Main.npc.Any(n => n.active && n.type == NPCID.TravellingMerchant))
            {
                tMArrivedOnce = true;
            }

            if (tMArrivedOnce && !Main.dayTime)
            {
                tMArrivedOnce = false;
            }

            if (!tMArrivedOnce && !Main.npc.Any(n => n.active && n.type == NPCID.TravellingMerchant) && BirthdayParty.PartyIsUp && Main.dayTime && Main.time < 27000)
            {
                NPC randromTownNPC = new();
                foreach (var npc in Main.npc)
                {
                    if
                    (
                        npc.active &&
                        npc.townNPC &&
                        npc.type != NPCID.TravellingMerchant &&
                        npc.type != NPCID.OldMan &&
                        npc.type != NPCID.SkeletonMerchant &&
                        npc.type != NPCID.BoundMechanic &&
                        npc.type != NPCID.BoundWizard &&
                        npc.type != NPCID.BoundTownSlimeOld &&
                        npc.type != NPCID.BoundTownSlimePurple &&
                        npc.type != NPCID.BoundTownSlimeYellow
                    )
                    {
                        randromTownNPC = npc;
                        break;
                    }
                }

                if (randromTownNPC != null)
                {
                    int tMID = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)randromTownNPC.position.X, (int)randromTownNPC.position.Y, NPCID.TravellingMerchant);
                    Chest.SetupTravelShop();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.NewText($"{Main.npc[tMID].FullName} {ArrivedText.Value}", 46, 116, 238);
                    }
                    else
                    {
                        Terraria.Chat.ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"{Main.npc[tMID].FullName} {ArrivedText.Value}"), new Color(46, 116, 238));
                    }
                }
            }
            base.PostUpdateNPCs();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("tMArrivedOnce", tMArrivedOnce);
            base.SaveWorldData(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            tMArrivedOnce = tag.GetBool("tMArrivedOnce");
            base.LoadWorldData(tag);
        }
    }
}
