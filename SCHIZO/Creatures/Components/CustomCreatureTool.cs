namespace SCHIZO.Creatures.Components;

partial class CustomCreatureTool
{
    public override string animToolName => referenceAnimation.ToString().ToLower();

    private new void Awake()
    {
        if (subnauticaModel) subnauticaModel.SetActive(IS_SUBNAUTICA);
        if (belowZeroModel) belowZeroModel.SetActive(IS_BELOWZERO);

        if (IS_BELOWZERO)
        {
            leftHandIKTarget = leftHandIKTargetOverrideBZ;
            rightHandIKTarget = rightHandIKTargetOverrideBZ;
        }

        base.Awake();
    }
}
