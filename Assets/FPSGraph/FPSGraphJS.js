// FPS Graph - Performance Analyzer Tool - Version 0.91
//
// To use FPS Graph simply add this script to your main camera. 
//    detailed explanation: Select the camera used in your scene then go to the inspector window click on the component menu and down to scripts you should find FPSGraph 
// Options:
//    Audio Feedback: This allows you to audibly "visualize" the perforamnce of the scene
//    Audio Feedback Volume: This specifies how loud the audio feedback is, from 0.0-1.0
//    Graph Multiply: This specifies how zoomed in the graph is, the default is the graph is multiplyed by 2x, meaning every pixel is doubled.
//    Graph Position: This specifies where the graph sits on the screen examples: x:0.0, y:0.0 (top-left) x:1.0, y:0.0 (top-right) x:0.0, y:1.0 (bottom-left) x:1.0 y:1.0 (bottom-right)
//    Frame History Length: This is the length of FPS that is displayed (Set this to a low amount if you are targeting older mobile devices)

import System.Diagnostics.Stopwatch;
#pragma strict

private var mat : Material;

public function CreateLineMaterial() {
    #if UNITY_ANDROID
        if(androidMaterial==null)
            Debug.LogError("You must attach the FPSGraph-AndroidMaterial for FPS graph to work on Android");
        mat = androidMaterial;
    #else
        if( !mat ) {
            mat = new Material( "Shader \"Lines/Colored Blended\" {" +
                    "SubShader { Pass { " +
                    "    Blend SrcAlpha OneMinusSrcAlpha " +
                    "    ZWrite Off Cull Off Fog { Mode Off } " +
                    "    BindChannels {" +
                    "      Bind \"vertex\", vertex Bind \"color\", color }" +
                    "} } }" );
            mat.hideFlags = HideFlags.HideAndDontSave;
            mat.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    #endif
}

public var androidMaterial:Material;
public var audioFeedback:boolean = false;
public var audioFeedbackVolume:float = 0.5;
public var graphMultiply:int = 2;
public var graphPosition:Vector2 = Vector2(0.0, 0.0);
public var frameHistoryLength:int = 120;
public var CpuColor:Color = Color( 53.0/ 255.0, 136.0 / 255.0, 167.0 / 255.0 );
public var RenderColor:Color = Color( 112.0/ 255.0, 156.0 / 255.0, 6.0 / 255.0 );
public var OtherColor:Color = Color( 193.0/ 255.0, 108.0 / 255.0, 1.0 / 255.0 );

// Re-created Assets
private var numberBits:int[] = [1,1,1,0,1,1,1,0,1,1,1,0,1,1,1,0,0,0,1,0,1,1,1,0,1,1,1,0,0,1,0,0,1,1,1,0,0,0,1,1,0,1,0,0,1,0,0,0,1,0,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,1,0,0,1,0,0,1,0,1,0,0,0,1,1,0,1,0,0,1,0,0,0,0,1,0,0,1,1,0,1,1,1,0,1,1,1,0,1,1,1,0,0,0,1,0,1,1,1,0,1,1,1,1,0,1,0,1,1,0,0,1,0,1,0,0,0,1,0,1,0,1,0,1,0,0,0,1,0,0,0,0,0,1,0,1,0,1,0,1,0,1,1,1,1,0,0,1,0,0,0,1,0,0,1,1,1,0,0,0,1,0,1,1,1,0,1,1,1,0,1,1,1,0,1,1,1,0,1,1,1];
private var fpsBits:int[] = [1,0,0,0,1,0,0,0,1,1,1,1,1,0,0,1,1,1,0,0,1,1,1,0,0,0,1,0,1,0,1,1,0,0,1,1,0,1,1,1,0,1,1,1];
private var mbBits:int[] = [1,0,1,0,1,0,1,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,1,0,1,1,1,0,0,0,0,0,0,1,0,0];
private var bNote:float[] = [0.01300049,0.02593994,0.03900146,0.03894043,0.05194092,0.06494141,0.06494141,0.06494141,0.07791138,0.09091187,0.07791138,0.09091187,0.1039124,0.1038818,0.1168823,0.1168823,0.1298828,0.1298218,0.1429443,0.1427612,0.1429443,0.1687622,0.1688843,0.1687927,0.1818237,0.1818542,0.1947327,0.1949158,0.2206421,0.2079163,0.2206726,0.2208557,0.2337036,0.2208252,0.2207642,0.2337646,0.2467651,0.2337341,0.2597656,0.2467651,0.2597046,0.2727661,0.2597046,0.2597656,0.2857361,0.2856445,0.2857666,0.2986755,0.2987061,0.3117065,0.311676,0.324646,0.3247375,0.324585,0.3507385,0.337616,0.350647,0.363678,0.3765564,0.363678,0.3766174,0.3765869,0.389679,0.4025269,0.4286499,0.4284668,0.4286499,0.4544983,0.4545898,0.4674988,0.4675293,0.4805603,0.4804382,0.5065918,0.5194092,0.5195007,0.5195007,0.5324097,0.5455322,0.5583801,0.5585022,0.5713501,0.5715027,0.5713806,0.5844421,0.5974121,0.5973511,0.6104431,0.6103821,0.6233521,0.6364136,0.649292,0.6493835,0.6493225,0.662384,0.6752625,0.6753845,0.6882935,0.7012634,0.6883545,0.7012634,0.7142944,0.7273254,0.7272034,0.7402954,0.7402344,0.7402649,0.7402954,0.7532043,0.7532654,0.7792053,0.7792358,0.7792053,0.7792358,0.7922058,0.7921753,0.7922668,0.8051147,0.8052979,0.7921143,0.7922668,0.8051758,0.8051453,0.7922974,0.7921143,0.8182678,0.8051453,0.8182068,0.8051758,0.7922058,0.8052063,0.7922058,0.7922058,0.7662354,0.7662354,0.7532043,0.7403564,0.7401428,0.7403259,0.7142944,0.7012024,0.7014771,0.6751404,0.6494751,0.662262,0.6364136,0.6103516,0.6104126,0.6103821,0.5844116,0.5714111,0.5584717,0.5454102,0.5325317,0.5324097,0.5195313,0.5064392,0.4935303,0.4805298,0.4675293,0.4545593,0.4674988,0.4675598,0.4544983,0.4416504,0.4544373,0.4546509,0.4284668,0.4416504,0.4285278,0.4285583,0.4286194,0.4284973,0.4286499,0.4415283,0.4285583,0.4285889,0.4545288,0.4415588,0.4415894,0.4415283,0.4675293,0.4545898,0.4804382,0.4676208,0.4674683,0.4805298,0.4805298,0.5065308,0.4934082,0.5066223,0.5323181,0.5455627,0.5454102,0.5584717,0.5584412,0.5713806,0.5714722,0.5713806,0.5974731,0.5973511,0.5974121,0.6234131,0.5973206,0.5975037,0.623291,0.6234131,0.6233826,0.6233521,0.6493835,0.6233521,0.6363525,0.6234131,0.6233215,0.6364441,0.636261,0.6364441,0.636322,0.6104126,0.6363525,0.6104126,0.610321,0.6234436,0.6103516,0.5974121,0.6104431,0.623291,0.6104736,0.623291,0.6104736,0.6233215,0.5974121,0.6234436,0.61026,0.6105347,0.5972595,0.6235046,0.6102905,0.5974731,0.5973511,0.6104431,0.5843506,0.5844727,0.5843811,0.5844421,0.5714111,0.5714722,0.5713501,0.5585327,0.5583496,0.5585022,0.5324707,0.5454407,0.5194702,0.5194702,0.4935303,0.5064697,0.4935608,0.4934082,0.4806213,0.4804077,0.4806519,0.4674072,0.4676208,0.4674683,0.4935608,0.4804993,0.4804993,0.4675903,0.4674377,0.4676514,0.4544373,0.4546204,0.4545288,0.4674988,0.4675903,0.4544678,0.4546509,0.4544373,0.4416199,0.4544983,0.4415894,0.4415894,0.4414978,0.4545898,0.4284973,0.4546509,0.4414673,0.4545898,0.4415894,0.4544678,0.4416504,0.4414673,0.4286499,0.4414978,0.4286499,0.4284668,0.4286804,0.4414368,0.4287109,0.4284363,0.4286804,0.4154968,0.4286194,0.4285583,0.4155884,0.4025879,0.4026184,0.3895874,0.4026184,0.4025574,0.3766479,0.3896179,0.3766174,0.3896484,0.3635254,0.3637695,0.3375244,0.3507996,0.3245544,0.3247681,0.311615,0.3117065,0.2857361,0.285675,0.2598267,0.2596436,0.2468262,0.2207336,0.2077942,0.2208252,0.1947327,0.1818848,0.1687927,0.1558533,0.1428528,0.1298828,0.1038513,0.1169128,0.09091187,0.09088135,0.06500244,0.06484985,0.06500244,0.0519104,0.03897095,0.02600098,0.02593994,0.01300049,3.051758E-05,-9.155273E-05,-0.01287842,-9.155273E-05,-0.02590942,-0.02603149,-0.03890991,-0.03900146,-0.03894043,-0.03897095,-0.05194092,-0.06494141,-0.05194092,-0.07794189,-0.07791138,-0.07791138,-0.09091187,-0.1039429,-0.1038513,-0.1168823,-0.1168823,-0.1299133,-0.1427917,-0.1429443,-0.1557312,-0.1429443,-0.1687622,-0.1689148,-0.1817017,-0.1949463,-0.1946716,-0.2078857,-0.2337341,-0.2207642,-0.2207947,-0.2467651,-0.2467346,-0.2338257,-0.2856445,-0.2987671,-0.2986145,-0.3117676,-0.3116455,-0.3117371,-0.337616,-0.3506775,-0.337616,-0.350708,-0.3506165,-0.3636169,-0.4026794,-0.4024658,-0.3897705,-0.3894653,-0.4026794,-0.4155884,-0.4155273,-0.4286499,-0.4284973,-0.4415894,-0.4545593,-0.4545288,-0.4415283,-0.4416199,-0.4544983,-0.4545593,-0.4675598,-0.4674683,-0.4675903,-0.4674988,-0.4805603,-0.4674988,-0.4805298,-0.4675293,-0.4804993,-0.4805908,-0.4804382,-0.4805908,-0.4804382,-0.4675903,-0.4675293,-0.4674988,-0.4675903,-0.4674377,-0.4676208,-0.4415283,-0.4545288,-0.4545898,-0.4674683,-0.4416199,-0.4415283,-0.4285889,-0.4285583,-0.4155884,-0.4026184,-0.4155884,-0.4025269,-0.3897095,-0.3894958,-0.3767395,-0.3765564,-0.350647,-0.363678,-0.3635864,-0.3377075,-0.337616,-0.3377075,-0.311676,-0.311676,-0.2987366,-0.2856445,-0.2728271,-0.2726135,-0.2598267,-0.2596741,-0.2338257,-0.2207336,-0.2078247,-0.2077332,-0.2078857,-0.1947021,-0.1949158,-0.1687317,-0.1689148,-0.1557922,-0.1558838,-0.1427917,-0.1299744,-0.1167908,-0.1169434,-0.09085083,-0.1039429,-0.09088135,-0.07794189,-0.06491089,-0.05197144,-0.03894043,-0.02600098,-0.01293945,-0.02603149,-0.01296997,3.051758E-05,0.01293945,0.01306152,0.03887939,0.03900146,0.03894043,0.03897095,0.06497192,0.06488037,0.05200195,0.07785034,0.0909729,0.07788086,0.1039124,0.09091187,0.09091187,0.09088135,0.1039124,0.1039124,0.1298218,0.1169434,0.1298218,0.1298828,0.1169128,0.1298218,0.1428833,0.1169128,0.1297913,0.1299744,0.1427612,0.1169434,0.1298523,0.1558228,0.1558838,0.1298523,0.1428528,0.1428528,0.1298828,0.1298523,0.1169128,0.1298523,0.1168518,0.1169434,0.1038513,0.1169128,0.1038818,0.1168823,0.09088135,0.1169434,0.09085083,0.0909729,0.1038208,0.1039734,0.07785034,0.0909729,0.07785034,0.0909729,0.07788086,0.07797241,0.06484985,0.0909729,0.06491089,0.06494141,0.07794189,0.07785034,0.07803345,0.07781982,0.0909729,0.1038513,0.1039429,0.1038818,0.1168823,0.1168823,0.1168518,0.1299438,0.1427917,0.1428833,0.1558533,0.1558228,0.1558533,0.1688232,0.1818237,0.1817932,0.1948547,0.1947632,0.2207947,0.2207642,0.2337646,0.2337952,0.2597046,0.2337952,0.2597351,0.2727051,0.2727661,0.2726746,0.2987671,0.285675,0.2987061,0.2987061,0.311676,0.324707,0.3376465,0.324646,0.3247375,0.3246155,0.3377075,0.3376465,0.350647,0.337677,0.3376465,0.3506775,0.337616,0.363678,0.3506165,0.3506775,0.337677,0.337616,0.350708,0.324585,0.337738,0.324646,0.3246765,0.2857361,0.2986755,0.2857056,0.2727661,0.2726746,0.2727661,0.2597351,0.2467651,0.2467346,0.2337646,0.2207947,0.2207642,0.2078552,0.2206726,0.1949158,0.1817017,0.1819458,0.1687317,0.1688843,0.1688232,0.1557922,0.1559448,0.1557312,0.1559448,0.1427917,0.1558838,0.1557922,0.1429443,0.1557617,0.1558838,0.1558228,0.1688538,0.1688232,0.1948242,0.1947632,0.2078247,0.2207947,0.2337036,0.2338867,0.246582,0.2729187,0.2855835,0.3117371,0.3246765,0.337616,0.350708,0.3895874,0.3766174,0.4155884,0.4285583,0.4545898,0.4674683,0.4805603,0.4935303,0.5324097,0.5455322,0.5583496,0.5844727,0.5974121,0.6233215,0.6364441,0.6622314,0.6754456,0.7011719,0.701416,0.7142029,0.7273254,0.7532349,0.7532349,0.7532654,0.7791748,0.7792969,0.7921448,0.8182373,0.8051147,0.8052673,0.8181458,0.8312073,0.8311157,0.8181763,0.8182068,0.8312073,0.8181152,0.8052673,0.8050842,0.8052979,0.7921143,0.8052979,0.7921143,0.7922974,0.7791443,0.7662354,0.7662659,0.7662048,0.7532959,0.7402344,0.7142334,0.7143555,0.7272034,0.7013855,0.7012329,0.688324,0.688324,0.675293,0.662384,0.6493225,0.6493225,0.6364441,0.636261,0.6235046,0.62323,0.6235046,0.59729,0.5975342,0.61026,0.5975037,0.5973206,0.5844727,0.5844116,0.5973511,0.5845032,0.5713196,0.5715332,0.5713806,0.5584412,0.5584717,0.5583801,0.5455322,0.5454102,0.5454712,0.5324707,0.5324402,0.5065308,0.5194397,0.5065308,0.4934692,0.4805603,0.4934692,0.4675598,0.4415283,0.4545898,0.4545288,0.4415283,0.4285889,0.4285889,0.4025574,0.3897095,0.3894653,0.3767395,0.3765564,0.3636475,0.3506775,0.3376465,0.3246765,0.3246765,0.311676,0.2987061,0.2857361,0.2727051,0.2727356,0.2727356,0.2467041,0.2597961,0.2597046,0.2597656,0.2467651,0.2467041,0.2337952,0.2337646,0.2467651,0.2337341,0.2468262,0.2596436,0.2598267,0.2596741,0.2727966,0.2726746,0.2727661,0.285675,0.2987061,0.2987366,0.2986755,0.324707,0.324646,0.3376465,0.3377075,0.3505859,0.3507385,0.3505554,0.3637085,0.3635864,0.363678,0.3635864,0.3766479,0.3636475,0.3506165,0.363678,0.3506165,0.3376465,0.3377075,0.3505859,0.3247375,0.324646,0.2987061,0.2857361,0.2466736,0.2728577,0.2466125,0.2209167,0.2076721,0.1819153,0.1557617,0.1559143,0.1168213,0.1039429,0.07788086,0.03900146,0.01293945,0.01303101,-0.03897095,-0.06497192,-0.09085083,-0.1169434,-0.1298218,-0.1558533,-0.1948242,-0.2077637,-0.2207947,-0.2597351,-0.2727356,-0.2987061,-0.3116455,-0.3377075,-0.337616,-0.363678,-0.3636475,-0.3895569,-0.4026489,-0.4155579,-0.4155579,-0.4286499,-0.4414978,-0.4415894,-0.4415283,-0.4545593,-0.4415588,-0.4675598,-0.4415283,-0.4415588,-0.4545593,-0.4545288,-0.4285889,-0.4285583,-0.4285889,-0.4025879,-0.4155884,-0.4025879,-0.3766479,-0.3765869,-0.3636475,-0.3636475,-0.3636475,-0.3376465,-0.3246765,-0.3246765,-0.311676,-0.2987366,-0.2986755,-0.2857056,-0.2727661,-0.285675,-0.2597656,-0.2597351,-0.2467346,-0.2338257,-0.2336731,-0.2208557,-0.2207336,-0.1948547,-0.2077637,-0.1948242,-0.1947632,-0.1948547,-0.1947632,-0.1818848,-0.1817627,-0.1818542,-0.1817932,-0.1818237,-0.1818237,-0.1817932,-0.1558838,-0.1817932,-0.1558838,-0.1557922,-0.1558838,-0.1687622,-0.1559448,-0.1557617,-0.1429138,-0.1688232,-0.1558228,-0.1558533,-0.1558533,-0.1298218,-0.1429443,-0.1297913,-0.1299438,-0.1297913,-0.09094238,-0.1168823,-0.09091187,-0.1038818,-0.07797241,-0.1037903,-0.09103394,-0.07781982,-0.07800293,-0.06488037,-0.06497192,-0.0519104,-0.03900146,-0.03890991,-0.03903198,-0.03887939,-0.03903198,-0.03887939,-0.02606201,-0.02590942,-0.02600098,-0.02597046,-0.03894043,-0.01303101,-0.02593994,-0.02597046,-0.03900146,-0.02590942,-0.02606201,-0.03887939,-0.03900146,-0.05194092,-0.03894043,-0.03900146,-0.05187988,-0.06503296,-0.07781982,-0.07800293,-0.09082031,-0.09100342,-0.09082031,-0.1040039,-0.09082031,-0.09094238,-0.09091187,-0.09091187,-0.1038818,-0.1039429,-0.1038208,-0.1039734,-0.1168213,-0.1039734,-0.1168213,-0.1169128,-0.1298523,-0.1039124,-0.1168823,-0.1168823,-0.09091187,-0.1168823,-0.1168823,-0.1038818,-0.1169128,-0.1168213,-0.1039734,-0.1298218,-0.1168823,-0.1039124,-0.1168518,-0.1039124,-0.1169128,-0.1168518,-0.1168518,-0.0909729,-0.1168213,-0.1039734,-0.09082031,-0.1039734,-0.1038208,-0.07803345,-0.07781982,-0.0909729,-0.06488037,-0.07797241,-0.07788086,-0.06500244,-0.07781982,-0.07803345,-0.07781982,-0.07800293,-0.09082031,-0.07803345,-0.09082031,-0.07797241,-0.07788086,-0.09094238,-0.09088135,-0.0909729,-0.09085083,-0.1168823,-0.1169434,-0.1167603,-0.1170654,-0.1426697,-0.1430054,-0.1427917,-0.1428223,-0.1559143,-0.1557922,-0.1688538,-0.1558533,-0.1817627,-0.1688843,-0.1947937,-0.1947937,-0.2078247,-0.2077332,-0.1948547,-0.2077942,-0.2077332,-0.1948853,-0.1947327,-0.2078552,-0.2077637,-0.1948242,-0.1947632,-0.1948242,-0.1948242,-0.1947632,-0.2078552,-0.1947327,-0.1818848,-0.1817627,-0.1688843,-0.1557922,-0.1558838,-0.1428528,-0.1298218,-0.1169739,-0.1038208,-0.09094238,-0.1039124,-0.07788086,-0.07794189,-0.05194092,-0.03897095,-0.0519104,-0.01309204,-0.02581787,-0.0001525879];
private var graphKeys:Color[] = [Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,1), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0), Color(1,1,1,0)];

