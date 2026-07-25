using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    public UnityEvent<float> LightsOffGimmick;
    public UnityEvent<float> WindGimmick;
    public UnityEvent<bool, float> ResetTimer;
}
