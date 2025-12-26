using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager
{
    private static Dictionary<string, SpriteAtlas> atlasCache = new();

    /// <summary>
    /// 특정 아틀라스에서 원하는 스프라이트 호출 함수
    /// </summary>
    /// <param name="atlasName"> 찾아올 아틀라스 에셋의 이름 </param>
    /// <param name="spriteName"> 아틀라스에서 찾을 스프라이트 이름 </param>
    /// <returns> 찾고자 하는 스프라이트 </returns>
    public static Sprite GetSprite(string atlasName, string spriteName)
    {
        if (!atlasCache.TryGetValue(atlasName, out var atlas))
        {
            atlas = Resources.Load<SpriteAtlas>($"Atlas/{atlasName}");
            atlasCache.Add(atlasName, atlas);
        }

        return atlas.GetSprite(spriteName);
    }
}
