using System;
using System.Text.RegularExpressions;
using Cogworks.Essentials.Constants.StringConstants;
using Cogworks.Essentials.Enums;
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

            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("Not a valid URL");
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

        public static VideoServiceType GetVideoServiceType(string url, out string videoId)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("Not a valid URL");
            }

            videoId = string.Empty;

            var youTubeMatch = Regex.Match(url, SocialMediaConstants.RegexConstants.YouTube);

            if (youTubeMatch.Success && youTubeMatch.Groups.Count > 1)
            {
                videoId = youTubeMatch.Groups[1].Value;
                return VideoServiceType.YouTube;
            }

            var vimeoMatch = Regex.Match(url, SocialMediaConstants.RegexConstants.Vimeo);

            if (vimeoMatch.Success && vimeoMatch.Groups.Count > 1)
            {
                videoId = vimeoMatch.Groups[1].Value;
                return VideoServiceType.Vimeo;
            }

            return VideoServiceType.Unknown;
        }
    }
}