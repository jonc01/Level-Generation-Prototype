using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private GameObject RunDust;
    [SerializeField] private GameObject LandingDust;
    [SerializeField] private GameObject JumpDust;

    float JumpDustDuration = .417f;
    float RunDustDuration = .417f;

    private void Start()
    {
        
    }

    public void SpawnRunVFX(Transform spawnPos, bool facingRight = true)
    {
        if (RunDust == null) return;
        GameObject g;
        if (facingRight) g = Instantiate(RunDust, spawnPos.position, Quaternion.identity, transform);
        else g = Instantiate(RunDust, spawnPos.position, spawnPos.rotation * Quaternion.Euler(0, 180f, 0), transform);
        StartCoroutine(DeleteObject(g, RunDustDuration));
    }
    
    public void SpawnJumpVFX(Transform spawnPos)
    {
        if (JumpDust == null) return;
        GameObject g = Instantiate(JumpDust, spawnPos.position, Quaternion.identity, transform);
        StartCoroutine(DeleteObject(g, JumpDustDuration));
    }

    IEnumerator DeleteObject(GameObject g, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(g);
    }
}
