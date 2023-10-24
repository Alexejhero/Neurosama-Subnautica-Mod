namespace SCHIZO.Items.Data.Crafting;

partial class Ingredient
{
    private NIngredient _converted;

    public NIngredient Convert()
    {
        if (_converted != null) return _converted;

        return _converted = new NIngredient(item.GetTechType(), amount);
    }
}
