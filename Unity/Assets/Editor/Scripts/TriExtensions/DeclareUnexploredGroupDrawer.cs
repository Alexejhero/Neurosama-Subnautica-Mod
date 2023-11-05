using Editor.Scripts.TriExtensions;
using TriExtensions;
using TriInspector;
using TriInspector.Elements;

[assembly: RegisterTriGroupDrawer(typeof(TriFoldoutGroupDrawer))]

namespace Editor.Scripts.TriExtensions
{
    internal class TriFoldoutGroupDrawer : TriGroupDrawer<DeclareUnexploredGroupAttribute>
    {
        public override TriPropertyCollectionBaseElement CreateElement(DeclareUnexploredGroupAttribute attribute)
        {
            return new TriBoxGroupElement(new TriBoxGroupElement.Props
            {
                title = attribute.Title,
                titleMode = TriBoxGroupElement.TitleMode.Foldout,
                expandedByDefault = attribute.Expanded,
                hideIfChildrenInvisible = true,
            });
        }
    }
}
