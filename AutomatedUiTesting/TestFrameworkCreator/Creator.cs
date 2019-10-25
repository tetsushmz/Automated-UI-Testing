using Interfaces;
using TestFramework;

namespace TestFrameworkCreator
{
    public static class Creator
    {
        public static IVisualStudioApp CreateVisualStudioApp()
        {
            var result = new VisualStudioApp();
            return result;
        }
    }
}
