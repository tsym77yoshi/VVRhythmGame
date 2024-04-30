class CharaKeyRangeAdjustmentValue{
private static readonly string charaKeyRangeAdjustmentValues = @"3002,-4
3000,-4
3006,-5
3004,-4
3037,-9
3003,-2
3001,0
3007,-3
3005,0
3038,-7
3075,-3
3076,6
3008,-2
3010,0
3009,-8
3065,-5
3011,-17
3039,-9
3040,-14
3041,-18
3012,-14
3032,-8
3033,-7
3034,-9
3035,-3
3013,-22
3081,-18
3082,-23
3083,-21
3084,-27
3085,-22
3014,-7
3016,-7
3015,-2
3018,-6
3017,-4
3020,-5
3066,-7
3077,-2
3078,-3
3079,-2
3080,-5
3021,-18
3023,-6
3024,-3
3025,-7
3026,0
3027,-7
3028,-2
3029,-8
3030,-10
3031,-9
3042,-18
3043,-6
3044,-12
3045,-7
3046,-4
3047,-6
3048,-3
3049,-4
3051,-15
3052,-21
3053,-17
3054,-2
3055,-8
3056,-9
3057,-7
3058,-1
3059,-3
3061,-8
3062,-4
3063,-2
3064,-4
3067,-14
3068,-2
3069,-4
3070,2
3071,-33
3072,0
3073,-10
3074,-4";
public static int GetCharaKeyRangeAdjustmentValue(int styleid){
  string[] charaValues = charaKeyRangeAdjustmentValues.Split("\r\n");
  foreach(string charaValue in charaValues){
    if(charaValue.Split(",")[0]==styleid.ToString()){
      return int.Parse(charaValue.Split(",")[1]);
    }
  }
  return 0;
}
}