private var audioClip:AudioClip;
private var audioSource:AudioSource;

private var graphTexture:Texture2D;
private var graphHeight:int = 100;

private var textOverlayMask:int[,];

private var dtHistory:float[,];
private var i:int;
private var j:int;
private var x:int;
private var y:int;
private var val:float;
private var color:Color;
private var color32:Color32;
private var maxFrame:float = 0.0;
private var yMulti:float;

private var fpsColors:Color[];
private var fpsColorsTo:Color[];
private var lineColor:Color = new Color(1.0, 1.0, 1.0, 0.25);
private var darkenedBack:Color = new Color(0.0,0.0,0.0,0.5);
private var darkenedBackWhole:Color = new Color(0.0,0.0,0.0,0.25);

private var colorsWrite:Color32[];

private var graphSizeGUI:Rect;

private var stopWatch:System.Diagnostics.Stopwatch;
private var lastElapsed:float;
private var fps:float;
private var graphSizeMin:int;

function Awake(){
    if(gameObject.GetComponent(Camera)==null)
        Debug.LogWarning("FPS Graph needs to be attached to a Camera object");

    CreateLineMaterial();

    fpsColors = [ RenderColor, CpuColor, OtherColor];
    fpsColorsTo =  [fpsColors[0] * 0.7, fpsColors[1] * 0.7, fpsColors[2] * 0.7];
}

