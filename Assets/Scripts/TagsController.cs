using UnityEngine;
using System.Collections.Generic;

public class TagsController : MonoBehaviour
{
    public List<GameTag> tags = new List<GameTag>();
    
    public bool HasTag(GameTag tagToCheck)
    {
        return tags.Contains(tagToCheck);
    }
}
