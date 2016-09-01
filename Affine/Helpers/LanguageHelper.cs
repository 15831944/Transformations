namespace Seo.Helpers
{
    using System.Globalization;

    internal static class LanguageHelper
    {
        public static void SetLanguage(string name)
        {
            switch (name)
            {
                case "bg-BG":
                    Properties.Resources.Culture = new CultureInfo("bg-BG");

                    break;
                default:
                    Properties.Resources.Culture = new CultureInfo("en-US");

                    break;
            }
        }
    }
}