function Start () {
    graphSizeMin = frameHistoryLength > 95 ? frameHistoryLength : 95;

    textOverlayMask = new int[graphHeight, graphSizeMin];

    dtHistory = new float[3, frameHistoryLength];

    stopWatch = System.Diagnostics.Stopwatch();
    stopWatch.Start();

    graphTexture = new Texture2D( graphSizeMin, 7, TextureFormat.ARGB32, false, false);

    colorsWrite = new Color32[ graphTexture.width * 7 ];
    graphTexture.filterMode = FilterMode.Point;
    graphSizeGUI = Rect( 0, 0, graphTexture.width * graphMultiply, graphTexture.height * graphMultiply );

    whiteBits(14,23, 4, 11, fpsBits);
    whiteBits(14,48, 4, 11, fpsBits);
    whiteBits(14,93, 4, 11, fpsBits);

    for (x = 0; x < graphTexture.width; ++x) {
        for (y= 0; y < 7; ++y) {
            if(x < 95 && y < 5){
                color = graphKeys[ y*95 + x ];
            }else{
                color.a = 0.0;
            }
            graphTexture.SetPixel(x, y, color);
            colorsWrite[ (y) * graphTexture.width + x ] = color;
        }
    }
    graphTexture.Apply();

    if(audioFeedback)
        initAudio();
}

