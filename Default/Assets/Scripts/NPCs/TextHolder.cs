using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHolder : MonoBehaviour {

    public List<TextAsset> textAssetList;

    public string[] giveMeTextList(int textAssetNum) {
        return textAssetList[textAssetNum].text.Split('\n');
    }

}
