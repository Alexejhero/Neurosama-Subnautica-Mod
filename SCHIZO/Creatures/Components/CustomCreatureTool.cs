namespace SCHIZO.Creatures.Components;

partial class CustomCreatureTool
{
    public override string animToolName => data.referenceAnimation.ToString().ToLower();

    private new void Awake()
    {
        if (data.subnauticaModel) data.subnauticaModel.SetActive(IS_SUBNAUTICA);
        if (data.belowZeroModel) data.belowZeroModel.SetActive(IS_BELOWZERO);

        if (IS_BELOWZERO)
        {
            leftHandIKTarget = data.leftHandIKTargetOverrideBZ;
            rightHandIKTarget = data.rightHandIKTargetOverrideBZ;
        }

        base.Awake();
    }

#if BELOWZERO
    public override void OnHolsterBegin()
    {
        OnToolAnimHolster();
        base.OnHolsterBegin();
    }
#else
    public void OnHolsterBegin()
    {
        OnToolAnimHolster();
    }
#endif
}
