using System.Diagnostics.CodeAnalysis;

namespace SCHIZO.Creatures;

[SuppressMessage("ReSharper", "RedundantOverriddenMember")]
[SuppressMessage("ReSharper", "Unity.RedundantEventFunction")]
public class RetargetCreatureAction : CreatureAction
{
#if BELOWZERO
    public virtual void Awake()
    {
        Extensions.EnsureComponentsExtensions.EnsureComponentFields(this);
    }

    public override void Start()
    {
        base.Start();
    }

    public virtual void OnEnable()
    {
    }
#else
    public override void Awake()
    {
        base.Awake();
    }

    public virtual void Start()
    {
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public sealed override float Evaluate(Creature creat, float time) => Evaluate(time);
    public virtual float Evaluate(float time) => base.Evaluate(creature, time);

    public sealed override void StartPerform(Creature creat, float time) => StartPerform(time);
    public virtual void StartPerform(float time) => base.StartPerform(creature, time);

    public sealed override void Perform(Creature creat, float time, float deltaTime) => Perform(time, deltaTime);
    public virtual void Perform(float time, float deltaTime) => base.Perform(creature, time, deltaTime);

    public sealed override void StopPerform(Creature creat, float time) => StopPerform(time);
    public virtual void StopPerform(float time) => base.StopPerform(creature, time);
#endif
}
