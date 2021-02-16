namespace Cogworks.Essentials.Constants.StringConstants
{
    public static class SocialMediaConstants
    {
        public const string YouTubeEmbedUrl = "https://www.youtube.com/embed/";
        public const string VimeoEmbedUrl = "https://player.vimeo.com/video/";

        public static class RegexConstants
        {
            public const string YouTube = @"(?:youtube(?:-nocookie)?\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})";
            public const string Vimeo = @"(http|https)?:\/\/(www\.|player\.)?vimeo.com\/(?:channels\/(?:\w+\/)?|groups\/([^\/]*)\/videos\/|video\/|)(\d+)(?:|\/\?)";
        }
    }
}