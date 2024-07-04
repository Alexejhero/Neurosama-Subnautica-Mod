using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHIZO.Helpers;
partial class FMODHelpers
{
    public static class GameEvents
    {
        public static FMODAsset FishSplat { get; } = GetFmodAsset("event:/sub/common/fishsplat");
    }
}
