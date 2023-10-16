namespace SCHIZO.Items.Data.Crafting;

partial class Ingredient
{
    public NIngredient Convert()
    {
        return new NIngredient(item.Convert(), amount);
    }
}
