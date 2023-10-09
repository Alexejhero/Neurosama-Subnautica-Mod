// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SCHIZO.Unity.Retargeting.BelowZero;
using SCHIZO.Unity.Retargeting.Subnautica;
using UnityEditor;

namespace SCHIZO.Unity.Items
{
    public static class test
    {
        [InitializeOnLoadMethod]
        public static void x()
        {
            Dictionary<int, (string, string)> techtypes = new Dictionary<int, (string, string)>();

            foreach (TechType_SN t in typeof(TechType_SN).GetEnumValues().Cast<TechType_SN>())
            {
                techtypes[(int) t] = ("Subnautica", t.ToString());
            }

            foreach (TechType_BZ t in typeof(TechType_BZ).GetEnumValues().Cast<TechType_BZ>())
            {
                bool isObsolete = typeof(TechType_BZ).GetField(t.ToString()).GetCustomAttribute<ObsoleteAttribute>() != null;

                if (techtypes.TryGetValue((int) t, out (string attributes, string value) tuple))
                {
                    techtypes[(int) t] = (tuple.attributes + (isObsolete ? ",BelowZeroObsolete" : ",BelowZero"), tuple.value == t.ToString() ? t.ToString() : tuple.value + "," + t.ToString());
                }
                else
                {
                    techtypes[(int) t] = (isObsolete ? "BelowZeroObsolete" : "BelowZero", t.ToString());
                }
            }

            File.WriteAllLines(@"C:\Users\alexe\Desktop\new.txt", techtypes.Select(l => $"[{l.Value.Item1}] {l.Value.Item2} = {l.Key},"));
        }
    }
}
