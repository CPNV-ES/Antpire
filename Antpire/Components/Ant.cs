using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;

namespace Antpire.Components {
    internal class Ant {

        public enum State {
            Idle,
            Attacking,
            Scouting,
            Dying
        }   

        public State actualState = State.Scouting;
        public State oldState;
        public int limit = 1;
        public int counter = 0;
        public float countDuration = 2f; //every  2s.
        internal float currentTime;
    }
}
