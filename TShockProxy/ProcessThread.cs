//
//  ProcessThread.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using System;

namespace TShockProxy
{
    public abstract class ProcessThread
    {
        private bool _interrupted = false;

        public bool Interrupted() 
        {
            return _interrupted;
        }

        public void Interrupt()
        {
            if (!Interrupted())
            {
                _interrupted = true;
            }
        }

        public virtual void Run()
        {

        }
    }
}

