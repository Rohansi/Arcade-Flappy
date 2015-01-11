using GameAPI;
using GameAPI.BudgetBoy;

namespace Games.Flappy
{
    public class Swatches
    {
        public readonly SwatchIndex ClearColor;
        public readonly SwatchIndex Text;

        public readonly SwatchIndex Player1;
        public readonly SwatchIndex Player2;

        public readonly SwatchIndex Pipe;

        public readonly SwatchIndex Ground;


        public Swatches(PaletteBuilder palette)
        {
            ClearColor = palette.Add(0x93CCEA, 0x93CCEA, 0x93CCEA);
            Text = palette.Add(0x000000, 0x000000, 0x000000);

            Player1 = palette.Add(0x424242, 0xFFFFFF, 0xFF6650);
            Player2 = palette.Add(0xFF0000, 0xFFC000, 0xFFF000);

            Pipe = palette.Add(0x424242, 0x73BF2E, 0x5F9926);

            Ground = palette.Add(0x424242, 0x73BF2E, 0xE6DD9A, 0x424242);
        }
    }
}
