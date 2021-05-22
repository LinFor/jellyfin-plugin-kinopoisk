using System.Runtime.Serialization;

namespace Jellyfin.Plugin.Kinopoisk.Api.Model
{
    public class StaffItem
    {
        public int StaffId { get; set; }
        public string NameRu { get; set; }
        public string NameEn { get; set; }
        public string Description { get; set; }
        public string PosterUrl { get; set; }
        public string ProfessionText { get; set; }
        public ProfessionEnum ProfessionKey { get; set; }
    }

    [DataContract]
    public enum ProfessionEnum
    {
        [EnumMember(Value = "UNKNOWN")]
        Unknown,

        [EnumMember(Value = "WRITER")]
        Writer,

        [EnumMember(Value = "OPERATOR")]
        Operator,

        [EnumMember(Value = "EDITOR")]
        Editor,

        [EnumMember(Value = "COMPOSER")]
        Composer,

        [EnumMember(Value = "PRODUCER_USSR")]
        ProducerUssr,

        [EnumMember(Value = "TRANSLATOR")]
        Translator,

        [EnumMember(Value = "DIRECTOR")]
        Director,

        [EnumMember(Value = "DESIGN")]
        Design,

        [EnumMember(Value = "PRODUCER")]
        Producer,

        [EnumMember(Value = "ACTOR")]
        Actor,

        [EnumMember(Value = "VOICE_DIRECTOR")]
        VoiceDirector
    }
}
