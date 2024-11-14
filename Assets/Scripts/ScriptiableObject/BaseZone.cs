using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Zone", menuName = "Zone")]
public class BaseZone : ScriptableObject
{
    public string Description = "This is the default zone.";
    public Vector2 position;
    public Vector2 size;
    public List<BaseFish> FeaturedFish;
    public List<BaseMutation> FeaturedMutations;
    public List<BaseFish> GetSortedFeaturedFish(){
        List<BaseFish> Sorted = FeaturedFish.OrderBy(x =>x.Rarity.OneIn).ToList();
        return Sorted;
    }
    public List<BaseMutation> GetSortedFeaturedMutations()
    {
        List<BaseMutation> Sorted = FeaturedMutations.OrderBy(x => x.OneIn).ToList();
        return Sorted;
    }
}
