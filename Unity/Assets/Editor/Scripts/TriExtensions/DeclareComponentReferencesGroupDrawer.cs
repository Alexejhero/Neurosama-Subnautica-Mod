using Editor.Scripts.TriExtensions;
using SCHIZO.TriInspector.Attributes;
using TriInspector;
using TriInspector.Elements;

[assembly: RegisterTriGroupDrawer(typeof(DeclareComponentReferencesGroupDrawer))]

namespace Editor.Scripts.TriExtensions
{
    internal class DeclareComponentReferencesGroupDrawer : TriGroupDrawer<DeclareComponentReferencesGroupAttribute>
    {
        public override TriPropertyCollectionBaseElement CreateElement(DeclareComponentReferencesGroupAttribute attribute)
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
