using GameAPI;
using GameAPI.BudgetBoy;

namespace Games.Flappy
{
    public static class CoroutineContainerExtensions
    {
        // completed when any of the contained awaitables completes
        public static Awaitable WhenAny(this CoroutineContainer container, params Awaitable[] inner)
        {
            return new WhenAnyAwaitable(inner);
        }

        private class WhenAnyAwaitable : Awaitable
        {
            private readonly Awaitable[] _inner;
            private bool _isComplete;

            public override bool IsComplete { get { return _isComplete; } }

            public WhenAnyAwaitable(params Awaitable[] inner)
            {
                _inner = inner;
            }

            protected override void OnUpdate()
            {
                foreach (var a in _inner)
                {
                    a.Update();

                    if (a.IsComplete)
                    _isComplete = true;
                }
            }
        }
    }
}
