using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerData;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour
{
    public OVRHand hand;

    public float threshold = 0.1f;

    public OVRSkeleton skeleton;

    private List<OVRBone> fingerBones;

    public List<Gesture> gestures;

    private Gesture previousGesture;

    public TMPro.TextMeshPro outText;

    // Start is called before the first frame update
    void Start()
    {
        //var hand = GetComponent<OVRHand>();
        fingerBones = new List<OVRBone>(skeleton.Bones);
        previousGesture = new Gesture();
    }

    // Update is called once per frame
    void Update()
    {

        outText.text = "Number Gestures Saved: " + gestures.Count;
        
        Gesture currentGesture = onRecognized();
        bool recognized = !currentGesture.Equals(new Gesture());

        if(recognized && !currentGesture.Equals(previousGesture))
        {
            previousGesture = currentGesture;
            currentGesture.onRecognized.Invoke();
            FindObjectOfType<Spawner>().Spawn(UnityEngine.Random.Range(0, 3));
        }
    }

    public void SaveGesture()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBones)
        {
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerData = data;
        gestures.Add(g);
    }

    Gesture onRecognized()
    {
        Gesture g = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach(var gesture in gestures)
        {
            float sumDistance = 0;
            bool discarded = false;
            for(int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);

                
                if (distance > threshold)
                {
                    discarded = true;
                    break;
                }

                sumDistance += distance;
            }

            if(!discarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                g = gesture;
            }
        }
        return g;
    }
}
