using UnityEngine;
using System.Collections;

public class SortParticleSystem : MonoBehaviour {
    public string LayerName = "Particles";
    public void Start()
    {
        gameObject.GetComponent<ParticleSystemRenderer>().sortingLayerName = LayerName;
    }
}
