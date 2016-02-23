//
//  KickException.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using System;

namespace Bolt.Exception
{
    public class KickException : ApplicationException
    {
        public KickException(string message) : base(message)
        {
        }
    }
}

