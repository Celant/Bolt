//
//  AuthenticationHeader.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Multiplicity.Packets;
using System;
using System.Collections.Generic;

namespace Bolt.Protocol
{
    public class LoginQueue
    {
        public PlayerInfo playerInfo;
        public PlayerHP playerHP;
        public PlayerMana playerMana;
        public AddPlayerBuff playerBuffs;
        public List<PlayerInventorySlot> playerSlot = new List<PlayerInventorySlot>();

        public LoginQueue()
        {
        }
    }
}

