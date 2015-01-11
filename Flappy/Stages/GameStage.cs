using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameAPI;
using GameAPI.BudgetBoy;

namespace Games.Flappy
{
    public class GameStage : Stage
    {
        public new Main Game
        {
            get { return (Main)base.Game; }
        }

        private Ground _ground1;
        private Ground _ground2;

        private Player _player;
        private int _score;

        private Text _title;
        private Text _titleMessage;

        private Text _gameOver;

        private Text _currentScore;

        public GameStage(Main game)
          : base(game)
        {
            var font = Graphics.GetImage("Resources", "font");

            _title = Add(new Text(font, Game.Swatches.Text), 1000);
            _title.IsVisible = false;
            _title.Value = "Flappy";

            _titleMessage = Add(new Text(font, Game.Swatches.Text), 1000);
            _titleMessage.IsVisible = false;
            _titleMessage.Value = "Press A to Flap";

            _gameOver = Add(new Text(font, Game.Swatches.Text), 1000);
            _gameOver.IsVisible = false;
            _gameOver.Value = "Game Over!";

            _currentScore = Add(new Text(font, Game.Swatches.Text), 1000);
            _currentScore.IsVisible = false;
        }

        private IEnumerator GameCoro()
        {
            while (true)
            {
                _score = 0;

                // spawn player
                _player = Add(new Player(), 50);
                _player.Position = new Vector2f(Graphics.Bounds.Width, Graphics.Bounds.Height / 2);

                // freeze player
                _player.IsActive = false;

                // show the title screen
                _title.IsVisible = true;
                _titleMessage.IsVisible = true;

                // wait for A to be pressed
                yield return Until(() => Controls.A.JustPressed);

                // hide the title screen
                _title.IsVisible = false;
                _titleMessage.IsVisible = false;

                // clear JustPressed so we dont immediately flap
                yield return null;

                // show the score
                _currentScore.IsVisible = true;

                // unfreeze player
                _player.IsActive = true;

                // start spawning pipes
                StartCoroutine(PipeSpawnCoro(_player));

                // wait until the player loses
                while (!_player.IsDead)
                {
                    // count the pipes the player passed
                    var clearedPipes = GetEntities<Pipe>()
                        .Where(p => !p.IsFlipped && !p.IsCleared)
                        .Where(p => _player.Bounds.Left > p.Bounds.Right);

                    foreach (var p in clearedPipes)
                    {
                        p.IsCleared = true;
                        _score++;
                    }

                    yield return null;
                }

                // show game over
                _gameOver.IsVisible = true;

                // wait a bit, but allow the user to skip it after half a second
                // need "this." for WhenAny because its an ext method
                yield return Delay(0.5);
                yield return this.WhenAny(Delay(2.0), Until(() => Controls.A.JustPressed));

                // remove everything but the ground
                foreach (var e in GetEntities().Where(e => !(e is Ground)))
                {
                    Remove(e);
                }

                // hide score and gmae over
                _currentScore.IsVisible = false;
                _gameOver.IsVisible = false;
            }
        }

        private IEnumerator PipeSpawnCoro(Player player)
        {
            var random = new Random();

            // wait a bit before spawning the first one
            yield return Delay(0.2);

            while (!player.IsDead)
            {
                var offscreenX = Graphics.Bounds.Right;

                var minGapSize = (int)player.Bounds.Height * 5;
                var maxGapSize = (int)player.Bounds.Height * 7;
                var gapSize = random.Next(minGapSize, maxGapSize);

                var minGapOffset = Graphics.Height / 4;
                var maxGapOffset = minGapOffset * 2;
                var gapOffset = Graphics.Bounds.Top - random.Next(minGapOffset, maxGapOffset);

                var bottomPipe = Add(new Pipe(), 1);
                bottomPipe.Position = new Vector2f(offscreenX, gapOffset - gapSize - bottomPipe.Bounds.Height);

                var topPipe = Add(new Pipe(), 1);
                topPipe.IsFlipped = true;
                topPipe.Position = new Vector2f(offscreenX, gapOffset);

                yield return Delay(1.0 + (random.NextDouble() * 0.25));
            }
        }

        protected override void OnEnter()
        {
            Debug.Log("GameStage entered");
            Graphics.SetClearColor(Game.Swatches.ClearColor);

            StartCoroutine(GameCoro);

            _ground1 = Add(new Ground(), 100);
            _ground2 = Add(new Ground(), 100);
        }

        protected override void OnUpdate()
        {
            // remove old pipes
            var cutoff = Graphics.Bounds.Left - 64;

            var oldPipes = GetEntities<Pipe>()
                .Where(p => p.Position.X <= cutoff);

            foreach (var pipe in oldPipes)
            {
                Remove(pipe);
            }

            base.OnUpdate();
        }

        protected override void OnRender()
        {
            // move camera
            Graphics.Center = new Vector2i((int)_player.Position.X, Graphics.Height / 2);

            // align text
            _title.Position = Graphics.Center - (_title.Size / 2) + new Vector2i(0, Graphics.Bounds.Height / 4);
            _titleMessage.Position = Graphics.Center - (_titleMessage.Size / 2) - new Vector2i(0, Graphics.Bounds.Height / 4);

            _gameOver.Position = Graphics.Center - (_gameOver.Size / 2) + new Vector2i(0, Graphics.Bounds.Height / 4);

            _currentScore.Position = Graphics.Bounds.TopRight - _currentScore.Size - new Vector2i(8, 8);

            // update score text
            if (_currentScore.IsVisible)
                _currentScore.Value = string.Format("Score: {0}", _score);

            // align grounds
            var size = (int)_ground1.Bounds.Width;
            var startIdx = Graphics.Bounds.Left / size;
            var start = startIdx * size;

            _ground1.Position = new Vector2f(start, 0);
            _ground2.Position = new Vector2f(start + size, 0);

            base.OnRender();
        }
    }
}