public function initAudio(){
  audioClip = AudioClip.Create("FPS-BNote", bNote.Length, 1, 44100, false, false);
  audioClip.SetData(bNote, 0);

  audioSource = gameObject.AddComponent( AudioSource );
  audioSource.loop = true;
  audioSource.clip = audioClip;
}

private var xExtern:int;
private var yExtern:int;
private var startAt:int;
private var yOffset:int;
private var xLength:int;

function whiteBits( startX:int, startY:int, textWidth:int, textHeight:int, bits:int[]){
  yExtern = startY;
  for(y=0; y < textWidth; y++){
    xExtern = startX;
    yOffset = y*textHeight;
    for(x=0; x < textHeight; x++){
      textOverlayMask[yExtern, xExtern] = bits[ yOffset + x ];
      xExtern++;
    }

    yExtern++;
  }
}

function addNumberAt( startX:int, startY:int, num:int, isLeading:boolean ){
    if(isLeading && num==0)
        num = -1;
    startAt = num * 4;
    xLength = startAt + 3;

    yExtern = startY;
    for(y=0; y < 5; y++){
        xExtern = startX;
        yOffset = y*39;
        for(x=startAt; x < xLength; x++){
            //textOverlayMask[yExtern, xExtern] = num==-1 ? 0 : numberBits[ yOffset + x ];
            if(num!=-1 && numberBits[ yOffset + x ]){
                x1 = xExtern * graphMultiply + xOff;
                y1 = yExtern * graphMultiply + yOff;
                GL.Vertex3(x1,y1,0);
                GL.Vertex3(x1,y1+1* graphMultiply,0);
                GL.Vertex3(x1+1* graphMultiply,y1+1* graphMultiply,0);
                GL.Vertex3(x1+1* graphMultiply,y1,0);
            }
            xExtern++;
        }

        yExtern++;
    }
}

