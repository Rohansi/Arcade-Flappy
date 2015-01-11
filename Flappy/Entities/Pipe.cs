using System;
using System.Collections;
using GameAPI;
using GameAPI.BudgetBoy;

namespace Games.Flappy
{
    public class Pipe : Entity
    {
        public new Main Game
        {
            get { return (Main)base.Game; }
        }

        private Sprite _sprite;

        public Pipe()
        {

        }

        public bool IsFlipped
        {
            get { return _sprite.FlipY; }
            set { _sprite.FlipY = value; }
        }

        // true if the player got a point from this pipe
        public bool IsCleared { get; set; }

        protected override void OnLoadGraphics(Graphics graphics)
        {
            var image = graphics.GetImage("Resources", "pipe");

            if (_sprite == null)
            {
                _sprite = Add(new Sprite(image, Game.Swatches.Pipe), 0);
            }
            else
            {
                _sprite.Image = image;
            }
        }
    }
}
