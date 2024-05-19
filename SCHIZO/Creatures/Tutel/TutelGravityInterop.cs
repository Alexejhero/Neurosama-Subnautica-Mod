namespace SCHIZO.Creatures.Tutel;

partial class TutelGravityInterop
{
    private void Awake()
    {
#if BELOWZERO
        LandCreatureGravity lcg = gameObject.AddComponent<LandCreatureGravity>();
        lcg.onSurfaceTracker = onSurfaceTracker as OnSurfaceTracker;
        lcg.liveMixin = liveMixin as LiveMixin;
        lcg.creatureRigidbody = creatureRigidbody;
        lcg.worldForces = worldForces as WorldForces;
        lcg.bodyCollider = bodyCollider;
        lcg.pickupable = pickupable as Pickupable;
        lcg.downforce = downforce;
        lcg.aboveWaterGravity = aboveWaterGravity;
        lcg.underWaterGravity = underWaterGravity;
        lcg.applyDownforceUnderwater = applyDownforceUnderwater;
        lcg.canGoInStasisUnderwater = canGoInStasisUnderwater;
        lcg.trackSurfaceCollider = trackSurfaceCollider;
#else
        CaveCrawlerGravity ccg = gameObject.AddComponent<CaveCrawlerGravity>();
        ccg.caveCrawler = caveCrawler;
        ccg.liveMixin = liveMixin as LiveMixin;
        ccg.crawlerRigidbody = creatureRigidbody;
#endif
        Destroy(this);
    }
}
