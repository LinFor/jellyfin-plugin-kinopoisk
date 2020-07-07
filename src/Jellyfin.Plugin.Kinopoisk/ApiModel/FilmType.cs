using System.Runtime.Serialization;

namespace Jellyfin.Plugin.Kinopoisk.ApiModel
{
    [DataContract]
    public enum FilmType
    {
        [EnumMember(Value = "UNKNOWN")]
        Unknown,

        [EnumMember(Value = "FILM")]
        Film,

        [EnumMember(Value = "TV_SHOW")]
        TvShow
    }
}
