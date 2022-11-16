using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class TutorialControl : MonoBehaviour
{

    TutorialConfig config => TutorialConfig.instance;
    private void Start()
    {

    }


    private void Update()
    {
        if (useTimer)
        {
            mTimer += Time.deltaTime;
            if (mTimer > maxTime)
            {
                moveNext = true;
            }
        }
    }

    bool useTimer;
    float maxTime;
    float mTimer;

    bool moveNext = false;

    bool MoveNext()
    {
        if (moveNext)
        {
            moveNext = false;
            return true;
        }
        return false;
    }
    IEnumerator TutorialCycle()
    {

        foreach (var x in config.insrtuctions)
        {
            SetHelpText(x.helpText);
            switch (x.type)
            {
                case InstructionType.ASkForInput:

                    break;
            }
            yield return new WaitUntil(MoveNext);
        }
    }



    void SetHelpText(string helpText)
    {
        //TODO
    }

}