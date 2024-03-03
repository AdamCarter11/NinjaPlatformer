using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] GameObject slashObj;
    [SerializeField] float disableTime = 4f;
    GameObject player;
    private bool disabled = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < 5 && !disabled)
        {
            slashObj.SetActive(true);
        }
        else
        {
            slashObj.SetActive(false);
        }
    }

    public void StarDisableHelper()
    {
        StartCoroutine(DisableTime());
    }
    IEnumerator DisableTime()
    {
        disabled = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        slashObj.SetActive(false);
        yield return new WaitForSeconds(disableTime);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        disabled = false;
    }
}
 