using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public static class UtilityExtension 
{
    public static void AddClick(this Image img, UnityAction<BaseEventData> onClick)
    {
        EventTrigger trigger=img.gameObject.GetComponent<EventTrigger>();
        if (trigger==null)
        {
            trigger = img.gameObject.AddComponent<EventTrigger>();
        }
        trigger.AddEventTriggerListener(EventTriggerType.PointerClick, onClick);
    }

    public static void AddEventTriggerListener(this EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = eventTrigger.triggers.Find((trigger) => trigger.eventID == triggerType);
        if (entry == null)
        {
            entry = new EventTrigger.Entry() { eventID = triggerType };
            eventTrigger.triggers.Add(entry);
        }
        entry.callback.AddListener(action);
    }

}
