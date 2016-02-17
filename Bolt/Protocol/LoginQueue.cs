//
//  AuthenticationHeader.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using LibMultiplicity;
using LibMultiplicity.Packets.v1302;
using LibMultiplicity.Packets.v1241;
using System;
using System.Collections.Generic;

namespace Bolt.Protocol
{
    public class LoginQueue
    {
        public LibMultiplicity.Packets.v1302.PlayerInfo playerInfo;
        public LibMultiplicity.Packets.v1241.PlayerHP playerHP;
        public LibMultiplicity.Packets.v1241.PlayerMana playerMana;
        public LibMultiplicity.Packets.v1241.PlayerBuffs playerBuffs;
        public List<LibMultiplicity.Packets.v1241.PlayerInventorySlot> playerSlot = new List<PlayerInventorySlot>();

        public LoginQueue()
        {
        }
    }
}

