using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleListener : RhythmListener {

    public GameObject[] logo;
    public GameObject noteRight;
    public GameObject noteLeft;

    Vector3[] logoScale;

    private void Start() {
        logoScale = new Vector3[logo.Length];
        for (int i = 0; i < logo.Length; i++) {
            logoScale[i] = logo[i].transform.localScale;
        }

        GetComponent<AudioSource>().Play();
    }

    private void Update() {
        for (int i = 0; i < logo.Length; i++) {
            Vector3 logoScl = logo[i].transform.localScale;
            logoScl += (logoScale[i] - logoScl) / 10f * Time.deltaTime * 60f;
            logo[i].transform.localScale = logoScl;
        }
    }

    public override void BPMEvent(RhythmEventData data) {
        logo[2].transform.localScale = logoScale[2] * 1.2f;
    }

    public override void RhythmEvent(RhythmEventData data) {
        if (data.layer.layerName == "h5") {
            if (data.objects.Count > 0) {
                int d = (int)data.objects[0];
                logo[d == 52 ? 0 : 1].transform.localScale = logoScale[d == 52 ? 0 : 1] * 1.2f;
            }
            return;
        }

        float y = 0;
        if (data.objects.Count > 0) {
            y = ((int)data.objects[0] - 65) / 2f;
        }
        bool h1 = (data.layer.layerName == "h1");
        Instantiate(h1 ? noteRight : noteLeft, transform.position + Vector3.up * y + Vector3.right * (h1 ? -8 : 8), Quaternion.identity);
    }


}
