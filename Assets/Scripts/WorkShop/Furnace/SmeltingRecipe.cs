using System;

[Serializable]
public class SmeltingRecipeByID
{
    public int inputItem1ID;
    public int inputItem2ID = -1;  // -1 = нет второго предмета
    public int outputItemID;
    public float smeltTime = 3f;
}