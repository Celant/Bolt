//
//  IPlayer.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using System;

namespace TShockProxy.Connection
{
    public interface IPlayer : IConnection
    {
        void SendData(byte[] data);
    }
}

