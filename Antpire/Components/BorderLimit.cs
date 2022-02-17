using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Antpire.Components {
    internal class BorderLimit {
        private int xleft;
        private int xright;
        private int yup;
        private int ydown;

        public BorderLimit(int xl, int xr, int yu, int yd) {
            this.xleft = xl;
            this.xright = xr;
            this.yup = yu;
            this.ydown = yd;
        }
    }
}
