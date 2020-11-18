using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using MathNet.Numerics;

public class UnityViewManager : MonoBehaviour
{
    public static GComponent Main_com,ChooseCom,ClothUpCom,ClothDownCom,ClothCom,HairCom,LoadingCom,SliderCom,GenderCom;
    public static GButton chooseimageBtn,choosehairBtn,chooseclothBtn,chooseclothupBtn,chooseclothdownBtn,sureSize,sureGender,bowAni_Btn,runAni_Btn,walkAni_Btn;
    public static GLoader imageIcon;
    public static GTextField title;
    public static Controller chooseCon,bgCon,listCon,sliderCon,genderCon;
    public static GList upclothList, downclothList, clothList, hairList;
    public static GSlider slider_bodyheight, slider_bodyweight, slider_chestgirth, slider_bellygirth, slider_hipgirth,slider_shoulderwith,slider_trouserlength;
    public static GTextField slider_bodyheight_text, slider_bodyweight_text, slider_chestgirth_text, slider_bellygirth_text, slider_hipgirth_text, slider_shoulderwith_text, slider_trouserlength_text;
    public static void Inst(string packName) {
        Application.targetFrameRate = 60;
        UIObjectFactory.SetLoaderExtension(typeof(MyGloader));
        if (Screen.width > Screen.height) {
            //GRoot.inst.SetContentScaleFactor(736, 414, UIContentScaler.ScreenMatchMode.MatchHeight);
            GRoot.inst.SetContentScaleFactor(736, 414, UIContentScaler.ScreenMatchMode.MatchHeight);
            UIPackage.AddPackage(packName);
            Main_com = UIPackage.CreateObject(packName, "Main_h").asCom;
        } else {
            GRoot.inst.SetContentScaleFactor(414, 736, UIContentScaler.ScreenMatchMode.MatchHeight);
            UIPackage.AddPackage(packName);
            Main_com = UIPackage.CreateObject(packName, "Main_v").asCom;
        }
        GRoot.inst.AddChild(Main_com);
        ChooseCom = Main_com.GetChild("ChooseCom").asCom;
        ClothCom = Main_com.GetChild("ClothCom").asCom;
        ClothUpCom = Main_com.GetChild("ClothUpCom").asCom;
        ClothDownCom = Main_com.GetChild("ClothDownCom").asCom;
        HairCom = Main_com.GetChild("HairCom").asCom;
        LoadingCom = Main_com.GetChild("LoadingCom").asCom;
        SliderCom = Main_com.GetChild("SliderCom").asCom;
        GenderCom = Main_com.GetChild("GenderCom").asCom;
        chooseimageBtn = Main_com.GetChild("choose").asButton;
        imageIcon = Main_com.GetChild("imageIcon").asLoader;
        choosehairBtn = ChooseCom.GetChild("choosehairBtn").asButton;
        chooseclothBtn = ChooseCom.GetChild("chooseclothBtn").asButton;
        chooseclothupBtn = ChooseCom.GetChild("chooseupclothBtn").asButton;
        chooseclothdownBtn = ChooseCom.GetChild("choosedownclothBtn").asButton;      
        chooseCon = Main_com.GetController("choose_con");
        listCon = Main_com.GetController("list_con");
        bgCon = Main_com.GetController("bg_con");
        title = Main_com.GetChild("title").asTextField;
        upclothList = ClothUpCom.GetChild("upclothlist").asList;
        downclothList = ClothDownCom.GetChild("downclothlist").asList;
        clothList = ClothCom.GetChild("clothlist").asList;
        hairList = HairCom.GetChild("hairlist").asList;
        slider_bodyheight = SliderCom.GetChild("slider1").asSlider;
        slider_bodyheight.min = 140.0f;
        slider_bodyheight.max = 195.0f;
        slider_bodyweight = SliderCom.GetChild("slider2").asSlider;
        slider_bodyweight.min = 20.0f;
        slider_bodyweight.max = 120.0f;
        slider_chestgirth = SliderCom.GetChild("slider3").asSlider;
        slider_chestgirth.min = 0.7;
        slider_chestgirth.max = 1.3;
        slider_bellygirth = SliderCom.GetChild("slider4").asSlider;
        slider_bellygirth.min = 0.5;
        slider_bellygirth.max = 1.3;
        slider_hipgirth = SliderCom.GetChild("slider5").asSlider;
        slider_hipgirth.min = 0.75f;
        slider_hipgirth.max = 1.5f;
        slider_shoulderwith = SliderCom.GetChild("slider6").asSlider;
        slider_shoulderwith.min = 0.32f;
        slider_shoulderwith.max = 0.55f;
        slider_trouserlength = SliderCom.GetChild("slider7").asSlider;
        slider_trouserlength.min = 0.5f;
        slider_trouserlength.max = 1.2f;
        sureSize = SliderCom.GetChild("OK").asButton;
        sliderCon = Main_com.GetController("slider_con");
        genderCon = GenderCom.GetController("gender_con");
        sureGender = GenderCom.GetChild("sure").asButton;
        slider_bodyheight_text = slider_bodyheight.GetChild("text").asTextField;
        slider_bodyweight_text = slider_bodyweight.GetChild("text").asTextField;
        slider_chestgirth_text = slider_chestgirth.GetChild("text").asTextField;
        slider_bellygirth_text = slider_bellygirth.GetChild("text").asTextField;
        slider_hipgirth_text = slider_hipgirth.GetChild("text").asTextField;
        slider_shoulderwith_text = slider_shoulderwith.GetChild("text").asTextField;
        slider_trouserlength_text = slider_trouserlength.GetChild("text").asTextField;
        bowAni_Btn = ChooseCom.GetChild("bow").asButton;
        walkAni_Btn = ChooseCom.GetChild("walk").asButton;
        runAni_Btn = ChooseCom.GetChild("run").asButton;


    } 
}
