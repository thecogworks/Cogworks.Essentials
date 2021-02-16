using System.Text.RegularExpressions;
using Cogworks.Essentials.Constants.StringConstants;
using Cogworks.Essentials.Extensions;

namespace Cogworks.Essentials.Helpers
{
    public static class VideoHelper
    {
        public static string GetVideoEmbeddedUrl(string url)
        {
            if (!url.HasValue())
            {
                return string.Empty;
            }

            var youTubeMatch = Regex.Match(url, SocialMediaConstants.RegexConstants.YouTube);

            if (youTubeMatch.Success)
            {
                var youTubeId = youTubeMatch.Groups[1].Value;
                return $"{SocialMediaConstants.YouTubeEmbedUrl}{youTubeId}?rel=0";
            }

            var vimeoMatch = Regex.Match(url, SocialMediaConstants.RegexConstants.Vimeo);

            if (vimeoMatch.Success)
            {
                var vimeoId = vimeoMatch.Groups[4].Value;
                return $"{SocialMediaConstants.VimeoEmbedUrl}{vimeoId}";
            }

            return string.Empty;
        }
    }
}