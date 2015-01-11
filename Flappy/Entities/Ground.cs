using System;
using System.Collections;
using GameAPI;
using GameAPI.BudgetBoy;

namespace Games.Flappy
{
    public class Ground : Entity
    {
        public new Main Game
        {
            get { return (Main)base.Game; }
        }

        private Sprite _sprite;

        public Ground()
        {

        }

        protected override void OnLoadGraphics(Graphics graphics)
        {
            var image = graphics.GetImage("Resources", "ground");

            if (_sprite == null)
            {
                _sprite = Add(new Sprite(image, Game.Swatches.Ground), 0);
            }
            else
            {
                _sprite.Image = image;
            }
        }
    }
}
