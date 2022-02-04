using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;

namespace Antpire.Components {
    internal class Insect {
        public bool shouldChangeDestination = false;
        public Vector2 velocity = new Vector2();
        public Vector2 destination = new Vector2();

        // States
        public enum states
        {
            Idle,
            Attacking,
            Walking,
            Scouting
        }

        public states state = states.Idle;

        public void changeDestinationTo(Vector2 d)
        {
            this.destination = d;
            this.shouldChangeDestination = true;
        }
    }
}
