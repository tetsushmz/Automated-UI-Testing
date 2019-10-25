using Interfaces;
using TestFramework;

namespace Utility
{
    public static class VisualStudioAppCreator
    {
        public static IVisualStudioApp CreateVisualStudioApp()
        {
            var result = new VisualStudioApp();
            return result;
        }
    }
}
