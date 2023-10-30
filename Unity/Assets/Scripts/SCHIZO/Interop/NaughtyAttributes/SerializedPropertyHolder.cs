namespace SCHIZO.Interop.NaughtyAttributes
{
    public sealed partial class SerializedPropertyHolder
    {
        public object serializedObject_targetObject =>
#if UNITY_EDITOR
            _property.serializedObject.targetObject;
#else
            throw new System.InvalidOperationException();
#endif
    }
}
