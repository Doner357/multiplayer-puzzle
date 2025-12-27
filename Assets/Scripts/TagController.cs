using UnityEngine;
using System.Collections.Generic;

public class TagController : MonoBehaviour
{
    public List<GameTags> tags = new List<GameTags>();
    
    public bool HasTag(GameTags tagToCheck)
    {
        return tags.Contains(tagToCheck);
    }
}
