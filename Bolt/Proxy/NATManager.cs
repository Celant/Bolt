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
        public byte ProxyPlayerID;
        public byte ServerPlayerID;

        public NATManager(byte proxyPlayerID) : this(proxyPlayerID, 0)
        {
        }

        public NATManager(byte proxyPlayerID, byte serverPlayerID)
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
            byte[] translatedPacket = TranslatePacket(packet, ServerPlayerID, ProxyPlayerID);
            if (translatedPacket.Length <= 0)
            {
                return new byte[0];
            }
            if (translatedPacket != packet.ToArray())
            {
                //Console.WriteLine("[Bolt] Translated packet {0}", packet.ID);
                //Console.WriteLine("[Bolt] From ID: {0}, to ID: {1}", ServerPlayerID, ProxyPlayerID);
            }
            else
            {
                //Console.WriteLine("[Bolt] Packet ID: {0} did not need translating", packet.ID);
            }
            return translatedPacket;
        }

        private byte[] TranslatePacketDownstream(TerrariaPacket packet)
        {
            byte[] translatedPacket = TranslatePacket(packet, ProxyPlayerID, ServerPlayerID);
            if (translatedPacket.Length <= 0)
            {
                return new byte[0];
            }
            if (translatedPacket != packet.ToArray())
            {
                Console.WriteLine("[Bolt] Translated packet {0}", packet.ID);
                Console.WriteLine("[Bolt] From ID: {0}, to ID: {1}", ProxyPlayerID, ServerPlayerID);
            }
            else
            {
                Console.WriteLine("[Bolt] Packet ID: {0} did not need translating", packet.ID);
            }
            return translatedPacket;
        }

        private byte[] TranslatePacket(TerrariaPacket packet, byte fromPlayerId, byte toPlayerId)
        {
            switch (packet.ID)
            {
                case (byte) PacketTypes.PlayerInfo:
                    v1302.PlayerInfo playerInfo = packet as v1302.PlayerInfo;
                    playerInfo.PlayerID = CheckPlayerID(playerInfo.PlayerID, fromPlayerId, toPlayerId);
                    return playerInfo.ToArray();
                case (byte) PacketTypes.PlayerSlot:
                    v1241.PlayerInventorySlot playerSlot = packet as v1241.PlayerInventorySlot;
                    playerSlot.PlayerID = CheckPlayerID(playerSlot.PlayerID, fromPlayerId, toPlayerId);
                    return playerSlot.ToArray();
                case (byte) PacketTypes.PlayerSpawn:
                    v1302.SpawnPlayer playerSpawn = packet as v1302.SpawnPlayer;
                    playerSpawn.PlayerID = CheckPlayerID(playerSpawn.PlayerID, fromPlayerId, toPlayerId);
                    return playerSpawn.ToArray();
                case (byte) PacketTypes.PlayerUpdate:
                    v1302.UpdatePlayer playerUpdate = packet as v1302.UpdatePlayer;
                    playerUpdate.PlayerID = CheckPlayerID(playerUpdate.PlayerID, fromPlayerId, toPlayerId);
                    return playerUpdate.ToArray();
                case (byte) PacketTypes.PlayerActive:
                    v1241.PlayerActive playerActive = packet as v1241.PlayerActive;
                    playerActive.PlayerID = CheckPlayerID(playerActive.PlayerID, fromPlayerId, toPlayerId);
                    return playerActive.ToArray();
                case (byte) PacketTypes.PlayerHp:
                    v1241.PlayerHP playerHp = packet as v1241.PlayerHP;
                    playerHp.PlayerID = CheckPlayerID(playerHp.PlayerID, fromPlayerId, toPlayerId);
                    return playerHp.ToArray();
                case (byte) PacketTypes.NpcUpdate:
                    v1241.NPCUpdate npcUpdate = packet as v1241.NPCUpdate;
                    npcUpdate.Target = CheckPlayerID(npcUpdate.Target, fromPlayerId, toPlayerId);
                    return npcUpdate.ToArray();
                case (byte) PacketTypes.NpcItemStrike:
                    v1308.NPCItemStrike npcItemStrike = packet as v1308.NPCItemStrike;
                    npcItemStrike.PlayerID = CheckPlayerID(npcItemStrike.PlayerID, fromPlayerId, toPlayerId);
                    return npcItemStrike.ToArray();
                case (byte) PacketTypes.ChatText:
                    v1308.ChatMessage chatText = packet as v1308.ChatMessage;
                    chatText.PlayerID = CheckPlayerID(chatText.PlayerID, fromPlayerId, toPlayerId);
                    return chatText.ToArray();
                case (byte) PacketTypes.PlayerDamage:
                    v1308.PlayerDamage playerDamage = packet as v1308.PlayerDamage;
                    playerDamage.PlayerID = CheckPlayerID(playerDamage.PlayerID, fromPlayerId, toPlayerId);
                    return playerDamage.ToArray();
                case (byte) PacketTypes.ProjectileDestroy:
                    v1308.ProjectileDestroy projectileDestroy = packet as v1308.ProjectileDestroy;
                    projectileDestroy.Owner = CheckPlayerID(projectileDestroy.Owner, fromPlayerId, toPlayerId);
                    return projectileDestroy.ToArray();
                case (byte) PacketTypes.TogglePvp:
                    v1308.TogglePvp togglePvp = packet as v1308.TogglePvp;
                    togglePvp.PlayerID = CheckPlayerID(togglePvp.PlayerID, fromPlayerId, toPlayerId);
                    return togglePvp.ToArray();
                case (byte) PacketTypes.EffectHeal:
                    v1308.EffectHeal effectHeal = packet as v1308.EffectHeal;
                    effectHeal.PlayerID = CheckPlayerID(effectHeal.PlayerID, fromPlayerId, toPlayerId);
                    return effectHeal.ToArray();
                case (byte) PacketTypes.Zones:
                    v1308.Zones zones = packet as v1308.Zones;
                    zones.PlayerID = CheckPlayerID(zones.PlayerID, fromPlayerId, toPlayerId);
                    return zones.ToArray();
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
                    effectMana.PlayerID = CheckPlayerID(effectMana.PlayerID, fromPlayerId, toPlayerId);
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
                    // For whatever stupid reason, this packet's PlayerID is a short not a byte. Casts appropriately.
                    v1308.SpawnBossOrInvasion spawnBossOrInvasion = packet as v1308.SpawnBossOrInvasion;
                    spawnBossOrInvasion.PlayerID = (short)CheckPlayerID((byte)spawnBossOrInvasion.PlayerID, fromPlayerId, toPlayerId);
                    return spawnBossOrInvasion.ToArray();
                case (byte) PacketTypes.PlayerDodge:
                    v1308.PlayerDodge playerDodge = packet as v1308.PlayerDodge;
                    playerDodge.PlayerID = CheckPlayerID(playerDodge.PlayerID, fromPlayerId, toPlayerId);
                    return playerDodge.ToArray();
                case (byte) PacketTypes.Teleport:
                    v1308.GenericTeleport genericTeleport = packet as v1308.GenericTeleport;
                    // Again for whatever stupid reason, this is a short too. Also casts appropriately.
                    genericTeleport.TargetID = (short) CheckPlayerID((byte) genericTeleport.TargetID, fromPlayerId, toPlayerId);
                    return genericTeleport.ToArray();
                case (byte) PacketTypes.PlayerHealOther:
                    v1308.HealOtherPlayer healOther = packet as v1308.HealOtherPlayer;
                    healOther.PlayerID = CheckPlayerID(healOther.PlayerID, fromPlayerId, toPlayerId);
                    return healOther.ToArray();
                case (byte) PacketTypes.NumberOfAnglerQuestsCompleted:
                    v1308.NumberOfAnglerQuestsCompleted questsCompleted = packet as v1308.NumberOfAnglerQuestsCompleted;
                    questsCompleted.PlayerID = CheckPlayerID(questsCompleted.PlayerID, fromPlayerId, toPlayerId);
                    return questsCompleted.ToArray();
                case (byte) PacketTypes.SyncPlayerChestIndex:
                    v1308.SyncPlayerChestIndex syncPlayerChest = packet as v1308.SyncPlayerChestIndex;
                    syncPlayerChest.PlayerID = CheckPlayerID(syncPlayerChest.PlayerID, fromPlayerId, toPlayerId);
                    return syncPlayerChest.ToArray();
                case (byte) PacketTypes.SetPlayerStealth:
                    v1308.SetPlayerStealth setPlayerStealth = packet as v1308.SetPlayerStealth;
                    setPlayerStealth.PlayerID = CheckPlayerID(setPlayerStealth.PlayerID, fromPlayerId, toPlayerId);
                    return setPlayerStealth.ToArray();
                case (byte) PacketTypes.PlayerTeleportThroughPortal:
                    v1308.PlayerTeleportThroughPortal playerTeleportThroughPortal = packet as v1308.PlayerTeleportThroughPortal;
                    playerTeleportThroughPortal.PlayerID = CheckPlayerID(playerTeleportThroughPortal.PlayerID, fromPlayerId, toPlayerId);
                    return playerTeleportThroughPortal.ToArray();
                case (byte) PacketTypes.UpdateMinionTarget:
                    v1308.UpdateMinionTarget updateMinionTarget = packet as v1308.UpdateMinionTarget;
                    updateMinionTarget.PlayerID = CheckPlayerID(updateMinionTarget.PlayerID, fromPlayerId, toPlayerId);
                    return updateMinionTarget.ToArray();
                case (byte) PacketTypes.NebulaLevelUpRequest:
                    v1308.NebulaLevelUpRequest nebulaLevelUpRequest = packet as v1308.NebulaLevelUpRequest;
                    nebulaLevelUpRequest.PlayerID = CheckPlayerID(nebulaLevelUpRequest.PlayerID, fromPlayerId, toPlayerId);
                    return nebulaLevelUpRequest.ToArray();
                default:
                    return packet.ToArray();
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

