using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class SwitchRopeDirection : MonoBehaviour
{
    [SerializeField] ObiRopeCursor cursor;
    [SerializeField] ObiParticleAttachment attachmentStart, attachmentEnd;

    void Switch()
    {
        cursor.direction = !cursor.direction;
        cursor.cursorMu = 1 - cursor.cursorMu;
        cursor.sourceMu = 1 - cursor.sourceMu;

        //attachmentStart.
    }
}
