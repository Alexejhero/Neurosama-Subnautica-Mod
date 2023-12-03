using System.Linq;

namespace SCHIZO.Subtitles;

partial class SubtitlesData
{
    protected override void Register()
    {
        foreach ((SubtitleLine line, int i) in lines.Select((l,i) => (l,i)))
        {
            line.index = i;
            line.subtitlesKey = key;
        }
        SubtitlesHandler.Register(this);
    }

    partial class SubtitleLine
    {
        public string subtitlesKey;
        public int index;
        public global::SubtitlesData.Entry ToSubEntry()
        {
            global::SubtitlesData.Entry entry = new()
            {
                key = $"{subtitlesKey}_{index}",
                actor = actor,
                line = index,
                status = global::SubtitlesData.Status.Valid,
                subtitle = subtitlesKey
            };
            return entry;
        }
    }
}
