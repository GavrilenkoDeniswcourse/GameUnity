//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using UnityEngine.UIElements;


//public class ToolAnimetion : MonoBehaviour
//{
//    private Axe _axe;

//    private Item.ToolType _tool;

//    private Animator animator;

//    private const string TOOLTIPE = "toolTipe";

    
//    void Awake()
//    {
//        animator = GetComponent<Animator>();
//        _axe.OnAxe += _axe_OnAxe;
//    }

//    void Update()
//    {
        
//    }

//    private void _axe_OnAxe(object sender, EventArgs e)
//    {
//        animator.SetTrigger(TOOLTIPE);
//    }

//    private void AnimationTool()
//    {
//        switch (_tool)
//        {
//            default:
//            case Item.ToolType.None:

//                break;
//            case Item.ToolType.Axe:
//                _axe_OnAxe();
//                break;

//        }
//    }


//}
