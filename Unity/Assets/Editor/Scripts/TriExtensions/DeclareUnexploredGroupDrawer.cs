using Editor.Scripts.TriExtensions;
using SCHIZO.TriInspector.Attributes;
using TriInspector;
using TriInspector.Elements;

[assembly: RegisterTriGroupDrawer(typeof(DeclareUnexploredGroupDrawer))]

namespace Editor.Scripts.TriExtensions
{
    internal class DeclareUnexploredGroupDrawer : TriGroupDrawer<DeclareUnexploredGroupAttribute>
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