function addPeriodAt( startX:int, startY:int){
    x1 = startX*graphMultiply + xOff;
    x2 = (startX+1)*graphMultiply + xOff;
    y1 = startY*graphMultiply + yOff;
    y2 = (startY-1)*graphMultiply + yOff;
    GL.Vertex3(x1,y1,0);
    GL.Vertex3(x1,y2,0);
    GL.Vertex3(x2,y2,0);
    GL.Vertex3(x2,y1,0);
}

private var totalSeconds:float;
private var renderSeconds:float;
private var lateSeconds:float;
private var dt:float;
private var frameIter:int = 0;
private var eTotalSeconds:float;
private var emptySceneMemory:float = 1740000;

function Update () {
    eTotalSeconds = stopWatch.Elapsed.TotalSeconds;
    dt = eTotalSeconds - lastElapsed;
    lastElapsed = stopWatch.Elapsed.TotalSeconds;
  if(Time.frameCount>4){
    // Debug.Log("Update seconds:"+stopWatch.Elapsed.TotalSeconds);
    // Debug.Log("Update lastElapsed:"+lastElapsed);
    
    dtHistory[0, frameIter] = dt;

    //Debug.Log('frameIter:'+frameIter);
    //Debug.Log("dt:"+dt);
    frameIter++;

    if(audioFeedback){
      if(audioClip==null)
        initAudio();

      if(audioSource.isPlaying==false)
        audioSource.Play();
    }else if(audioSource && audioSource.isPlaying){
      audioSource.Stop();
    }

    if(audioClip){
      audioSource.pitch = Mathf.Clamp( dt * 90.0 - 0.7, 0.1, 50.0 );
      audioSource.volume = audioFeedbackVolume;
    }
    //Debug.Log("audioSource.pitch:"+audioSource.pitch);

    if(frameIter>=frameHistoryLength)
      frameIter = 0;

    beforeRender = stopWatch.Elapsed.TotalSeconds;
  }

  //Debug.Log("yMulti:"+yMulti + " maxFrame:"+maxFrame);
}


