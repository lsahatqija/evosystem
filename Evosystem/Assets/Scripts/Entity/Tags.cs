using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{
    public List<EntityTag> IsTags = new List<EntityTag>();
    public List<EntityTag> HasTags = new List<EntityTag>();
    public List<EntityTag> WantsTags = new List<EntityTag>();
    public List<EntityTag> AvoidTags = new List<EntityTag>();

    public void SetTags(List<EntityTag> isTags, List<EntityTag> hasTags, List<EntityTag> wantsTags, List<EntityTag> avoidTags)
    {
        IsTags = isTags;
        HasTags = hasTags;
        WantsTags = wantsTags;
        AvoidTags = avoidTags;
    }

    public bool Is(EntityTag tag) => IsTags.Contains(tag);
    public bool Has(EntityTag tag) => HasTags.Contains(tag);
    public bool Wants(EntityTag tag) => WantsTags.Contains(tag);
    public bool Avoids(EntityTag tag) => AvoidTags.Contains(tag);
}