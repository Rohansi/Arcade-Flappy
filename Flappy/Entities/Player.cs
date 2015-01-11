using System;
using System.Collections;
using System.Linq;
using GameAPI;
using GameAPI.BudgetBoy;

namespace Games.Flappy
{
    public class Player : Entity
    {
        public new Main Game
        {
            get { return (Main)base.Game; }
        }

        private Sprite _sprite1;
        private Sprite _sprite2;

        private Vector2f _velocity;

        public Player()
        {
            _velocity = new Vector2f(100, 0);
        }

        public bool IsDead { get; private set; }

        protected override void OnLoadGraphics(Graphics graphics)
        {
            var image1 = graphics.GetImage("Resources", "player_1");
            var image2 = graphics.GetImage("Resources", "player_2");

            if (_sprite1 == null)
            {
                _sprite1 = new Sprite(image1, Game.Swatches.Player1);
                _sprite2 = new Sprite(image2, Game.Swatches.Player2);
            }
            else
            {
                _sprite1.Image = image1;
                _sprite2.Image = image2;
            }

            // need to set LocalBounds because we draw our own sprites
            var size = (Vector2f)(_sprite1.Size * 0.75f);
            LocalBounds = new RectF(-(size / 2), size);
        }

        protected override void OnUpdate(double dt)
        {
            base.OnUpdate(dt);

            // gravity
            _velocity.Y += -400 * (float)dt;
            if (_velocity.Y < -225)
                _velocity.Y = -225;

            if (!IsDead)
            {
                if (Stage.Controls.A.JustPressed)
                    _velocity.Y = 150;

                var oldPosition = Position;
                Position += _velocity * (float)dt;

                var solidEntities = Stage.GetEntities(Bounds);

                // check if we collided with another entity
                var collided = solidEntities
                    .Where(e => !ReferenceEquals(e, this))
                    .FirstOrDefault(e => e.Bounds.Intersects(Bounds));

                // or if we hit the top
                var hitTop = Position.Y > (Stage.Graphics.Bounds.Top + 8);

                if (collided != null || hitTop)
                {
                    IsDead = true;

                    // stop moving right
                    _velocity.X = 0;

                    // stop moving up
                    if (_velocity.Y > 0)
                        _velocity.Y = 0;

                    if (collided is Pipe || hitTop)
                    {
                        // nosedive
                        _sprite1.Rotation = 3;
                        _sprite2.Rotation = 3;
                    }
                    else
                    {
                        // for some reason hitting the ground leaves us deep into it
                        // so we use the previous position to offset it
                        Position = oldPosition;
                    }
                }
            }
            else
            {
                var hitGround = Stage.GetEntities<Ground>()
                    .Any(g => g.Bounds.Intersects(Bounds));

                // when we hit the ground we stop doing stuff
                if (hitGround)
                    return;

                Position += _velocity * (float)dt;
            }
        }

        protected override void OnRender(Graphics graphics)
        {
            base.OnRender(graphics);

            CenterSprite(_sprite1);
            CenterSprite(_sprite2);

            _sprite1.Render(graphics);
            _sprite2.Render(graphics);

            //graphics.FillRect(Game.Swatches.Player1, (RectI)Bounds);
        }

        private void CenterSprite(Sprite sprite)
        {
            var rotation = sprite.Rotation;
            Vector2i size;

            if (rotation == 0 || rotation == 2) // horizontal
            {
                size = sprite.Size;
            }
            else // vertical
            {
                size = new Vector2i(sprite.Height, sprite.Width);
            }

            sprite.Position = (Vector2i)Position - (size / 2);
        }
    }
}
