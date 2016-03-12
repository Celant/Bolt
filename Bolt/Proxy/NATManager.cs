//
//  NATManager.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using LibMultiplicity;
using LibMultiplicity.Packets;
using v1241 = LibMultiplicity.Packets.v1241;
using v1302 = LibMultiplicity.Packets.v1302;
using v1308 = LibMultiplicity.Packets.v1308;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bolt.Proxy
{
    public enum TranslationType
    {
        Upstream = 0,
        Downstream = 1,
    }
    public class NATManager
    {
        public int ProxyPlayerID;
        public int ServerPlayerID;

        public NATManager(int proxyPlayerID) : this(proxyPlayerID, 0)
        {
        }

        public NATManager(int proxyPlayerID, int serverPlayerID)
        {
            this.ProxyPlayerID = proxyPlayerID;
            this.ServerPlayerID = serverPlayerID;
        }

        public byte[] ProccessPacket(byte[] packet, TranslationType type)
        {
            using (MemoryStream ms = new MemoryStream(packet))
            using (BinaryReader br = new BinaryReader(ms))
            {
                TerrariaPacket decodedPacket = TerrariaPacket.Deserialize(br);
                byte[] translatedPacket;
                switch (type)
                {
                    case TranslationType.Upstream:
                        translatedPacket = TranslatePacketUpstream(decodedPacket);
                        break;
                    case TranslationType.Downstream:
                        translatedPacket = TranslatePacketDownstream(decodedPacket);
                        break;
                    default:
                        translatedPacket = decodedPacket.ToArray();
                        break;
                }
                return translatedPacket;
            }
        }

        private byte[] TranslatePacketUpstream(TerrariaPacket packet)
        {
            return new byte[0];
        }

        private byte[] TranslatePacketDownstream(TerrariaPacket packet)
        {
            return new byte[0];
        }

        private byte[] TranslatePacket(TerrariaPacket packet, byte fromPlayerId, byte toPlayerId)
        {
            switch (packet.ID)
            {
                case (byte) PacketTypes.PlayerInfo:
                    v1302.PlayerInfo playerInfo = packet as v1302.PlayerInfo;
                    playerInfo.PlayerID = CheckPlayerID(playerInfo.PlayerID, fromPlayerId, toPlayerId);
                    return playerInfo.ToArray();
                    break;
                case (byte) PacketTypes.PlayerSlot:
                    v1241.PlayerInventorySlot playerSlot = packet as v1241.PlayerInventorySlot;
                    playerSlot.PlayerID = CheckPlayerID(playerSlot.PlayerID, fromPlayerId, toPlayerId);
                    return playerSlot.ToArray();
                    break;
                case (byte) PacketTypes.PlayerSpawn:
                    v1302.SpawnPlayer playerSpawn = packet as v1302.SpawnPlayer;
                    playerSpawn.PlayerID = CheckPlayerID(playerSpawn.PlayerID, fromPlayerId, toPlayerId);
                    return playerSpawn.ToArray();
                    break;
                case (byte) PacketTypes.PlayerUpdate:
                    v1302.UpdatePlayer playerUpdate = packet as v1302.UpdatePlayer;
                    playerUpdate.PlayerID = CheckPlayerID(playerUpdate.PlayerID, fromPlayerId, toPlayerId);
                    return playerUpdate.ToArray();
                    break;
                case (byte) PacketTypes.PlayerActive:
                    v1241.PlayerActive playerActive = packet as v1241.PlayerActive;
                    playerActive.PlayerID = CheckPlayerID(playerActive.PlayerID, fromPlayerId, toPlayerId);
                    return playerActive.ToArray();
                    break;
                case (byte) PacketTypes.PlayerHp:
                    v1241.PlayerHP playerHp = packet as v1241.PlayerHP;
                    playerHp.PlayerID = CheckPlayerID(playerHp.PlayerID, fromPlayerId, toPlayerId);
                    return playerHp.ToArray();
                    break;
                case (byte) PacketTypes.NpcUpdate:
                    v1241.NPCUpdate npcUpdate = packet as v1241.NPCUpdate;
                    npcUpdate.Target = CheckPlayerID(npcUpdate.Target, fromPlayerId, toPlayerId);
                    return npcUpdate.ToArray();
                    break;
                case (byte) PacketTypes.NpcItemStrike:
                    v1308.NPCItemStrike npcItemStrike = packet as v1308.NPCItemStrike;
                    npcItemStrike.PlayerID = CheckPlayerID(npcItemStrike.PlayerID, fromPlayerId, toPlayerId);
                    return npcItemStrike.ToArray();
                    break;
                case (byte) PacketTypes.ChatText:
                    v1308.ChatMessage chatText = packet as v1308.ChatMessage;
                    chatText.PlayerID = CheckPlayerID(chatText.PlayerID, fromPlayerId, toPlayerId);
                    return chatText.ToArray();
                    break;
                case (byte) PacketTypes.PlayerDamage:
                    v1308.PlayerDamage playerDamage = packet as v1308.PlayerDamage;
                    playerDamage.PlayerID = CheckPlayerID(playerDamage.PlayerID, fromPlayerId, toPlayerId);
                    return playerDamage.ToArray();
                    break;
                case (byte) PacketTypes.ProjectileDestroy:
                    v1308.ProjectileDestroy projectileDestroy = packet as v1308.ProjectileDestroy;
                    projectileDestroy.Owner = CheckPlayerID(projectileDestroy.Owner, fromPlayerId, toPlayerId);
                    return projectileDestroy.ToArray();
                    break;
                case (byte) PacketTypes.TogglePvp:
                    v1308.TogglePvp togglePvp = packet as v1308.TogglePvp;
                    togglePvp.PlayerID = CheckPlayerID(togglePvp.PlayerID, fromPlayerId, toPlayerId);
                    return togglePvp.ToArray();
                    break;
                case (byte) PacketTypes.EffectHeal:
                    v1308.EffectHeal effectHeal = packet as v1308.EffectHeal;
                    effectHeal.PlayerID = CheckPlayerID(effectHeal.PlayerID, fromPlayerId, toPlayerId);
                    return effectHeal.ToArray();
                    break;
                case (byte) PacketTypes.Zones:
                    v1308.Zones zones = packet as v1308.Zones;
                    zones.PlayerID = CheckPlayerID(zones.PlayerID, fromPlayerId, toPlayerId);
                    return zones.ToArray();
                    break;
                case (byte) PacketTypes.NpcTalk:
                    v1308.NpcTalk npcTalk = packet as v1308.NpcTalk;
                    npcTalk.PlayerID = CheckPlayerID(npcTalk.PlayerID, fromPlayerId, toPlayerId);
                    return npcTalk.ToArray();
                case (byte) PacketTypes.PlayerAnimation:
                    v1308.PlayerAnimation playerAnimation = packet as v1308.PlayerAnimation;
                    playerAnimation.PlayerID = CheckPlayerID(playerAnimation.PlayerID, fromPlayerId, toPlayerId);
                    return playerAnimation.ToArray();
                case (byte) PacketTypes.PlayerMana:
                    v1241.PlayerMana playerMana = packet as v1241.PlayerMana;
                    playerMana.PlayerID = CheckPlayerID(playerMana.PlayerID, fromPlayerId, toPlayerId);
                    return playerMana.ToArray();
                case (byte) PacketTypes.EffectMana:
                    v1308.EffectMana effectMana = packet as v1308.EffectMana;
                    effectMana.PlayerID = CheckPlayerID(playerMana.PlayerID, fromPlayerId, toPlayerId);
                    return effectMana.ToArray();
                case (byte) PacketTypes.PlayerKillMe:
                    v1308.PlayerKillMe playerKillMe = packet as v1308.PlayerKillMe;
                    playerKillMe.PlayerID = CheckPlayerID(playerKillMe.PlayerID, fromPlayerId, toPlayerId);
                    return playerKillMe.ToArray();
                case (byte) PacketTypes.PlayerTeam:
                    v1241.PlayerTeam playerTeam = packet as v1241.PlayerTeam;
                    playerTeam.PlayerID = CheckPlayerID(playerTeam.PlayerID, fromPlayerId, toPlayerId);
                    return playerTeam.ToArray();
                case (byte) PacketTypes.SignRead:
                    v1308.SignRead signRead = packet as v1308.SignRead;
                    signRead.PlayerID = CheckPlayerID(signRead.PlayerID, fromPlayerId, toPlayerId);
                    return signRead.ToArray();
                case (byte) PacketTypes.PlayerBuff:
                    v1241.PlayerBuffs playerBuff = packet as v1241.PlayerBuffs;
                    playerBuff.PlayerID = CheckPlayerID(playerBuff.PlayerID, fromPlayerId, toPlayerId);
                    return playerBuff.ToArray();
                case (byte) PacketTypes.NpcSpecial:
                    v1308.NpcSpecial npcSpecial = packet as v1308.NpcSpecial;
                    npcSpecial.PlayerID = CheckPlayerID(npcSpecial.PlayerID, fromPlayerId, toPlayerId);
                    return npcSpecial.ToArray();
                case (byte) PacketTypes.PlayerAddBuff:
                    v1241.PlayerAddBuff playerAddBuff = packet as v1241.PlayerAddBuff;
                    playerAddBuff.PlayerID = CheckPlayerID(playerAddBuff.PlayerID, fromPlayerId, toPlayerId);
                    return playerAddBuff.ToArray();
                case (byte) PacketTypes.PlayHarp:
                    v1308.PlayMusicItem playMusicItem = packet as v1308.PlayMusicItem;
                    playMusicItem.PlayerID = CheckPlayerID(playMusicItem.PlayerID, fromPlayerId, toPlayerId);
                    return playMusicItem.ToArray();
                case (byte) PacketTypes.SpawnBossorInvasion:
                    v1308.
                    break;
                case (byte) PacketTypes.PlayerDodge:
                    break;
                case (byte) PacketTypes.Teleport:
                    break;
                case (byte) PacketTypes.PlayerHealOther:
                    break;
                case (byte) PacketTypes.NumberOfAnglerQuestsCompleted:
                    break;
                case (byte) PacketTypes.SyncPlayerChestIndex:
                    break;
                case (byte) PacketTypes.SetPlayerStealth:
                    break;
                case (byte) PacketTypes.PlayerTeleportThroughPortal:
                    break;
                case (byte) PacketTypes.UpdateMinionTarget:
                    break;
                case (byte) PacketTypes.NebulaLevelUpRequest:
                    break;
                default:
                    break;
            }
        }

        private byte CheckPlayerID(byte curPlayerID, byte fromPlayerID, byte toPlayerID)
        {
            if (curPlayerID == fromPlayerID)
            {
                return toPlayerID;
            }
            else
            {
                return curPlayerID;
            }
        }
    }
}