function LateUpdate(){
  // Debug.Log("LateUpdate seconds:"+stopWatch.Elapsed.TotalSeconds);

  eTotalSeconds = stopWatch.Elapsed.TotalSeconds;
  dt = (eTotalSeconds - beforeRender);
  //Debug.Log("OnPostRender time:"+dt);

  dtHistory[2, frameIter] = dt;

  beforeRender = eTotalSeconds;
}

// function FixedUpdate(){
//     Debug.Log("FixedUpdate seconds:"+stopWatch.Elapsed.TotalSeconds);
// }

private var beforeRender:float;
private var fpsVals:float[] = new float[3];
private var x1:float;
private var x2:float;
private var y1:float;
private var y2:float;
private var xOff:float;
private var yOff:float;
private var lineY:int[] = [25, 50, 99];
private var lineY2:int[] = [21, 46, 91];
private var keyOffX:int[] = [61,34,1];
private var splitMb:String[];
private var first:int;
private var second:int;


function OnPostRender(){
    // Debug.Log("OnPostRender seconds:"+stopWatch.Elapsed.TotalSeconds);
    //Debug.Log("OnPostRender lastElapsed:"+lastElapsed);

    // CreateLineMaterial();

    // GL.InvalidateState();
    
    GL.PushMatrix();
    mat.SetPass(0);
    GL.LoadPixelMatrix();
    GL.Begin(GL.QUADS);

    xOff = graphPosition.x*(Screen.width - frameHistoryLength*graphMultiply);
    yOff = Screen.height - 100 * graphMultiply - graphPosition.y*(Screen.height-graphMultiply*107);

    // Shadow for whole graph
    GL.Color( darkenedBackWhole );
    GL.Vertex3(xOff, yOff-8*graphMultiply,0);
    GL.Vertex3(xOff,100 * graphMultiply + yOff,0);
    GL.Vertex3(graphSizeMin * graphMultiply + xOff,100.0* graphMultiply + yOff,0);
    GL.Vertex3(graphSizeMin * graphMultiply + xOff,yOff-8*graphMultiply,0);
    
    maxFrame = 0.0;
    for (x = 0; x < frameHistoryLength; ++x) {
        totalSeconds = dtHistory[ 0, x ];

        if(totalSeconds>maxFrame)
            maxFrame = totalSeconds;
        totalSeconds *= yMulti;
        fpsVals[0] = totalSeconds;

        renderSeconds = dtHistory[ 1, x ];
        renderSeconds *= yMulti;
        fpsVals[1] = renderSeconds;

        lateSeconds = dtHistory[ 2, x ];
        lateSeconds *= yMulti;
        fpsVals[2] = lateSeconds;

        i = x - frameIter - 1;
        if(i<0)
            i = frameHistoryLength + i;
    
        x1 = i * graphMultiply + xOff;
        x2 = (i+1) * graphMultiply + xOff;
        
        for(j = 0; j < fpsVals.Length; j++){
            y1 = j < fpsVals.Length - 1 ? fpsVals[j+1] * graphMultiply + yOff : yOff;
            y2 = fpsVals[j] * graphMultiply + yOff;

            //Debug.Log("x:"+x1+ x+" y:"+y1);

            GL.Color(fpsColorsTo[j]);
            GL.Vertex3(x1,y1,0);
            GL.Vertex3(x2,y1,0);
            GL.Color(fpsColors[j]);
            GL.Vertex3(x2,y2,0);
            GL.Vertex3(x1,y2,0);
        }

      //Debug.Log("x:"+(x-frameIter));
    }

    // Round to nearest relevant FPS
    if(maxFrame < 1.0/120.0){
        maxFrame = 1.0/120.0;
    }else if(maxFrame < 1.0/60.0){
        maxFrame = 1.0/60.0;
    }else if(maxFrame < 1.0/30.0){
        maxFrame = 1.0/30.0;
    }else if(maxFrame < 1.0/15.0){
        maxFrame = 1.0/15.0;
    }else if(maxFrame < 1.0/10.0){
        maxFrame = 1.0/10.0;
    }else if(maxFrame < 1.0/5.0){
        maxFrame = 1.0/5.0;
    }

    yMulti = graphHeight / maxFrame;


    // Add Horiz Lines
    GL.Color( lineColor );
    x1 = 28 * graphMultiply + xOff;
    x2 = graphSizeMin*graphMultiply + xOff;
    for(i = 0; i < lineY.Length; i++){
        y1 = lineY[i] * graphMultiply + yOff;
        y2 = (lineY[i]+1)* graphMultiply + yOff;
        GL.Vertex3(x1,y1,0);
        GL.Vertex3(x1,y2,0);
        GL.Vertex3(x2,y2,0);
        GL.Vertex3(x2,y1,0);
    }

    // Add FPS Shadows
    GL.Color( darkenedBack );
    x2 = 27 * graphMultiply + xOff;
    for(i = 0; i < lineY.Length; i++){
        y1 = lineY2[i] * graphMultiply + yOff;
        y2 = (lineY2[i]+9)* graphMultiply + yOff;
        GL.Vertex3(xOff, y1,0);
        GL.Vertex3(xOff, y2,0);
        GL.Vertex3(x2, y2,0);
        GL.Vertex3(x2, y1,0);
    }

    // Add Key Boxes
    for(i = 0; i < keyOffX.Length; i++){
        x1 = keyOffX[i]*graphMultiply + xOff + 1*graphMultiply;
        x2 = (keyOffX[i]+4)*graphMultiply + xOff + 1*graphMultiply;
        y1 = (5)*graphMultiply + yOff - 9*graphMultiply;
        y2 = (1)*graphMultiply + yOff - 9*graphMultiply;
        GL.Color( fpsColorsTo[i] );
        GL.Vertex3(x1,y1,0);
        GL.Vertex3(x1,y2,0);
        GL.Vertex3(x2,y2,0);
        GL.Vertex3(x2,y1,0);
    }

    for(i = 0; i < keyOffX.Length; i++){
        x1 = keyOffX[i]*graphMultiply + xOff;
        x2 = (keyOffX[i]+4)*graphMultiply + xOff;
        y1 = (5)*graphMultiply + yOff - 8*graphMultiply;
        y2 = (1)*graphMultiply + yOff - 8*graphMultiply;
        GL.Color( fpsColors[i] );
        GL.Vertex3(x1,y1,0);
        GL.Vertex3(x1,y2,0);
        GL.Vertex3(x2,y2,0);
        GL.Vertex3(x2,y1,0);
    }

    // Draw Key Text
    GL.Color(Color.white);
    for ( x = 0; x < graphTexture.width; ++x) {
        for (y= 0; y < graphHeight; ++y) {
            if(textOverlayMask[y,x]){
                x1 = x*graphMultiply + xOff;
                x2 = x*graphMultiply + 1*graphMultiply + xOff;
                y1 = y*graphMultiply + yOff;
                y2 = y*graphMultiply + 1*graphMultiply + yOff;
                GL.Vertex3(x1,y1,0);
                GL.Vertex3(x1,y2,0);
                GL.Vertex3(x2,y2,0);
                GL.Vertex3(x2,y1,0);
            }
        }
    }

    // Draw Mb
    for ( x = 0; x < 9; ++x) {
        for (y= 0; y < 4; ++y) {
            if(mbBits[y*9 + x]){
                x1 = x*graphMultiply + xOff + 111*graphMultiply;
                x2 = x*graphMultiply + 1*graphMultiply + xOff + 111*graphMultiply;
                y1 = y*graphMultiply + yOff + -7*graphMultiply;
                y2 = y*graphMultiply + 1*graphMultiply + yOff + -7*graphMultiply;
                GL.Vertex3(x1,y1,0);
                GL.Vertex3(x1,y2,0);
                GL.Vertex3(x2,y2,0);
                GL.Vertex3(x2,y1,0);
            }
        }
    }

    if(maxFrame>0){
        fps = Mathf.Round(1.0/maxFrame);
        addNumberAt( 1, 93, (fps / 100)%10, true );
        addNumberAt( 5, 93, (fps / 10.0)%10, true );
        
        addNumberAt( 9, 93, fps % 10, false );

        fps *= 2;
        addNumberAt( 1, 48, (fps / 100)%10, true );
        addNumberAt( 5, 48, (fps / 10)%10, true );
        addNumberAt( 9, 48, fps % 10, false );

        fps *= 1.5;
        addNumberAt( 1, 23, (fps / 100)%10, true );
        addNumberAt( 5, 23, (fps / 10)%10, true );
        addNumberAt( 9, 23, fps % 10, false );

        // Debug.Log("mem:"+ ((System.GC.GetTotalMemory(false) ) / 1000000.0 ).ToString("F2") + "mb");

        var mem:float = (System.GC.GetTotalMemory(true) ) / 1000000.0;
        // mem = 0.13;
       
        if(mem<1.0){
            splitMb = mem.ToString("F2").Split("."[0]);

            if(splitMb[1][0]=="0"[0]){
                first = 0;
                second = int.Parse( splitMb[1] );
            }else{
                first = int.Parse( splitMb[1] );
                second = first%10;
                first = (first/10)%10;
            }
            
            addPeriodAt( 100, -6);
            addNumberAt( 102, -7, first, false );
            addNumberAt( 106, -7, second, false );
        }else{
            splitMb = mem.ToString("F1").Split("."[0]);
            first = int.Parse( splitMb[0] );

            if(first>=10)
                addNumberAt( 96, -7, first/10, false );

            second = first%10;
            if(second<0)
                second = 0;
            addNumberAt( 100, -7, second, false );
            addPeriodAt( 104, -6);
            addNumberAt( 106, -7, int.Parse( splitMb[1] ), false );
        }
    }

    GL.End();
    GL.PopMatrix();


    dt = (stopWatch.Elapsed.TotalSeconds - beforeRender);
    //Debug.Log("OnPostRender time:"+dt);

    dtHistory[1, frameIter] = dt;

    eTotalSeconds = stopWatch.Elapsed.TotalSeconds;

    dt = (eTotalSeconds - lastElapsed);
    //Debug.Log("Update time:"+dt*1000 + " Time.delta:"+Time.deltaTime*1000);

    
    //lastElapsed = eTotalSeconds;

    // beforeRender = eTotalSeconds;
}

function OnGUI(){
    //Debug.Log("OnGUI time:"+stopWatch.Elapsed);
    if(Time.frameCount>4)
        GUI.DrawTexture( new Rect(graphPosition.x*(Screen.width-graphMultiply*frameHistoryLength), graphPosition.y*(Screen.height-graphMultiply*107) + 100*graphMultiply, graphSizeGUI.width, graphSizeGUI.height), graphTexture );
}
