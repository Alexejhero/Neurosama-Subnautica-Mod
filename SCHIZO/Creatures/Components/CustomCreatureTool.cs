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
            if (leftHandIKTargetOverrideBZ) leftHandIKTarget = leftHandIKTargetOverrideBZ;
            if (rightHandIKTargetOverrideBZ) rightHandIKTarget = rightHandIKTargetOverrideBZ;
        }

        base.Awake();
    }
}
