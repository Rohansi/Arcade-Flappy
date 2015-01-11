using System;
using GameAPI;
using GameAPI.BudgetBoy;
using ResourceLibrary;

namespace Games.Flappy
{
    [GameInfo(
        Title = "Flappy",
        AuthorName = "Rohan",
        AuthorContact = "steamcommunity.com/id/rohans",
        UpdateRate = 60
    )]
    [GraphicsInfo(Width = 256, Height = 240)]
    public class Main : Game
    {
        public Swatches Swatches { get; private set; }

        protected override void OnReset()
        {
            try
            {
                SetStage(new GameStage(this));
            }
            catch (Exception e)
            {
                Debug.Error("OnReset: " + e.ToString());
            }
        }

        protected override void OnLoadPalette(PaletteBuilder builder)
        {
            Swatches = new Swatches(builder);
        }
    }
}
