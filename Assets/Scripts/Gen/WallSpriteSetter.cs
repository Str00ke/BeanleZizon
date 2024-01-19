using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WallSpriteSetter : ScriptableObject
{
    [Serializable]
    public struct ElementSprSetCardinal
    {
        public Cardinals cardinal;
        public Sprite sprite;
    }


    [Serializable]
    public struct ElementSprSet
    {
        public Element element;
        public string elementName;

        //public ElementSprSetCardinal NorthSpr = { Cardinals.NORTH, null }; Come on Unity, upgrade to CSharp 10.0 ...
        public ElementSprSetCardinal NorthSpr;
        public ElementSprSetCardinal SouthSpr;
        public ElementSprSetCardinal EastSpr;
        public ElementSprSetCardinal WestSpr;
    }

    public List<ElementSprSet> Sets = new();
}
