using UnityEngine;

public class RaycastTouch : MonoBehaviour
{
    public AudioClip touchSound;
    private bool isPlaying = false;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        PlaySound();
                    }
                }
            }
        }
    }

    void PlaySound()
    {
        if (!isPlaying)
        {
            AudioSource.PlayClipAtPoint(touchSound, transform.position);
            isPlaying = true;
            Invoke("ResetPlayState", touchSound.length);
        }
    }

    void ResetPlayState()
    {
        isPlaying = false;
    }
}


