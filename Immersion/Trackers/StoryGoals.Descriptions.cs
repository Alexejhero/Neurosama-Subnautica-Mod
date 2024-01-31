namespace Immersion.Trackers;

partial class StoryGoals
{
    public static Dictionary<string, string> StoryGoalDescriptions = new()
    {
        // sorted roughly in story order

        // substitutions:
        //  {player} - player name
        // pronouns
        /// <see cref="Formatting.PronounSet"/>
        //  {subject} - e.g. "he"
        //  {object} - e.g. "him"
        //  {possessive} - "his"
        //  {is} - contraction of "X is" (e.g. "they're")
        //  {has} - contraction of "X has" (e.g. "they've")
        //  {reflexive} - "himself"

        // this is way more goals than we're actually going to use
        // TODO: go through and pick the ones we want
        #region Intro/Download
        ["IntroComplete"] = "{player} has crash landed in a frozen valley on planet 4546B.",
        // ["TwistyBridgesSOS"] = null, // first time SOS
        // ["OnEnterSanctuary"] = null, // triggers dialogue
        // ["SanctuaryCaveComplete"] = null, // finished entrance dialogue
        // ["OnEnterSanctuaryCubeRoom"] = null, // dialogue
        // ["StartApproachingCube"] = null, // dialogue
        // ["AlanDownloadBlackout"] = null,
        // despite this being a very very important goal, it's triggered in the middle of a cutscene
        // so we don't actually get a chance to react to it
        // ["SanctuaryCompleted"] = null,
        // ["DisableSanctuaryForceField"] = null, // disabled forcefields on places w/ precursor body parts
        // despite having "AfterDownload" in its name, this just triggers when you exit regardless of whether you have SanctuaryCompleted or not
        ["SanctuaryExitAfterDownload"] = "{player} has downloaded an alien Architect's consciousness into {possessive} head.",

        // ["Call_AlAn_Meet"] = null,
        // ["Body"] = "{player} must collect three blueprints to construct an Architect body.",
        #endregion Intro/Download

        #region Main story
        // ["TellPrecursorArtifact1"] = null, // gives first signal
        // ["VisitArtifact1"] = null,
        // etc for the rest of the artifacts
        // ["Log_Alan_Body_Request"] = null,
        // ["PrecursorAskedForBody"] = null,
        // ["Alan_Body_3_Clue"] = null, // rough location
        // ["Alan_Body_3_Clue2"] = null, // close
        // ["UnlockDeepPadsCache"] = null, // right outside
        ["Scan_PrecursorSkeleton"] = "{player} has scanned an Architect skeleton, obtaining the first blueprints for the Architect body parts.",
        // ["UnlockArcticSpiresCache"] = null,
        ["Scan_PrecursorTissue"] = "{player} has scanned a sample of Architect tissues, obtaining another blueprint for the Architect body parts.",
        // ["VrwcyjGimtxlzYzwkzfGlqdd"] = null, // encrypted with vigenere (key "belowzero") to avoid spoilers for alex
        ["Scan_PrecursorOrgans"] = "{player} has scanned a set of Architect organs, obtaining the final blueprint for the Architect body parts.",
        // the rest of the goals are encrypted with vigenere (key "belowzero") to avoid spoilers for alex
        // ["Bplb_XnhpTbgtzesc"] = null,
        // ["BxMczxJrqjpthuSiiajrlz"] = null,
        // ["PrAfabyigpvYDYEesfjglhac"] = null,
        // ["Tglb_LqitiswzfXnhp"] = null,
        // ["BplbPnIerHexs"] = null,
        #endregion Main story

        #region Delta Station
        // ["DeltaIslandBeaconTimed"] = null, // timed autodiscovery
        // ["DeltaIslandBeacon"] = null, // triggered when in range, enables the beacon
        // ["DeltaIslandFirstVisit"] = null,
        // ["Scan_Alterra_Locations_Map"] = null,
        #endregion Delta Station

        #region Marguerit subplot
        // ["Log_Marg_DeltaIsland_GoAway"] = null,
        // ["FirstEncounterStart"] = null, // first encounter w/ marguerit
        // ["FirstEncounterEnd"] = null,
        // ["UnlockMargPostJumpSignal"] = null,

        // ["UnlockMarg2"] = null, // approaching the base
        ["MargBaseFirstVisit"] = null,
        #region Mercury II
        // ["Log_ExplorationHint_ShipWreck1"] = null, // stern
        // ["Log_ExplorationHint_ShipWreck2"] = null, // bow
        #endregion Mercury II
        ["OnUnlockRadioTowerPPU"] = null, // scanned third PPU fragment
        ["OnUnlockRadioTowerTOM"] = null,
        // ["RadioTowerTOMConnected"] = null,
        ["RadioTowerHacked"] = "{player} has disabled the Delta Station satellite radio tower, as requested by Marguerit.",
        ["MargGreenhouseHint"] = null,
        ["OnScanMarguerit"] = "{player} is scanning Marguerit in an incredibly inappropriate way, please scold {object}.",
        #endregion Marguerit subplot

        #region Phi Robotics
        // ["GlacialBasinLandBeacon"] = null, // enables Phi beacon
        // ["Log_Robin_Phi_Enter"] = null,
        // ["SpyPenguinUnlocked"] = null,
        ["OnGlacialBasinBridgeItemInserted"] = "{player} has inserted a canister of hydraulic fluid into the receptacle. The bridge is now operational again."
        #endregion Phi Robotics

        #region Random
        // ["SeaMonkeyGift"] = null,
        #endregion Random
    };
}
