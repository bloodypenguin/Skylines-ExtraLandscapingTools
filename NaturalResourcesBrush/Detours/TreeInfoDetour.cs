using System.Collections.Generic;
using ColossalFramework.Math;



namespace NaturalResourcesBrush
{

    //TODO(earalov): make this detour work properly
    public class TreeInfoDetour :TreeInfo
    {
        public TreeInfo GetVariation(ref Randomizer r)
        {
            var mVariations = new List<TreeInfo.Variation>();
            for (uint index = 0U; (long)index < (long)PrefabCollection<TreeInfo>.LoadedCount(); ++index)
            {
                TreeInfo loaded = PrefabCollection<TreeInfo>.GetLoaded(index);
                if (loaded != null)
                {
                    var variations = loaded.m_variations;
                    if (variations != null)
                    {
                        mVariations.AddRange(variations);
                    }
                }
            }

            if (mVariations != null && mVariations.Count != 0)
            {
                int num = r.Int32(100U);
                for (int index = 0; index < mVariations.Count; ++index)
                {
                    num -= mVariations[index].m_probability;
                    if (num < 0)
                    {
                        TreeInfo treeInfo = mVariations[index].m_finalTree;
                        if (treeInfo != null)
                            return treeInfo.GetVariation(ref r);
                    }
                }
            }
            return this;
        } 
    }
}