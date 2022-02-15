using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Antpire.Drawing {
    internal class PolygonRenderable : IRenderable {
        private Polygon polygon;
        private Rectangle boundingBox;

        public Polygon Polygon { 
            get => polygon; 
            set {
                polygon = value;
                boundingBox = Polygon.BoundingRectangle.ToRectangle();
            }
        }
        public Color Color = Color.White;
        public float Thickness = 1.0f;

        public void Render(SpriteBatch spriteBatch, Transform2 trans, Rectangle viewRegion) {
            if(viewRegion.Intersects(new Rectangle { Location = boundingBox.Location + trans.Position.ToPoint(), Size = boundingBox.Size }))
                spriteBatch.DrawPolygon(trans.Position, Polygon, Color, Thickness);
        }
    }
}
