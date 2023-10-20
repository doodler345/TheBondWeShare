using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class RopeController : MonoBehaviour
{
    [SerializeField] ObiParticleAttachment _startAttachment, _endAttachement;
    [SerializeField] float _minLength = 2.0f, _maxLength = 5.0f;
    ObiRopeCursor _cursor;
    ObiRope _rope;
    ObiParticleAttachment.AttachmentType _type;

    private void Awake()
    {
        _cursor = GetComponent<ObiRopeCursor>();
        _rope = GetComponent<ObiRope>();
    }

    private void Start()
    {
        ResetLength();
    }

    public void StaticDynamicSwitch(bool isStatic, int playerID)
    {
        switch (playerID)
        {
            case 0:
                if (isStatic) _startAttachment.attachmentType = ObiParticleAttachment.AttachmentType.Static;
                else _startAttachment.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;
                break;
            case 1:
                if (isStatic) _endAttachement.attachmentType = ObiParticleAttachment.AttachmentType.Static;
                else _endAttachement.attachmentType = ObiParticleAttachment.AttachmentType.Dynamic;
                break;
            default:
                break;
        }

        if (_startAttachment.attachmentType != _endAttachement.attachmentType)
        {
            if (_startAttachment.attachmentType == ObiParticleAttachment.AttachmentType.Static)
            {
                _cursor.direction = true;
                _cursor.cursorMu = 0;
                _cursor.sourceMu = 1;
            }
            else
            {
                _cursor.direction = false;
                _cursor.cursorMu = 1;
                _cursor.sourceMu = 0;
            }
        }

    }

    public void Crane(bool up)
    {
        if (_startAttachment.attachmentType == _endAttachement.attachmentType) return;

        if (up)
        {
            if (_rope.restLength > _minLength)
                _cursor.ChangeLength(_rope.restLength - 1f * Time.deltaTime);
        }
        else
        {
            if (_rope.restLength < _maxLength)
                _cursor.ChangeLength(_rope.restLength + 1f * Time.deltaTime);
        }
    }

    public void ResetLength()
    {
        _cursor.ChangeLength(_maxLength);
    }

    public void EnableTearing(bool isActive)
    {
       _rope.tearingEnabled = isActive;
    }
}
