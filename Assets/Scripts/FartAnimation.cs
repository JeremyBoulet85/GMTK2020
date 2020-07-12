using UnityEngine;

public class FartAnimation : MonoBehaviour
{
    public void OnAnimationFinish()
    {
        Destroy(gameObject);
    }
}
